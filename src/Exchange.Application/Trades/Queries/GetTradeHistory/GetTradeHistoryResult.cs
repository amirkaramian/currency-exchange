using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Exchange.Application.Common.Mappings;
using Exchange.Domain.Entities;
using Exchange.Domain.Enums;

namespace Exchange.Application.Trades.Queries.GetTradeHistory;
public class GetTradeHistoryResult : IMapFrom<Transaction>
{
    public DateTime Created { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public string SourceCurrency { get; set; }
    public string DestionationCurrency { get; set; }
    public decimal Rate { get; set; }
    public decimal ExchangeAmountResult { get; set; }
    public DateTime DateRate { get; set; }
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Transaction, GetTradeHistoryResult>()
            .ForMember(d => d.SourceCurrency, opt => opt.MapFrom(s => s.SourceCurrency.Symbol))
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.ToString()));
    }
}
