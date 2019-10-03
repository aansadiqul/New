using System.Text;
using Abp.Authorization;
using ABD.Payment.Dto;
using System.IO;
using System.Net;
using Abp.Runtime.Session;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore;
using Abp.Linq.Extensions;
using ABD.ADOrders.Dto;
using ABD.Entities;
using Abp.Extensions;
using Abp.UI;
using ABD.Customers.Dto;
using ABD.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ABD.Payment
{
    [AbpAuthorize]
    public class PaymentAppService : ABDAppServiceBase, IPaymentAppService
    {
        private readonly IAbpSession _session;
        private readonly IRepository<Entities.Payment> _paymentRepository;
        private readonly IRepository<UserProfile> _userProfileRepository;

        public PaymentAppService(IAbpSession session,
            IRepository<Entities.Payment> paymentRepository,
            IRepository<UserProfile> userProfileRepository)
        {
            _session = session;
            _paymentRepository = paymentRepository;
            _userProfileRepository = userProfileRepository;
        }

        public async Task<PagedResultDto<PaymentDto>> GetAll(GetPaymentInput input)
        {
            var paymentQuery = CreateFilteredQuery(input);
            var userQueryCount = await paymentQuery.CountAsync();
            var payments = await paymentQuery.OrderByDescending(p => p.Id).PageBy(input).ToListAsync();

            return new PagedResultDto<PaymentDto>(
                userQueryCount,
                ObjectMapper.Map<List<PaymentDto>>(payments)
            );
        }

        public async Task<int> Create(PaymentInput input)
        {
            var userProfile = ObjectMapper.Map<CustomerDto>(await _userProfileRepository.GetAll().Where(x => x.UserId == _session.UserId).FirstOrDefaultAsync());
            input.CustomerName = userProfile.FName + " " + userProfile.LName;
            input.CompanyName = userProfile.CompanyName;
            var paymentId = await _paymentRepository.InsertAndGetIdAsync(ObjectMapper.Map<Entities.Payment>(input));
            return paymentId;
        }

        public KeyedSaleResponse CheckOut(PaymentRequestDto PaymentRequest)
        {
            var KeyedSaleResponse = new KeyedSaleResponse();
            KeyedSaleResponse = BuildTransaction(GetToken(), PaymentRequest);
            return KeyedSaleResponse;
        }

        protected KeyedSaleResponse BuildTransaction(OAuthTokenDto oAuthResult, PaymentRequestDto requestKeyedSale)
        {
            var KeyedSaleResponse = new KeyedSaleResponse();
            if (oAuthResult.ErrorFlag == false)
            {
                string OAuth = String.Format("Bearer {0}", oAuthResult.AccessToken);
                KeyedSaleResponse = KeyedSaleTrans(OAuth, requestKeyedSale);
                
            }
            return KeyedSaleResponse;
        }
        protected KeyedSaleResponse KeyedSaleTrans(string token, PaymentRequestDto keyedSaleRequest)
        {

            // Header details are available at Authentication header page.
            string methodUrl = ApiEndPointConfigurationDto.UrlKeyedSale;

            //converting request into JSON string
            var requestJSON = GetSeralizedString(keyedSaleRequest);

            //Optional - Display Json Request 
            //System.Web.HttpContext.Current.Response.Write ("<br>" + "Json Request: " + requestJSON + "<br>");

            //call for actual request and response
            var tempResponse = ProcessTransaction(methodUrl, token, requestJSON);

            //Create and assign the deseralized object to appropriate response type
            var keyedSaleResponse = new KeyedSaleResponse();
            keyedSaleResponse = DeserializeResponse<KeyedSaleResponse>(tempResponse);

            //Assign the http error 
            AssignError(tempResponse, (PayTraceBasicResponse)keyedSaleResponse);

            //Return the Desearlized object
            return keyedSaleResponse;

        }
        protected static string GetSeralizedString<T>(T obj)
        {
           var requestJSON = JsonConvert.SerializeObject(obj);
           return requestJSON;
        }
        protected static T DeserializeResponse<T>(TempResponse tempResponse)
        {
            T returnObject = default(T);

            //var jsSerializer= new JavaScriptSerializer ();

            //optional - Display the Json Response
            //DisplayJsonResponse(tempResponse.JsonResponse);


            if (null != tempResponse.JsonResponse)
            {
                // parse JSON data into C# obj
                //returnObject = jsSerializer.Deserialize<T>(tempResponse.JsonResponse);
                returnObject = JsonConvert.DeserializeObject<T>(tempResponse.JsonResponse);

            }

            return returnObject;
        }
        protected static void AssignError(TempResponse tempResponse, PayTraceBasicResponse basicResponse)
        {
            basicResponse.HttpErrorMessage = tempResponse.ErrorMessage;
        }
        protected TempResponse ProcessTransaction(string methodUrl, string token, string requestData)
        {

            // Header details are available at Authentication header page.

            string Baseurl = ApiEndPointConfigurationDto.BaseUrl; //Production.

            // variables for request stream and Respone reader 
            Stream dataStream = null;
            StreamReader reader = null;
            WebResponse response = null;

            TempResponse objTempResponse = new TempResponse();

            try
            {
                //Set the request header

                //Create a request using a URL that can receive a post. 
                WebRequest request = WebRequest.Create(Baseurl + methodUrl);

                // Set the Method property of the request to POST.
                request.Method = "POST";

                //to set HTTP version of the current request, use the Version10 and Version11 fields of the HttpVersion class.
                ((HttpWebRequest)request).ProtocolVersion = HttpVersion.Version11;

                // optional - to set the Accept property, cast the WebRequest object into HttpWebRequest class
                //((HttpWebRequest)request).Accept = "*/*";

                //Set the ContentType property of the WebRequest.
                request.ContentType = "application/json";
                //set the Authorization token
                ((HttpWebRequest)request).Headers[HttpRequestHeader.Authorization] = token;

                byte[] byteArray = Encoding.UTF8.GetBytes(requestData);

                // Set the ContentLength property of the WebRequest.
                request.ContentLength = byteArray.Length;

                // Get the request stream.
                dataStream = request.GetRequestStream();

                // Write the data to the request stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();

                // To Get the response.
                response = request.GetResponse();

                // Assuming Response status is OK otherwise catch{} will be excuted 
                // Get the stream containing content returned by the server.
                dataStream = response.GetResponseStream();

                // Open the stream using a StreamReader for easy access.
                reader = new StreamReader(dataStream);

                // Read the content.
                string responseFromServer = reader.ReadToEnd();

                // Assign/store Transaction Json Response to TempResposne Object 
                objTempResponse.JsonResponse = responseFromServer;

                return objTempResponse;
            }
            catch (WebException e)
            {

                // This exception will be raised if the server didn't return 200 - OK within response.
                // Retrieve more information about the error and API resoponse

                if (e.Response != null)
                {
                    //to retrieve the actual JSON response when any error occurs.
                    using (var responseStream = e.Response.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            string temp = (new StreamReader(responseStream)).ReadToEnd();
                            objTempResponse.JsonResponse = temp;
                        }
                    }

                    //Retrive http Error 
                    HttpWebResponse err = (HttpWebResponse)e.Response;
                    if (err != null)
                        objTempResponse.ErrorMessage = ((int)err.StatusCode) + " " + err.StatusDescription;
                }
                //Do your own error logging in this case
            }
            finally
            {
                // Clean up the streams.
                if (null != reader)
                    reader.Close();

                if (null != dataStream)
                    dataStream.Close();

                if (null != response)
                    response.Close();
            }

            //Do your code here
            return objTempResponse;

        }
        protected OAuthTokenDto GetToken()
        {
            // Those URL are available at Authentication header page.
            string BaseUrl = ApiEndPointConfigurationDto.BaseUrl;
            string OAuthUrl = ApiEndPointConfigurationDto.UrlOAuth;

            // variables for request stream and Respone reader 
            Stream dataStream = null;
            StreamReader reader = null;
            WebResponse response = null;

            //object 
            OAuthTokenDto OAuthTokenResult = new OAuthTokenDto();

            try
            {
                // Create a request using a URL that can receive a post. 
                WebRequest request = WebRequest.Create(BaseUrl + OAuthUrl);

                // Set the Method property of the request to POST.
                request.Method = "POST";

                //to set HTTP version of the current request, use the Version10 and Version11 fields of the HttpVersion class.
                ((HttpWebRequest)request).ProtocolVersion = HttpVersion.Version11;

                // optional - to set the Accept property, cast the WebRequest object into HttpWebRequest class
                ((HttpWebRequest)request).Accept = "*/*";

                //Set the ContentType property of the WebRequest.
                request.ContentType = "application/x-www-form-urlencoded";

                // Create Request data and convert it to a byte array.
                var requestData = ApiAccessCredentialsDto.GetFormattedRequest();

                byte[] byteArray = Encoding.UTF8.GetBytes(requestData);

                // Set the ContentLength property of the WebRequest.
                request.ContentLength = byteArray.Length;

                // Get the request stream.
                dataStream = request.GetRequestStream();

                // Write the data to the request stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();

                // To Get the response.
                response = request.GetResponse();

                // Assuming Respose status is OK otherwise catch{} will be excuted 
                // Get the stream containing content returned by the server.
                dataStream = response.GetResponseStream();

                // Open the stream using a StreamReader for easy access.
                reader = new StreamReader(dataStream);

                // Read the content.
                string responseFromServer = reader.ReadToEnd();

                // Display the Response content
                OAuthTokenResult = AuthTokenData(responseFromServer);

            }
            catch (WebException e)
            {

                // This exception will be raised if the server didn't return 200 - OK within response.

                // Retrieve more information about the error 

                OAuthTokenResult.ErrorFlag = true;

                if (e.Response != null)
                {
                    using (var responseStream = e.Response.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            string temp = (new StreamReader(responseStream)).ReadToEnd();
                            OAuthTokenResult.ObjError = JsonConvert.DeserializeObject<OAuthErrorDto>(temp);
                        }
                    }

                    //Retrive http Error 
                    HttpWebResponse err = (HttpWebResponse)e.Response;
                    OAuthTokenResult.ObjError.HttpTokenError = ((int)err.StatusCode) + " " + err.StatusDescription;
                }
                //Do your own error logging in this case
            }
            finally
            {
                // Clean up the streams.
                if (null != reader)
                    reader.Close();

                if (null != dataStream)
                    dataStream.Close();

                if (null != response)
                    response.Close();
            }

            //Do your code here
            return OAuthTokenResult;
        }

        protected OAuthTokenDto AuthTokenData(string responseData)
        {
            // Create an object to parse JSON data
            OAuthTokenDto objOauthToken = null;

            if (null != responseData)
            {
                // parase JSON data into C# obj
                objOauthToken = JsonConvert.DeserializeObject<OAuthTokenDto>(responseData);

                //optional as by default it will be false 		
                objOauthToken.ErrorFlag = false;
            }
            return objOauthToken;
        }

        private IQueryable<Entities.Payment> CreateFilteredQuery(GetPaymentInput input)
        {
            return _paymentRepository.GetAll()
                .Include(x => x.PaymentType)
                .Include(x => x.OrderType)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                    x => x.TransactionId.Contains(input.Keyword) || 
                         x.CustomerName.Contains(input.Keyword) ||
                         x.CompanyName.Contains(input.Keyword) ||
                         x.OrderType.Name.Contains(input.Keyword) ||
                         x.PaymentType.Name.Contains(input.Keyword));
        }

    }
}
