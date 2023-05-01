using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exchange.Domain.Entities;
public class Tenant
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; }
    public string ContactPhone { get; set; }
    public string ContactEmail { get; set; }
    public string DomainName { get; set; }


    public DateTime CreateUtc { get; set; }
    public DateTime UpdateUtc { get; set; }


    public Tenant()
    {
        Id = Guid.NewGuid();

        var utcNow = DateTime.UtcNow;
        CreateUtc = utcNow;
        UpdateUtc = utcNow;
    }
}
