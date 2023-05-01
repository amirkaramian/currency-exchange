using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exchange.Application.Currencies.Queries.GetCurrencySymbols;
public record CurrencySymbols
{
    public bool success { get; set; }
    public Dictionary<string, string> Symbols { get; set; }
    public List<CurrencyMap> CurrencyMap { get; set; }
}

public record CurrencyMap
{
    public CurrencyMap(string key, string value, Guid id)
    {
        Code = key;
        Descriptin = value;
        Id = id;    
    }
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string Descriptin { get; set; }
}