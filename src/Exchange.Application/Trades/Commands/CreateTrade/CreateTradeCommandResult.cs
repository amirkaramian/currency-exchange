using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Exchange.Application.Trades.Commands.CreateTrade;
public record CreateTradeCommandResult
{
    public DateTime date { get; set; }
    public Info info { get; set; }
    public Query query { get; set; }
    public decimal result { get; set; }
    public bool success { get; set; }
}
public record Info
{
    public decimal rate { get; set; }
    public long timestamp { get; set; }
}

public record Query
{
    public decimal amount { get; set; }
    public string from { get; set; }
    public string to { get; set; }
}