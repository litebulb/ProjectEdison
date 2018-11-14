using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edison.Tests.Microservices
{
    [TestFixture]
    public class PingPongMessageTestFixture : InMemoryTestFixture
    {
        private PongConsumer _pongConsumer;
        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _received = Handled<IPongMessage>(configurator);
        }

        protected override void PreCreateBus(IInMemoryBusFactoryConfigurator configurator)
        {
            var _pingConsumer = new PingConsumer();
            _pongConsumer = new PongConsumer();
            configurator.ReceiveEndpoint("test_ping_queue", e =>
            {
                e.Consumer(() => _pingConsumer);
            });

            configurator.ReceiveEndpoint("test_pong_queue", e =>
            {
                e.Consumer(() => _pongConsumer);
            });
        }

        Task<ConsumeContext<IPongMessage>> _received;

        [Test]
        public async Task test_how_to_test_consumers()
        {
            await Bus.Publish<IPingMessage>(new { MessageId = 100 });
            await _received;

            Assert.IsTrue(_pongConsumer.hitme);
            Assert.AreEqual(100, _pongConsumer.pongMessage.MessageId);
        }

        public class PingConsumer : IConsumer<IPingMessage>
        {
            public Task Consume(ConsumeContext<IPingMessage> context)
            {
                context.Publish<IPongMessage>(new { context.Message.MessageId });
                return Task.CompletedTask;
            }
        }

        public class PongConsumer : IConsumer<IPongMessage>
        {
            internal bool hitme;
            internal IPongMessage pongMessage;
            public Task Consume(ConsumeContext<IPongMessage> context)
            {
                hitme = true;
                pongMessage = context.Message;
                return Task.CompletedTask;
            }
        }

        public interface IPingMessage
        {
            int MessageId { get; set; }
        }

        public interface IPongMessage
        {
            int MessageId { get; set; }
        }
    }
}
