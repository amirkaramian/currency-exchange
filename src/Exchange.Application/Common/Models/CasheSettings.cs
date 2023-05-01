using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exchange.Application.Common.Models;
public class CasheSettings
{
    public int TradeExpiryMinutes { get; set; }
    public int SymbolReadExpiryMinutes { get; set; }
    public int TradeCountPerHour { get; set; }
}
