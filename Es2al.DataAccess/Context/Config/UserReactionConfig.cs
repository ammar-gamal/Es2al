using Es2al.Models.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Es2al.DataAccess.Context.Config
{
    public class UserReactionConfig : IEntityTypeConfiguration<UserReaction>
    {
        public void Configure(EntityTypeBuilder<UserReaction> builder)
        {
            builder.HasKey(e => new { e.UserId, e.AnswerId });

            builder.Property(e => e.React)
                   .HasColumnType("tinyint");

            builder.HasOne(e => e.User)
                   .WithMany(e => e.Reactions)
                   .HasForeignKey(e => e.UserId);

            builder.HasOne(e => e.Answer)
                   .WithMany(e => e.Reactions)
                   .HasForeignKey(e => e.AnswerId);
                    
        }
    }
}
