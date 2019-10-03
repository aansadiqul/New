using System;
using System.Collections.Generic;
using System.Text;

namespace ABD.Payment.Dto
{
    public class ApiEndPointConfigurationDto
    {
        public const string BaseUrl = "https://api.paytrace.com";
        public const string ApiVersion = "/v1";
        public const string UrlOAuth = "/oauth/token";
        public const string UrlKeyedSale = "/v1/transactions/sale/keyed";
        public const string UrlSwipedSale = "/v1/transactions/sale/swiped";
        public const string UrlKeyedAuthorization = "/v1/transactions/authorization/keyed";
        public const string UrlKeyedRefund = "/v1/transactions/refund/keyed";
        public const string UrlCapture = "/v1/transactions/authorization/capture";
        public const string UrlVoidTransaction = "/v1/transactions/void";
        public const string UrlCreateCustomer = "/v1/customer/create";
        public const string UrlVaultSaleByCustomerId = "/v1/transactions/sale/by_customer";
    }
}
