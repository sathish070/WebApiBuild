using Core_Arca.Data;
using Core_Arca.Models;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace Core_Arca.Helpers
{
    public class ServiceNowHelper
    {
        private readonly HttpClient _authClient;
        private readonly IConfiguration _configuration;
        private string _accessToken;
        private DateTime _tokenExpiry;
        private readonly HttpClient _httpClient = new();

        public ServiceNowHelper(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _authClient = httpClientFactory.CreateClient("ServiceNowAuthClient");
            _configuration = configuration;
        }

        internal (string username, string password) GetRequestedUsernamePassword(StringValues authValue)
        {
            var authHeader = AuthenticationHeaderValue.Parse(authValue);
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
            return (credentials[0], credentials[1]);
        }

        public void RemoveAuthorizationToken()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        private async Task<string> GetAccessTokenAsync()
        {
            if (_accessToken == null || DateTime.UtcNow >= _tokenExpiry)
            {
                await RefreshTokenAsync();
            }

            return _accessToken;
        }

        private async Task RefreshTokenAsync()
        {
            var clientId = _configuration["servicenow:clientId"];
            var clientSecret = _configuration["servicenow:clientSecret"];
            var userName = _configuration["servicenow:userName"];
            var password = _configuration["servicenow:password"];

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_configuration["servicenow:baseUrl"]}oauth_token.do");
            var body = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "username", userName },
                { "password", password }
            };

            request.Content = new FormUrlEncodedContent(body);

            var response = await _authClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<dynamic>(content);

            _accessToken = tokenResponse.access_token;

            double timeInSeconds = tokenResponse.expires_in;

            _tokenExpiry = DateTime.UtcNow.AddSeconds(timeInSeconds);
        }
            
        public async Task<Device> GetCustomerBySerialAsync(string id)
        {
            try
            {
                var accessToken = await GetAccessTokenAsync();
                var baseUrl = $"{_configuration["servicenow:baseUrl"]}api/now/table/alm_asset";

                var queryParameters = HttpUtility.ParseQueryString(string.Empty);
                queryParameters["sysparm_limit"] = "1";
                queryParameters["serial_number"] = id;

                var urlWithParams = $"{baseUrl}?{queryParameters}";

                var request = new HttpRequestMessage(HttpMethod.Get, urlWithParams);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await _authClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                dynamic data = JsonConvert.DeserializeObject<dynamic>(responseContent);
                if (data.result.Count > 0)
                {
                    string url = data.result[0].account.link;

                    var customerData = await GetCustomerById(url);

                    Device device = new Device
                    {
                        unit = data.result[0].sys_id.ToString(),
                        customerNumber = customerData.sys_id.ToString(),
                        address1 = customerData.street.ToString(),
                        city = customerData.city.ToString(),
                        state = customerData.state.ToString(),
                        zip = customerData.zip.ToString()
                    };

                    return device;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<ResponseData>> sendRequestToSNow(RequestData requestData)
        {
            try
            {
                var accessToken = await GetAccessTokenAsync();
                var baseUrl = $"{_configuration["servicenow:baseUrl"]}api/now/import/u_common_interface";
                var request = new HttpRequestMessage(HttpMethod.Post, baseUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                string jsonContent = JsonConvert.SerializeObject(requestData);

                request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _authClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                var responceContent = await response.Content.ReadAsStringAsync();

                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responceContent);
                var responseData = apiResponse?.result;

                return responseData;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public async Task<dynamic> GetCustomerById(string url)
        {
            try
            {
                var accessToken = await GetAccessTokenAsync();

                var request = new HttpRequestMessage(HttpMethod.Get, url);

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await _authClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                dynamic data = JsonConvert.DeserializeObject<dynamic>(responseContent);

                return data.result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

}
