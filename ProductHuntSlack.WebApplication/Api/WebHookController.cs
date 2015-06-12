using Phunt.Api.Clients;
using Phunt.Api.Models;
using Phunt.SlackLibrary.Clients;
using ProductHuntSlack.WebApplication.Data;
using ProductHuntSlack.WebApplication.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ProductHuntSlack.WebApplication.Api
{
    public class WebHookController : ApiController
    {
        private SlackWebHookClient _slackWebHookClient;
        private ProductHuntClient _phuntClient;
        [HttpPost]
        public async Task<IHttpActionResult> AddNew([FromBody]WebhookFormModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (PhuntSlackFeedEntities db = new PhuntSlackFeedEntities())
                    {
                        var d = db.ProduntHuntWebHooks.FirstOrDefault(f => f.SlackWebHook.Equals(model.WebHookUrl));

                        if (d != null)
                        {
                            return BadRequest("Slack Webhook already exists");
                        }
                    }

                    _phuntClient = new ProductHuntClient(ConfigurationManager.AppSettings["ProductHuntAuthToken"]);
                    ProductHuntPostModel productHuntModel = await _phuntClient.GetPostsByDay();
                    StringBuilder sb = new StringBuilder();
                    sb.Append("Welcome to the Product Hunt, Slack Nodification, thank you for joining, you will now be receiving updates from Product Hunt \n");
                    sb.Append("Here are the Products Hunt for today: \n\n");

                    foreach (var p in productHuntModel.posts.OrderBy(o => o.created_at_datetime))
                    {
                        sb.Append(string.Format("{0}: {1} - {2} \n <{3}|Discussion Url> <{4}|Product Url>\n {5} \n\n", p.id, p.name, p.tagline, p.discussion_url, p.redirect_url, p.created_at_datetime.ToString("MM-dd-yyyy hh:mm:ss tt")));
                    }
                    Phunt.SlackLibrary.Models.SlackWebHookModel slackWebHookModel = new Phunt.SlackLibrary.Models.SlackWebHookModel()
                    {
                        Text = sb.ToString(),
                        Username = string.Format("phunt-slackfeed-{0}", DateTime.Now.Millisecond),
                        Icon_Url = "",
                        Icon_Emoji = ":bear:"
                    };
                    _slackWebHookClient = new SlackWebHookClient(model.WebHookUrl);
                    await _slackWebHookClient.SendMessage(slackWebHookModel);

                    using (PhuntSlackFeedEntities db = new PhuntSlackFeedEntities())
                    {
                        db.ProduntHuntWebHooks.Add(new ProduntHuntWebHook()
                        {
                            SlackWebHook = model.WebHookUrl,
                            TotalProductHuntPosts = productHuntModel.posts.Count,
                            SentDate = DateTime.UtcNow
                        });
                        db.SaveChanges();
                    }

                    return Ok("Add successful");
                }
                catch (Exception e)
                {
                    return BadRequest("Internal Server Error! Try again later");
                }
            }

            return BadRequest(ModelState);
        }
    }
}
