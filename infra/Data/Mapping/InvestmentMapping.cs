using core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infra.Data.Mapping
{
    public class InvestmentMapping : IEntityTypeConfiguration<Investment>
    {
        public void Configure(EntityTypeBuilder<Investment> builder)
        {
            builder.ToTable("Investment");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OwnerId)
                   .IsRequired()
                   .HasColumnType("varchar(128)");

            builder.Property(x => x.CreationDate)
                   .IsRequired()
                   .HasColumnType("timestamptz");

            builder.Property(x => x.InitialAmount)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");


            builder.Property(x => x.WithdrawalDate)
                   .HasColumnType("timestamptz");


            builder.HasIndex(x => x.OwnerId);

        }
    }
}