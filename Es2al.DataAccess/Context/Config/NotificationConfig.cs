using Es2al.DataAccess.Context.Config.AbstractConfig;
using Es2al.Models.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Es2al.DataAccess.Context.Config
{
    public class NotificationConfig : BaseTextConfig<Notification>
    {
        public override void Configure(EntityTypeBuilder<Notification> builder)
        {
            base.Configure(builder);

            builder.HasOne(e => e.User)
                   .WithMany(e => e.Notifications)
                   .HasForeignKey(e => e.UserId);
            builder.Property(e => e.IsMarkedAsReed)
                   .HasDefaultValue(false);
        }
    }
}
