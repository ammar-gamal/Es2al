
using Es2al.Models.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Es2al.DataAccess.Context.Config
{
    public class UserTagConfig : IEntityTypeConfiguration<UserTag>
    {
        public void Configure(EntityTypeBuilder<UserTag> builder)
        {
            builder.HasKey(e => new { e.TagId ,e.UserId });

            builder.HasOne(e => e.Tag)
                 .WithMany(e => e.Users)
                 .HasForeignKey(e => e.TagId);

            builder.HasOne(e => e.User)
                   .WithMany(e => e.Tags)
                   .HasForeignKey(e => e.UserId);
        }
    }
}
