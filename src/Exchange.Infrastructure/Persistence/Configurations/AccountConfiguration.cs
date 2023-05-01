 
using Exchange.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Exchange.Infrastructure.Persistence.Configurations;
internal class AccountConfiguration : IEntityTypeConfiguration<UserAccount>
{
    public void Configure(EntityTypeBuilder<UserAccount> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.UserAccountGuid).IsUnique(true);

        builder.OwnsOne(x => x.Address);
    }
}
