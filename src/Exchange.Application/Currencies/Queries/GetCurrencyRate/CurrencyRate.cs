using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exchange.Application.Currencies.Queries.GetCurrencyRate;
public record CurrencyRate
{
    public bool success { get; set; }
    public string @base { get; set; }
    public string date { get; set; }
    public Dictionary<string, decimal> rates { get; set; }
    public int timestamp { get; set; }
}
