using Newtonsoft.Json;
using Phunt.SlackLibrary.Attributes;
using Phunt.SlackLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Phunt.SlackLibrary.Clients
{
    public class SlackWebHookClient
    {
        private readonly string _webHookUrl;

        public SlackWebHookClient(string webHookUrl)
        {
            this._webHookUrl = webHookUrl;
        }

        public async Task<bool> SendMessage(SlackWebHookModel webHook)
        {
            try
            {
                var propsAndValues = getSlackPropertiesAndValues(webHook);
                string jsonResult = JsonConvert.SerializeObject(propsAndValues);

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Host", "hooks.slack.com");
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string,string>("payload", jsonResult)
                });
                var response = await client.PostAsync(_webHookUrl, content);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Slack Web Hoot Client Exception:");
                Console.WriteLine(e.Message);
                return false;
            }
        }

        private Dictionary<string,string> getSlackPropertiesAndValues(object obj)
        {
            Dictionary<string, string> _dict = new Dictionary<string, string>();

            PropertyInfo[] props = obj.GetType().GetProperties();
            foreach (PropertyInfo prop in props)
            {
                object[] attrs = prop.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    SlackWebHookAttribute authAttr = attr as SlackWebHookAttribute;
                    if (authAttr != null)
                    {
                        string propName = prop.GetValue(obj).ToString();
                        string auth = authAttr.GetSlackParamName();

                        _dict.Add(auth, propName);
                    }
                }
            }

            return _dict;
        }
    }
}
