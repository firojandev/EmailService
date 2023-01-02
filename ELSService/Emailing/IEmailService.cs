using System;
using System.Collections.Generic;

namespace ELSService.Emailing
{
    public interface IEmailService
    {
        Task<bool> Send(EmailMessage emailMessage);
        List<EmailMessage> ReceiveEmail(int maxCount = 10);
    }
}
