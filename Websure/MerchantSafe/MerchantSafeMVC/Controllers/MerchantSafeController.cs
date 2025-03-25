using MerchantSafeMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using System.Xml;
using MerchantSafeAPI.Interfaces;
using MerchantSafeAPI.Models;

public class MerchantSafeController : Controller
{
    private readonly IMerchantSafeService _merchantSafeService;
    Uri _baseAddress = new Uri("http://localhost:5145/api");
    private readonly HttpClient _httpClient;
    private readonly ILogger<MerchantSafeController> _logger;

    public MerchantSafeController(HttpClient httpClient, ILogger<MerchantSafeController> logger, IMerchantSafeService merchantSafeService)
    {
        _merchantSafeService = merchantSafeService;
        _httpClient = httpClient;
        _httpClient.BaseAddress = _baseAddress;
        _logger = logger;
        _merchantSafeService = merchantSafeService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult AddCard()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddCard(MerchantSafeRequestViewModel request)
    {
        try
        {
            var response = await SendRequest(_httpClient.BaseAddress + "/MerchantSafe/add-card", request);
            return await ProcessXmlResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing AddCard request");
            return View("Response", new MerchantSafeResponseViewModel
            {
                ErrorMessage = $"Error processing request: {ex.Message}"
            });
        }
    }

    [HttpGet]
    public IActionResult UpdateCard()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UpdateCard(MerchantSafeRequestViewModel request)
    {
        try
        {
            var response = await SendRequest(_httpClient.BaseAddress + "/MerchantSafe/update-card", request);
            return await ProcessXmlResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing UpdateCard request");
            return View("Response", new MerchantSafeResponseViewModel
            {
                ErrorMessage = $"Error processing request: {ex.Message}"
            });
        }
    }

    [HttpGet]
    public IActionResult GetCards()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> GetCards(string safeKey)
    {
        try
        {
            var response = await _httpClient.GetAsync(_httpClient.BaseAddress + $"/MerchantSafe/cards/{safeKey}");
            var responseContent = await response.Content.ReadAsStringAsync();
            return await ProcessXmlResponse(responseContent, safeKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing GetCards request");
            return View("Response", new MerchantSafeResponseViewModel
            {
                ErrorMessage = $"Error processing request: {ex.Message}"
            });
        }
    }

    [HttpGet]
    public IActionResult DisableCards()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> DisableCards(string safeKey)
    {
        try
        {
            var response = await _httpClient.PostAsync(_httpClient.BaseAddress + $"/MerchantSafe/disable-cards/{safeKey}", null);
            var responseContent = await response.Content.ReadAsStringAsync();
            return await ProcessXmlResponse(responseContent, safeKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing DisableCards request");
            return View("Response", new MerchantSafeResponseViewModel
            {
                ErrorMessage = $"Error processing request: {ex.Message}"
            });
        }
    }

    [HttpGet]
    public IActionResult ProcessPayment()
    {
        var model = new MerchantSafeRequestViewModel();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ProcessPayment(MerchantSafeRequestViewModel request, int numberOfMonths)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            var apiRequest = MapToApiRequest(request);
            var apiResponse = await _merchantSafeService.ProcessPayment(apiRequest);

            if (apiResponse == null)
            {
                ModelState.AddModelError("", "No response received from payment service");
                return View(request);
            }

            if (apiResponse.ProcReturnCode == "00") // Assuming "00" is success code
            {
                var distribution = new PaymentDistributionViewModel
                {
                    TotalAmount = request.Amount ?? 0,
                    NumberOfMonths = numberOfMonths,
                    TransactionId = apiResponse.TransactionId ?? string.Empty,
                    MonthlyPayments = GenerateMonthlyPayments(request.Amount ?? 0, numberOfMonths)
                };

                return View("PaymentDistribution", distribution);
            }

            // If we get here, there was an error in the payment process
            ModelState.AddModelError("", apiResponse.ErrorMessage ?? "Payment processing failed");
            return View(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment");
            ModelState.AddModelError("", "Error processing payment: " + ex.Message);
            return View(request);
        }
    }



    private MerchantSafeResponseViewModel MapToViewModel(MerchantSafeResponse response)
    {
        return new MerchantSafeResponseViewModel
        {
            OrderId = response.TransactionId, // Using TransactionId for OrderId
            GroupId = "", // Map from response if available
            Response = response.Response,
            AuthCode = "", // Map from response if available
            HostRefNum = "", // Map from response if available
            ProcReturnCode = response.ProcReturnCode,
            TransId = response.TransactionId, // Using TransactionId for TransId
            ErrMsg = response.ErrorMessage,
            ErrorMessage = response.ErrorMessage,
            RawResponse = response.RawResponse,
            Extra = new ExtraResponseInfo
            {
                SettleId = "", // Map from response if available
                TrxDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), // Or map from response
                ErrorCode = "", // Map from response if available
                HostMsg = "", // Map from response if available
                NumCode = "" // Map from response if available
            }
        };
    }


    private MerchantSafeRequest MapToApiRequest(MerchantSafeRequestViewModel requestViewModel)
    {
        if (requestViewModel == null)
            throw new ArgumentNullException(nameof(requestViewModel));

        return new MerchantSafeRequest
        {
            SafeKey = requestViewModel.SafeKey,
            Amount = requestViewModel.Amount,
            Currency = requestViewModel.Currency,
            Description = requestViewModel.Description,
            Priority = requestViewModel.Priority,
            AccountClosureDay = requestViewModel.AccountClosureDay,
            CardInfo = requestViewModel.CardInfo != null ? new CardInfo
            {
                Number = requestViewModel.CardInfo.Number,
                Expires = requestViewModel.CardInfo.Expires,
                Cvv = requestViewModel.CardInfo.Cvv,
                CardOwner = requestViewModel.CardInfo.CardOwner
            } : null
        };
    }


    private List<MonthlyPayment> GenerateMonthlyPayments(decimal totalAmount, int numberOfMonths)
    {
        var monthlyPayments = new List<MonthlyPayment>();
        var monthlyAmount = Math.Round(totalAmount / numberOfMonths, 2);
        var remainingAmount = totalAmount - (monthlyAmount * (numberOfMonths - 1));

        var currentDate = DateTime.Now;

        for (int i = 0; i < numberOfMonths; i++)
        {
            monthlyPayments.Add(new MonthlyPayment
            {
                PaymentDate = currentDate.AddMonths(i),
                Amount = i == numberOfMonths - 1 ? remainingAmount : monthlyAmount,
                IsEdited = false
            });
        }

        return monthlyPayments;
    }

    [HttpPost]
    public IActionResult SaveDistribution(PaymentDistributionViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("PaymentDistribution", model);
        }

        var total = model.MonthlyPayments.Sum(x => x.Amount);
        if (Math.Abs(total - model.TotalAmount) > 0.01m)
        {
            ModelState.AddModelError("", "The sum of monthly payments must equal the total payment amount.");
            return View("PaymentDistribution", model);
        }

        // TODO: Save the distribution to your database

        return RedirectToAction("Index");
    }



    private async Task<MerchantSafeResponseViewModel> SendRequest(string url, MerchantSafeRequestViewModel request)
    {
        var jsonRequest = JsonSerializer.Serialize(request);
        // Change the content type to application/json
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        // Add headers
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        var response = await _httpClient.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"API request failed with status code: {response.StatusCode}");
            return new MerchantSafeResponseViewModel
            {
                ErrorMessage = $"API request failed: {response.StatusCode}",
                Response = "Error",
                ProcReturnCode = "999" // Or whatever error code you want to use
            };
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        try
        {
            return JsonSerializer.Deserialize<MerchantSafeResponseViewModel>(responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing response");
            return new MerchantSafeResponseViewModel
            {
                ErrorMessage = "Error processing server response",
                Response = "Error",
                ProcReturnCode = "999"
            };
        }
    }


    private async Task<IActionResult> ProcessXmlResponse(string responseContent, string safeKey = null)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var jsonResponse = JsonSerializer.Deserialize<MerchantSafeResponseViewModel>(responseContent, options);

            if (string.IsNullOrEmpty(responseContent))
            {
                return View("Response", new MerchantSafeResponseViewModel { ErrorMessage = "No response received." });
            }

            // Process the XML response
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(responseContent);

            var cardDetails = new MerchantSafeCardDetailsViewModel
            {
                ProcReturnCode = xmlDoc.SelectSingleNode("//ProcReturnCode")?.InnerText ?? string.Empty,
                Response = xmlDoc.SelectSingleNode("//Response")?.InnerText ?? string.Empty,
                Label = xmlDoc.SelectSingleNode("//Extra/PAN1LABEL")?.InnerText ?? string.Empty,
                SafeKeyLastModified = xmlDoc.SelectSingleNode("//Extra/SAFEKEYLASTMODIFIED")?.InnerText ?? string.Empty,
                Pan = xmlDoc.SelectSingleNode("//Extra/PAN1")?.InnerText ?? string.Empty,
                PanStatus = xmlDoc.SelectSingleNode("//Extra/PAN1STATUS")?.InnerText ?? string.Empty,
                IndexAccountClosure = xmlDoc.SelectSingleNode("//Extra/PAN1INDEXACCOUNTCLOSURE")?.InnerText ?? string.Empty,
                PanExpiry = xmlDoc.SelectSingleNode("//Extra/PAN1EXPIRY")?.InnerText ?? string.Empty,
                PanOwner = xmlDoc.SelectSingleNode("//Extra/PAN1OWNER")?.InnerText ?? string.Empty,
                NumberOfPans = xmlDoc.SelectSingleNode("//Extra/NUMBEROFPANS")?.InnerText ?? string.Empty,
                IndexOrder = xmlDoc.SelectSingleNode("//Extra/PAN1INDEXORDER")?.InnerText ?? string.Empty,
                PanDescription = xmlDoc.SelectSingleNode("//Extra/PAN1DESCRIPTION")?.InnerText ?? string.Empty
            };

            if (safeKey != null)
            {
                ViewData["SafeKey"] = safeKey;
            }

            return View("CardDetails", cardDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing XML response: {ResponseContent}", responseContent);
            return View("Response", new MerchantSafeResponseViewModel
            {
                ErrorMessage = "Error processing response",
                RawResponse = responseContent
            });
        }
    }


    private async Task<IActionResult> ProcessXmlResponse(MerchantSafeResponseViewModel response)
    {
        return await ProcessXmlResponse(JsonSerializer.Serialize(response));
    }
}
