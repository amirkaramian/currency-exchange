using Exchange.Domain.Events;
 

namespace Exchange.RabbitMQBus.Bus
{
    public interface IEventBus
    {
        void Publish<T>(T @event) where T : IntegrationEvent;
        void Subscribe<T, TH>() where T : IntegrationEvent where TH : IEventHandler<T>;
    }
}
