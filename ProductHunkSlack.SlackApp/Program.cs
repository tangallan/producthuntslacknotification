using Newtonsoft.Json;
using Phunt.Api.Clients;
using Phunt.SlackLibrary.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Threading;
using Phunt.Api.Models;
using System.Diagnostics;

namespace ProductHunkSlack.SlackApp
{
    class Program
    {
        static ProductHuntPostModel productHuntModel;

        static void Main(string[] args)
        {
            int threadSleepMs = int.Parse(ConfigurationManager.AppSettings["SleepInMS"]);
            Stopwatch stopWatch = new Stopwatch();
            while (true)
            {
                Console.WriteLine("Running app at : " + DateTime.Now.ToString("MMMM dd, yyyy hh:mm:ss tt"));
                stopWatch.Reset();
                stopWatch.Start();

                var phuntAuthToken = ConfigurationManager.AppSettings["ProductHuntAuthToken"];

                ProductHuntClient phuntClient = new ProductHuntClient(phuntAuthToken);
                productHuntModel = phuntClient.GetPostsByDay().Result;

                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format("{0} Product Hunts \n", DateTime.Now.ToString("MMM dd, yyyy")));
                foreach (var p in productHuntModel.posts.OrderBy(o => o.created_at_datetime))
                {
                    sb.Append(string.Format("{0}: {1} - {2} \n <{3}|Discussion Url> <{4}|Product Url>\n {5} \n\n", p.id, p.name, p.tagline, p.discussion_url, p.redirect_url, p.created_at_datetime.ToString("MM-dd-yyyy hh:mm:ss tt")));
                }

                SlackWebHookClient client;
                Phunt.SlackLibrary.Models.SlackWebHookModel model = new Phunt.SlackLibrary.Models.SlackWebHookModel()
                {
                    Text = sb.ToString(),
                    Username = string.Format("phunt-slackfeed-{0}", DateTime.Now.Millisecond),
                    Icon_Url = "",
                    Icon_Emoji = ":bear:"
                };

                using (PhuntSlackFeedEntities db = new PhuntSlackFeedEntities())
                {
                    //gets all product hunt web hooks where today is not sent 
                    var webHookWhereIsNotSent = db.ProduntHuntWebHooks.Where(w => System.Data.Entity.DbFunctions.TruncateTime(w.SentDate) < System.Data.Entity.DbFunctions.TruncateTime(DateTime.UtcNow)).ToList();

                    //get all product hunt web hooks where we sent today but there were some added hunts (are there any new hunts for the day?)
                    var webHookWhereIsSent = db.ProduntHuntWebHooks.Where(w => System.Data.Entity.DbFunctions.TruncateTime(w.SentDate) == System.Data.Entity.DbFunctions.TruncateTime(DateTime.UtcNow) && productHuntModel.posts.Count > w.TotalProductHuntPosts).ToList();

                    //new day
                    foreach (var w in webHookWhereIsNotSent)
                    {
                        client = new SlackWebHookClient(w.SlackWebHook);
                        bool isSuccess = client.SendMessage(model).Result;

                        Console.WriteLine(w.SlackWebHook + " is success : " + isSuccess);

                        w.SentDate = DateTime.UtcNow;
                        w.TotalProductHuntPosts = productHuntModel.posts.Count;
                    }

                    //did we re
                    foreach (var w in webHookWhereIsSent)
                    {
                        model = new Phunt.SlackLibrary.Models.SlackWebHookModel()
                        {
                            Text = getNewHuntsSlackText(productHuntModel.posts.Count - w.TotalProductHuntPosts),
                            Username = string.Format("phunt-slackfeed-{0}", DateTime.Now.Millisecond),
                            Icon_Url = "",
                            Icon_Emoji = ":bear:"
                        };

                        client = new SlackWebHookClient(w.SlackWebHook);
                        bool isSuccess = client.SendMessage(model).Result;

                        Console.WriteLine(w.SlackWebHook + " is success : " + isSuccess);

                        w.SentDate = DateTime.UtcNow;
                        w.TotalProductHuntPosts = productHuntModel.posts.Count;
                    }

                    db.SaveChanges();

                    
                }

                stopWatch.Stop();
                Console.WriteLine("App complete in : " + stopWatch.ElapsedMilliseconds + " ms");
                Console.WriteLine("Sleeping...");
                Console.WriteLine("***********************************************************");
                Console.WriteLine("\n");
                Thread.Sleep(threadSleepMs);
            }
        }

        static string getNewHuntsSlackText(int difference)
        {
            var pHuntData = productHuntModel.posts.OrderBy(o => o.created_at_datetime).Skip(Math.Max(0, productHuntModel.posts.Count - difference)).ToList();
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("New Product Hunts Added For {0} \n", DateTime.Now.ToString("MMM dd, yyyy")));
            foreach (var p in pHuntData.OrderBy(o => o.created_at_datetime))
            {
                sb.Append(string.Format("{0}: {1} - {2} \n <{3}|Discussion Url> <{4}|Product Url>\n {5} \n\n", p.id, p.name, p.tagline, p.discussion_url, p.redirect_url, p.created_at_datetime.ToString("MM-dd-yyyy hh:mm:ss tt")));
            }

            return sb.ToString();
        }
    }

}
