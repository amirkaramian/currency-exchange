using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exchange.Domain.Events;
public class RegisterClientEvent : IntegrationEvent
{
    public Guid UserAccountGuid { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public Guid TenantId { get; set; }
    public RegisterClientEvent(Guid tenantId) : base(tenantId)
    {
        TenantId = tenantId;
    }
}
