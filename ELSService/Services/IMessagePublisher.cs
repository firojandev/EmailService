using System;
namespace ELSService.Services
{
	public interface IMessagePublisher
	{
		Task<Boolean> sendMessage(string message);
	}
}

