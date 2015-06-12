using Newtonsoft.Json;
using Phunt.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Phunt.Api.Clients
{
    public class ProductHuntClient
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _grantType;

        private readonly string _authToken;

        public ProductHuntClient(string clientId, string clientSecret, string grantType)
        {
            this._clientId = clientId;
            this._clientSecret = clientSecret;
            this._grantType = grantType;
        }

        public ProductHuntClient(string authToken)
        {
            this._authToken = authToken;
        }

        /// <summary>
        /// get all posts
        /// </summary>
        /// <param name="searchUrl"></param>
        /// <param name="older"></param>
        /// <param name="newer"></param>
        /// <param name="perPage"></param>
        /// <returns></returns>
        public async Task<ProductHuntPostModel> GetAllPosts(string searchUrl = "", int? older = null, int? newer = null, int perPage = 50)
        {
            try
            {
                HttpClient client = await getProductHuntHttpClient();

                string uriparams = this.buildProductHuntGetAllParams(searchUrl, older, newer, perPage);
                string uri = string.Format("https://api.producthunt.com/v1/posts/all?{0}", uriparams);
                var response = await client.GetAsync(uri, HttpCompletionOption.ResponseContentRead);
                var responseStr = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<ProductHuntPostModel>(responseStr);
                client.Dispose();

                return data;
            }
            catch (Exception e)
            {
                Console.WriteLine("Product Hunt Client Exception:");
                Console.WriteLine(e.Message);
                return new ProductHuntPostModel() { posts = new List<ProductHuntPost>() };
            }
        }

        /// <summary>
        /// get posts by day, if no day is even, it will use DateTime.now as default
        /// </summary>
        /// <param name="day"></param>
        public async Task<ProductHuntPostModel> GetPostsByDay(DateTime? day = null)
        {
            day = day ?? DateTime.Now;

            try
            {
                HttpClient client = await getProductHuntHttpClient();

                string uri = string.Format("https://api.producthunt.com/v1/posts?day={0}", day.Value.ToString("yyyy-MM-dd"));
                var response = await client.GetAsync(uri, HttpCompletionOption.ResponseContentRead);
                var responseStr = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<ProductHuntPostModel>(responseStr);
                client.Dispose();

                return data;
            }
            catch (Exception e)
            {
                Console.WriteLine("Product Hunt Client Exception:");
                Console.WriteLine(e.Message);
                return new ProductHuntPostModel() { posts = new List<ProductHuntPost>() };
            }
        }

        /// <summary>
        /// get posts from days ago
        /// </summary>
        /// <param name="daysAgo"></param>
        /// <returns></returns>
        public async Task<ProductHuntPostModel> GetPostsDaysAgo(int daysAgo)
        {
            try
            {
                HttpClient client = await getProductHuntHttpClient();
                string uri = string.Format("https://api.producthunt.com/v1/posts?days_ago={0}", daysAgo);
                var response = await client.GetAsync(uri, HttpCompletionOption.ResponseContentRead);
                var responseStr = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<ProductHuntPostModel>(responseStr);
                client.Dispose();

                return data;
            }
            catch (Exception e)
            {
                Console.WriteLine("Product Hunt Client Exception:");
                Console.WriteLine(e.Message);
                return new ProductHuntPostModel() { posts = new List<ProductHuntPost>() };
            }
        }

        private async Task<string> getAuthorizationToken()
        {
            if (!string.IsNullOrEmpty(this._authToken))
                return _authToken;
            else
            {
                if (string.IsNullOrEmpty(this._clientId) && string.IsNullOrEmpty(this._clientSecret) && string.IsNullOrEmpty(this._authToken))
                    throw new MissingFieldException("Client Key and Client Secret or Auth Token is needed");

                try
                {
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Add("Host", "api.producthunt.com");
                    client.DefaultRequestHeaders.Add("User-Agent", "Product Hunt Slack App");
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    string content = JsonConvert.SerializeObject(new
                    {
                        client_id = this._clientId,
                        client_secret = this._clientSecret,
                        grant_type = this._grantType
                    });

                    var response = await client.PostAsync("https://api.producthunt.com/v1/oauth/token", new StringContent(content, Encoding.UTF8, "application/json"));
                    var responseStr = await response.Content.ReadAsStringAsync();

                    var obj = JsonConvert.DeserializeObject<ProductHuntAuthModel>(responseStr);
                    if (obj != null)
                    {
                        return obj.access_token;
                    }
                    else
                    {
                        //throw new JsonSerializationException("Invalid JSON to Deserialize into Object");
                        Console.WriteLine("Invalid JSON to Deserialize into Object");
                        return "";
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Product Hunt Client Exception:");
                    Console.WriteLine(e.Message);
                    return "";
                }
            }
        }
        private async Task<HttpClient> getProductHuntHttpClient()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Host", "api.producthunt.com");
            client.DefaultRequestHeaders.Add("User-Agent", "Product Hunt Slack App");
            client.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", await this.getAuthorizationToken()));
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }
        private string buildProductHuntGetAllParams(string searchUrl, int? older, int? newer, int perPage)
        {
            StringBuilder sbparams = new StringBuilder();
            if (!string.IsNullOrEmpty(searchUrl))
            {
                sbparams.Append("search[url]=" + HttpUtility.UrlEncode(searchUrl) + "&");
            }

            if (older.HasValue)
            {
                sbparams.Append("older" + older.Value + "&");
            }

            if(newer.HasValue)
            {
                sbparams.Append("newer" + newer.Value + "&");
            }

            sbparams.Append("per_page=" + perPage);

            return sbparams.ToString();
        }
    }
}
