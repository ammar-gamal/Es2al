using Es2al.DataAccess.Context.Config.AbstractConfig;
using Es2al.Models.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Es2al.DataAccess.Context.Config
{
    public class AnswerConfig : BaseTextConfig<Answer>
    {
        public override void Configure(EntityTypeBuilder<Answer> builder)
        {
            base.Configure(builder);

            builder.HasOne(e => e.User)
                   .WithMany(e => e.Answers)
                   .HasForeignKey(e => e.UserId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(e => e.Question)
                   .WithOne(e => e.Answer)
                   .HasForeignKey<Answer>(e=>e.QuestionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
