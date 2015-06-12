using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phunt.SlackLibrary.Attributes
{
    public class SlackWebHookAttribute : Attribute
    {
        private readonly string _slackParamName;
        public SlackWebHookAttribute(string slackParamName)
        {
            this._slackParamName = slackParamName;
        }

        public string GetSlackParamName()
        {
            return this._slackParamName;
        }
    }
}
