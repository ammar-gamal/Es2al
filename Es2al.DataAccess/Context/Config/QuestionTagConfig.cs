using Es2al.Models.Entites;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Es2al.DataAccess.Context.Config
{
    public class QuestionTagConfig : IEntityTypeConfiguration<QuestionTag>
    {
        public void Configure(EntityTypeBuilder<QuestionTag> builder)
        {
            builder.HasKey(e => new { e.TagId, e.QuestionId});

            builder.HasOne(e => e.Tag)
                   .WithMany(e => e.Questions)
                   .HasForeignKey(e => e.TagId); 

            builder.HasOne(e => e.Question)
                   .WithMany(e => e.Tags)
                   .HasForeignKey(e => e.QuestionId);
        }
    }
}
