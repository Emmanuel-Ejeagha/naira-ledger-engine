namespace NairaLedger.Infrastructure.Persistence.Configurations;

internal class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Reference)
            .HasConversion(r => r.Value, s => new TransactionReference(s))
            .IsRequired()
            .HasMaxLength(24);

        builder.HasIndex(t => t.Reference).IsUnique();

        builder.Property(t => t.Type)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(t => t.CreatedAt).IsRequired();

        builder.Property(t => t.InitiatedByUserId);

        builder.HasMany(t => t.Entries)
            .WithOne()
            .HasForeignKey("TransactionId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.Metadata.FindNavigation(nameof(Transaction.Entries))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(t => t.DomainEvents);
    }
}