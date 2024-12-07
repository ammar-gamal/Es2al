using Es2al.Models.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es2al.DataAccess.Context.Config
{
    public class QuestionThreadConfig : IEntityTypeConfiguration<QuestionThread>
    {
        public void Configure(EntityTypeBuilder<QuestionThread> builder)
        {
            builder.Property(e => e.IsClosed)
                   .HasDefaultValue(false);

            builder.HasMany(e => e.Questions)
                   .WithOne(e => e.Thread)
                   .HasForeignKey(e => e.ThreadId);
        }
    }
}
