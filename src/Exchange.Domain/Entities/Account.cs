using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Exchange.Domain.Entities;
public class UserAccount : BaseAuditableEntity
{
    public Guid UserAccountGuid { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    [AllowNull]
    public string PhoneNumber { get; set; }
    [AllowNull]
    public string LogoFileName { get; set; }
    [AllowNull]
    public Address Address { get; set; }
}
