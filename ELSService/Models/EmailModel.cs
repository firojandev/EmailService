using System;
using ELSService.Emailing;

namespace ELSService.Models
{
	public class EmailModel
	{
	    public List<EmailAddress> SenderEmailsList { set; get; }
        public List<EmailAddress> ReceiverEmailsList { set; get; }
		public string Subject { set; get; }
        public string Body { set; get; }

    }
}

