 
using System.Threading.Tasks;
using Exchange.Domain.Events;

namespace Exchange.RabbitMQBus.Bus
{
    public interface IEventHandler<in TEvent> : IEventHandler where TEvent : IntegrationEvent
    {
        Task Handle(TEvent @event);
       
    }

    public interface IEventHandler
    {

    }
}
