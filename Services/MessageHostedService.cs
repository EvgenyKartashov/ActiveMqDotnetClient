using Apache.NMS;
using Apache.NMS.Util;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Options;
using ActiveMqClient.Options;

namespace ActiveMqClient.Services
{
    public class MessageHostedService : IHostedService
    {
        private readonly ILogger<MessageHostedService> _logger;
        private readonly Queue1Config _queue1Config;
        private readonly Queue2Config _queue2Config;
        private readonly IConnection _connectionQ1;
        private readonly IConnection _connectionQ2;

        private ISession _consumerSession;
        private ISession _producerSession;

        public MessageHostedService(ILogger<MessageHostedService> logger, IOptions<Queue1Config> queue1Config, IOptions<Queue2Config> queue2Config)
        {
            _logger = logger;

            _queue1Config = queue1Config.Value;
            _queue2Config = queue2Config.Value;

            var factory1 = new NMSConnectionFactory(new Uri($"amqp://{_queue1Config.Host}:{_queue1Config.Port}"));
            _connectionQ1 = factory1.CreateConnection(_queue1Config.User, _queue1Config.Password);
            _connectionQ1.Start();

            var factory2 = new NMSConnectionFactory(new Uri($"amqp://{_queue2Config.Host}:{_queue2Config.Port}"));
            _connectionQ2 = factory2.CreateConnection(_queue2Config.User, _queue2Config.Password);
            _connectionQ2.Start();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _consumerSession = _connectionQ1.CreateSession(AcknowledgementMode.AutoAcknowledge);

            var consumerDestination = SessionUtil.GetDestination(_consumerSession, $"queue://{_queue1Config.IncomingQueue}");
            var consumer = _consumerSession.CreateConsumer(consumerDestination);
            consumer.Listener += Consumer_Listener;

            SendMessagesAsync();

            return Task.CompletedTask;
        }

        private async void SendMessagesAsync()
        {
            _producerSession = _connectionQ2.CreateSession(AcknowledgementMode.AutoAcknowledge);

            var producerDestination = SessionUtil.GetDestination(_producerSession, $"queue://{_queue2Config.OutgoingQueue}");
            var producer = _producerSession.CreateProducer(producerDestination);

            var credentials = _queue2Config.Credentials;

            await Task.Delay(10000);

            var msg1 = _producerSession.CreateMessage();
            msg1.Properties.SetString("messageId", "123");
            msg1.Properties.SetString("user", credentials.Login);
            msg1.Properties.SetString("password", credentials.Password);
            msg1.Properties.SetString("messageInfo", "msg1");

            producer.Send(msg1);
            await Task.Delay(2000);

            var msg2 = _producerSession.CreateMessage();
            msg2.Properties.SetString("messageId", "456");
            msg2.Properties.SetString("user", credentials.Login);
            msg2.Properties.SetString("password", credentials.Password);
            msg2.Properties.SetString("messageInfo", "msg2");

            producer.Send(msg2);
            await Task.Delay(2000);

            var msg3 = _producerSession.CreateMessage();
            msg3.Properties.SetString("messageId", "789");
            msg3.Properties.SetString("user", credentials.Login);
            msg3.Properties.SetString("password", credentials.Password);
            msg3.Properties.SetString("messageInfo", "msg3");

            producer.Send(msg3);
            await Task.Delay(2000);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _consumerSession.Close();
            _consumerSession.Close();

            _producerSession.Close();
            _producerSession.Close();

            await Task.CompletedTask;
        }


        private void Consumer_Listener(IMessage message)
        {
            if(message == null)
            {
                _logger.LogWarning("No message received!");
                return;
            }

            _logger.LogInformation($"Recieved messageId: {message.Properties.GetString("messageId")}");
            _logger.LogInformation($"Recieved messageInfo: {message.Properties.GetString("messageInfo")}");
        }
    }
}
