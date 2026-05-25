namespace NairaLedger.Infrastructure.Persistence.Configurations;

internal class IdempotencyRecordConfiguration : IEntityTypeConfiguration<IdempotencyRecord>
{
    public void Configure(EntityTypeBuilder<IdempotencyRecord> builder)
    {
        builder.ToTable("IdempotencyRecords");

        builder.HasKey(r => r.Key);
        builder.Property(r => r.Key).HasMaxLength(128);

        builder.Property(r => r.ResponseData).IsRequired().HasColumnType("jsonb");
        builder.Property(r => r.CreatedAt).IsRequired();
    }
}