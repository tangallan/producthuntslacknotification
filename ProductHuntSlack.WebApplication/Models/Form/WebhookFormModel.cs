using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProductHuntSlack.WebApplication.Models
{
    public class WebhookFormModel : IValidatableObject
    {
        [Required]
        [Url]
        [Display(Name="Slack Webhook")]
        public string WebHookUrl { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> result = new List<ValidationResult>();

            if (!WebHookUrl.Contains("hooks.slack.com/services"))
            {
                result.Add(new ValidationResult("Please enter a VALID Slack Integration Link"));
            }

            return result;
        }
    }
}