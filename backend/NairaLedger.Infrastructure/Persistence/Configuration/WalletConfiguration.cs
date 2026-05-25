namespace NairaLedger.Infrastructure.Persistence.Configurations;

internal class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.ToTable("Wallets");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.UserId)
            .HasConversion(id => id.Value, guid => new UserId(guid))
            .IsRequired();

        builder.Property(w => w.Tag)
            .HasConversion(tag => tag != null ? tag.Value : null, val => val != null ? new WalletTag(val) : null)
            .HasMaxLength(50);

        builder.Property(w => w.KycLevel)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(w => w.IsActive).IsRequired();

        builder.Property(w => w.CreatedAt).IsRequired();

        builder.Property(w => w.Version)
            .IsConcurrencyToken()
            .ValueGeneratedNever();

        builder.Ignore(w => w.DomainEvents);
    }
}