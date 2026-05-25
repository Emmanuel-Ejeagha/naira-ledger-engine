namespace NairaLedger.Infrastructure.Persistence.Configurations;

internal class LedgerEntryConfiguration : IEntityTypeConfiguration<LedgerEntry>
{
    public void Configure(EntityTypeBuilder<LedgerEntry> builder)
    {
        builder.ToTable("LedgerEntries");

        builder.HasKey(le => le.Id);

        builder.Property(le => le.WalletId).IsRequired();
        builder.Property(le => le.Amount).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(le => le.Direction)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(10);
        builder.Property(le => le.Description).IsRequired().HasMaxLength(500);
        builder.Property(le => le.CreatedAt).IsRequired();
    }
}