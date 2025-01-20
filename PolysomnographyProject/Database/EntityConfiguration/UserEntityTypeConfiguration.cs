namespace PolysomnographyProject.Database.EntityConfiguration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;
using Models.Business;
using Models.Business.Sleep;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedNever();

        builder.OwnsOne(u => u.PersonalSleepData, personalSleepData =>
        {
            personalSleepData.OwnsOne(d => d.SleepTimePreferences, sleepTimePreferences =>
            {
                sleepTimePreferences.Property(p => p.StartTime)
                                    .IsRequired();

                sleepTimePreferences.Property(p => p.EndTime)
                                    .IsRequired();
            });
        });
        
        builder.Property(u => u.UniqueLogin).IsRequired();

        builder.HasMany(u => u.SleepResults)
               .WithOne(u => u.User);
        
        builder.OwnsOne(u => u.TelegramUserData, telegramUserData =>
        {
            telegramUserData.Property(u => u.TelegramId)
                            .IsRequired();
            
            telegramUserData.Property(u => u.TelegramUsername)
                            .IsRequired();
        });
    }
}