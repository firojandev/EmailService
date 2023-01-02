using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELSService.Emailing;
using ELSService.Models;
using ELSService.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ELSService.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class MsgPublisherController : Controller
    {
        private IMessagePublisher _iMessagePublisher;

        private readonly IEmailService _emailService;

        public MsgPublisherController(IMessagePublisher iMessagePublisher, IEmailService emailService)
        {
            _iMessagePublisher = iMessagePublisher;
            _emailService = emailService;
        }

        [Route("[action]")]
        [HttpPost]
        [Consumes("application/json")]
        public async Task<JsonResult> SendMessage(EmailModel emailModel)
        {

            //var result = await _iMessagePublisher.sendMessage(messageModel.msg);
            //List<EmailAddress> FromEmailsList = new List<EmailAddress>();
            //List<EmailAddress> ToEmailsList = new List<EmailAddress>();

            //EmailAddress EmailFrom = new EmailAddress();
            //EmailFrom.Name = "ELS";
            //EmailFrom.Address = "demo@onlineicttutor.com";
            //FromEmailsList.Add(EmailFrom);

          
            //EmailAddress EmailTo = new EmailAddress();
            //EmailTo.Name = "Adiba";
            //EmailTo.Address = "silbd.andev@gmail.com";
            //ToEmailsList.Add(EmailTo);

            EmailMessage emailMessage = new EmailMessage();
            emailMessage.Subject = emailModel.Subject;
            emailMessage.Content = emailModel.Body;

            emailMessage.FromAddresses = emailModel.SenderEmailsList;
            emailMessage.ToAddresses = emailModel.ReceiverEmailsList;
         
           await _emailService.Send(emailMessage);


            return Json("1");
        }

    }
}

