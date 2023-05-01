using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exchange.Domain.Entities;
public class Transaction : BaseAuditableEntity
{
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public TransactionType Type { get; set; }
    public Guid SourceCurrencyId { get; set; }
    public Currency SourceCurrency { get; set; }
    public Guid DestinationCurrencyId { get; set; }
    public string DestionationCurrency { get; set; }
    public Guid UserAccountId { get; set; }
    public UserAccount UserAccount { get; set; }
    public decimal Rate { get; set; }
    public decimal ExchangeAmountResult { get; set; }
    public DateTime DateRate { get; set; }
}
