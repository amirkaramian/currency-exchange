using Exchange.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Exchange.Infrastructure.Persistence.Configurations;
public class CurrencyConfiguration : IEntityTypeConfiguration<Domain.Entities.Currency>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Currency> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.Code).IsUnique(true);
    }
}
