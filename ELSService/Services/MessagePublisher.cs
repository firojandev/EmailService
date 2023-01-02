using System;
using System.Text;
using System.Threading.Channels;
using RabbitMQ.Client;

namespace ELSService.Services
{
    public class MessagePublisher : IMessagePublisher
    {
        private IConnection _connection;
        private IModel _channel;

        public MessagePublisher()
        {
            InitRabbitMQ();
        }

        private void InitRabbitMQ()
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri("")
            };

            // create a connection and open a channel, dispose them when done
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
      
        }



        public async Task<bool> sendMessage(string message)
        {
            // ensure that the queue exists before we publish to it
            var queueName = "els.queue.emails";
            //bool durable = false;
            //bool exclusive = false;
            //bool autoDelete = true;

            //_channel.QueueDeclare(queueName, durable, exclusive, autoDelete, null);

            // the data put on the queue must be a byte array
            var data = Encoding.UTF8.GetBytes(message);
            // publish to the "default exchange", with the queue name as the routing key
            var exchangeName = "";
            var routingKey = queueName;
            _channel.BasicPublish(exchangeName, routingKey, null, data);

            return true;
        }
    }
}

