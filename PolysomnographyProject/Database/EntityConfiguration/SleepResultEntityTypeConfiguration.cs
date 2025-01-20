namespace PolysomnographyProject.Database.EntityConfiguration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Business.Sleep;
using Models.Helping;

public class SleepResultEntityTypeConfiguration : IEntityTypeConfiguration<SleepResult>
{
    public void Configure(EntityTypeBuilder<SleepResult> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever();

        builder.OwnsOne<SleepResultData>(r => r.Data, sleepResultData =>
        {
            sleepResultData.Property(d => d.HF).IsRequired();

            sleepResultData.Property(d => d.LF).IsRequired();

            sleepResultData.Property(d => d.SDNN).IsRequired();
        });
    }
}