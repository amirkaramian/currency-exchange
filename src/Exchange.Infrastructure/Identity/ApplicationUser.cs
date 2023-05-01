using Microsoft.AspNetCore.Identity;

namespace Exchange.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
