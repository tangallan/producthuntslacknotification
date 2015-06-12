using Phunt.SlackLibrary.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phunt.SlackLibrary.Models
{
    public class SlackWebHookModel
    {
        [SlackWebHookAttribute("text")]
        public string Text { get; set; }

        [SlackWebHookAttribute("username")]
        public string Username { get; set; }

        [SlackWebHookAttribute("icon_url")]
        public string Icon_Url { get; set; }

        [SlackWebHookAttribute("icon_emoji")]
        public string Icon_Emoji { get; set; }
    }
}
