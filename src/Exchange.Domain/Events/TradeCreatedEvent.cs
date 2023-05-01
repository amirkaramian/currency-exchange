using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exchange.Domain.Events;
public class TradeCreatedEvent : BaseEvent
{
    public TradeCreatedEvent(Transaction item)
    {
        Item = item;
    }

    public Transaction Item { get; }
}
