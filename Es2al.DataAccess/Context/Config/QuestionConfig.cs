using Es2al.DataAccess.Context.Config.AbstractConfig;
using Es2al.Models.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Es2al.DataAccess.Context.Config
{
    public class QuestionConfig : BaseTextConfig<Question>
    {
        override public void Configure(EntityTypeBuilder<Question> builder)
        {
            base.Configure(builder);

      
            builder.HasMany(e => e.FollowsUpQuestions)
                   .WithOne(e => e.ParentQuestion)
                   .HasForeignKey(e => e.ParentQuestionId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(e => e.Sender)
                   .WithMany(e => e.SendedQuestions)
                   .HasForeignKey(e => e.SenderId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(e => e.Receiver)
                   .WithMany(e => e.ReceivedQuestions)
                   .HasForeignKey(e => e.ReceiverId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
