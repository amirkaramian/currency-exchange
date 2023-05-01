using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exchange.Application.Trades.EventHandlers;
public class TradeHistoryCommand
{
    public Guid UserAccount { get; set; }
    public int Count { get; set; }
    public DateTime TimeTrade { get; set; }
    public DateTime StartTimeTrade { get; set; }
}
