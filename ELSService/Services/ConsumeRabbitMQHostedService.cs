using System;
using System.Text;
using System.Text.Json;
using ELSService.Emailing;
using ELSService.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ELSService.Services
{
	public class ConsumeRabbitMQHostedService: BackgroundService
	{
        private IConnection _connection;
        private IModel _channel;
        private readonly IEmailService _emailService;

        public ConsumeRabbitMQHostedService(ILoggerFactory loggerFactory, IEmailService emailService)
        {
            InitRabbitMQ();
            _emailService = emailService;
        }

        private void InitRabbitMQ()
        {
            var factory = new ConnectionFactory {
                Uri = new Uri("")
            };

            // create connection  
            _connection = factory.CreateConnection();

            // create channel  
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare("demo.exchange", ExchangeType.Topic);
            _channel.QueueDeclare("els.queue.emails", false, false, false, null);
            _channel.QueueBind("demo.queue.log", "demo.exchange", "demo.queue.*", null);
            _channel.BasicQos(0, 1, false);

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                // received message  
               // var content = System.Text.Encoding.UTF8.GetString(ea.Body.ToArray());
                string message = Encoding.UTF8.GetString(ea.Body.ToArray());

                // handle the received message  
                HandleMessage(ea.RoutingKey, message);
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;//13499 , intern sample is not showing

            _channel.BasicConsume("els.queue.emails", false, consumer);

            return Task.CompletedTask;
        }

        private async Task<Boolean> HandleMessage(string RoutingKey, string content)
        {
            // we just print this message   
            Console.WriteLine($"consumer received {content}");

            switch (RoutingKey)
            {
                case "els.queue.emails":

                    try
                    {
                        EmailModel emailModel = JsonSerializer.Deserialize<EmailModel>(content)!;

                        EmailMessage emailMessage = new EmailMessage();
                        emailMessage.Subject = emailModel.Subject;
                        emailMessage.Content = emailModel.Body;

                        emailMessage.FromAddresses = emailModel.SenderEmailsList;
                        emailMessage.ToAddresses = emailModel.ReceiverEmailsList;

                        await _emailService.Send(emailMessage);
                    }
                    catch(Exception e)
                    {
                        
                    }
                    break;

                default:
                    break;

            }
            return true;

        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }

    }
}

