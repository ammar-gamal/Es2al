using Es2al.Models.Entites;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Es2al.DataAccess.Context.Config
{
    public class UserFollowConfig : IEntityTypeConfiguration<UserFollow>
    {
        public void Configure(EntityTypeBuilder<UserFollow> builder)
        {
            builder.HasKey(e => new { e.FollowerId, e.FollowingId });

            builder.HasOne(e => e.Follower)
                   .WithMany(e => e.Followings)
                   .HasForeignKey(e=>e.FollowerId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(e => e.Following)
                   .WithMany(e => e.Followers)
                   .HasForeignKey(e => e.FollowingId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
