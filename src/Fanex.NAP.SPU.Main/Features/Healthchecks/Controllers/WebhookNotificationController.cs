using Fanex.NAP.SPU.Main.Features.Healthchecks.Models;
using Microsoft.AspNetCore.Mvc;
using Sentry;
using Sentry.Protocol;
using System;
using System.Threading.Tasks;

namespace Fanex.NAP.SPU.Main.Features.Healthchecks.Controllers
{
    public class WebhookNotificationController : Controller
    {
        private readonly ISentryClient sentryClient;       

        public WebhookNotificationController(ISentryClient sentryClient)
        {
            this.sentryClient = sentryClient;           
        }

        [HttpPost]
        public IActionResult Send([FromBody] NotificationModel model)
        {
            //if (model != null)
            //{
            //   this.sentryClient.CaptureMessage(model.Message, model.ErrorCode == 0 ? SentryLevel.Info : SentryLevel.Error);
            //}

            return Ok();
        }
    }
}
