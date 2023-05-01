using Exchange.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Exchange.Infrastructure.Persistence.Configurations;
public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Amount)
         .HasColumnType("decimal(18,2)");

        builder.HasOne(x => x.UserAccount).WithMany().HasForeignKey(x => x.UserAccountId);

        builder.HasOne(x => x.SourceCurrency).WithMany().HasForeignKey(x => x.SourceCurrencyId);
    }
}
