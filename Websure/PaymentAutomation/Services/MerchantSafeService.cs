using System.Globalization;
using System.Text;
using System.Xml;
using Microsoft.Extensions.Options;
using PaymentAutomationAPI.Models;
using PaymentAutomationAPI.Interfaces;

namespace PaymentAutomationAPI.Services
{
    public class MerchantSafeService : IMerchantSafeService
    {
        private readonly HttpClient _httpClient;
        private readonly MerchantSafeConfig _config;
        private readonly ILogger<MerchantSafeService> _logger;

        public MerchantSafeService(
            HttpClient httpClient,
            IOptions<MerchantSafeConfig> config,
            ILogger<MerchantSafeService> logger)
        {
            _httpClient = httpClient;
            _config = config.Value;
            _logger = logger;
        }

        public async Task<MerchantSafeResponse> AddCard(MerchantSafeRequest request)
        {
            var xmlRequest = GenerateXmlRequest("ADDPAN", request);
            return await SendRequest(xmlRequest);
        }

        public async Task<MerchantSafeResponse> UpdateCard(MerchantSafeRequest request)
        {
            var xmlRequest = GenerateXmlRequest("UPDATEPAN", request);
            return await SendRequest(xmlRequest);
        }

        public async Task<MerchantSafeResponse> GetCards(string safeKey)
        {
            var request = new MerchantSafeRequest
            {
                SafeKey = safeKey,
            };
            var xmlRequest = GenerateXmlRequest("GETPANS", request);
            return await SendRequest(xmlRequest);
        }

        public async Task<MerchantSafeResponse> DisableCards(string safeKey)
        {
            var request = new MerchantSafeRequest
            {
                SafeKey = safeKey,
            };
            var xmlRequest = GenerateXmlRequest("DISABLEPANS", request);
            return await SendRequest(xmlRequest);
        }

        public async Task<MerchantSafeResponse> ProcessPayment(MerchantSafeRequest request)
        {
            var xmlRequest = GenerateXmlRequest("Auth", request);
            return await SendRequest(xmlRequest);
        }

        private string GenerateXmlRequest(string type, MerchantSafeRequest request)
        {
            var xml = new StringBuilder();
            xml.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xml.Append("<CC5Request>");
            xml.AppendFormat("<Name>{0}</Name>", _config.Username);
            xml.AppendFormat("<Password>{0}</Password>", _config.Password);
            xml.AppendFormat("<ClientId>{0}</ClientId>", _config.ClientId);
            xml.AppendFormat("<Type>MerchantSafe</Type>");

            if (request.CardInfo != null)
            {
                xml.AppendFormat("<Number>{0}</Number>", request.CardInfo.Number);
                xml.AppendFormat("<Expires>{0}</Expires>", request.CardInfo.Expires);
            }
            else if (!string.IsNullOrEmpty(request.SafeKey))
            {
                xml.AppendFormat("<Number>{0}</Number>", request.SafeKey);
            }

            if (request.Amount.HasValue)
            {
                xml.AppendFormat("<Total>{0}</Total>", request.Amount.Value.ToString("0.00", CultureInfo.InvariantCulture));
                xml.AppendFormat("<Currency>{0}</Currency>", request.Currency);
            }

            xml.Append("<Extra>");
            if (!string.IsNullOrEmpty(type) && type != "Auth")
            {
                xml.AppendFormat("<MERCHANTSAFE>{0}</MERCHANTSAFE>", type);
            }
            if (!string.IsNullOrEmpty(request.SafeKey))
            {
                xml.AppendFormat("<MERCHANTSAFEKEY>{0}</MERCHANTSAFEKEY>", request.SafeKey);
            }
            if (request.CardInfo?.CardOwner != null)
            {
                xml.AppendFormat("<MERCHANTSAFECARDOWNER>{0}</MERCHANTSAFECARDOWNER>", request.CardInfo.CardOwner);
            }
            if (request.Priority.HasValue)
            {
                xml.AppendFormat("<MERCHANTSAFEPRIORITY>{0}</MERCHANTSAFEPRIORITY>", request.Priority.Value);
            }
            if (request.AccountClosureDay.HasValue)
            {
                xml.AppendFormat("<MERCHANTSAFEACCOUNTCLOSURE>{0}</MERCHANTSAFEACCOUNTCLOSURE>", request.AccountClosureDay.Value);
            }
            if (!string.IsNullOrEmpty(request.Description))
            {
                xml.AppendFormat("<MERCHANTSAFEDESCRIPTION>{0}</MERCHANTSAFEDESCRIPTION>", request.Description);
            }
            xml.Append("</Extra>");
            xml.Append("</CC5Request>");

            return xml.ToString();
        }

        private async Task<MerchantSafeResponse> SendRequest(string xmlRequest)
        {
            try
            {
                var content = new StringContent(xmlRequest, Encoding.UTF8, "application/x-www-form-urlencoded");
                var response = await _httpClient.PostAsync(_config.ApiUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(responseContent);

                return new MerchantSafeResponse
                {
                    ProcReturnCode = xmlDoc.SelectSingleNode("//ProcReturnCode")?.InnerText,
                    Response = xmlDoc.SelectSingleNode("//Response")?.InnerText,
                    ErrorMessage = xmlDoc.SelectSingleNode("//ErrMsg")?.InnerText,
                    TransactionId = xmlDoc.SelectSingleNode("//TransId")?.InnerText,
                    RawResponse = responseContent
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing MerchantSafe request");
                throw;
            }
        }
    }

}
