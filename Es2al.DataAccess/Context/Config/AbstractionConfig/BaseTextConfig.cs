using Es2al.Models.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Es2al.DataAccess.Context.Config.AbstractConfig
{
    public class BaseTextConfig<TEntity> : BaseDateConfig<TEntity>,IEntityTypeConfiguration<TEntity>
                                           where TEntity : BaseText
    {
         override public void Configure(EntityTypeBuilder<TEntity> builder)
         {
            base.Configure(builder);

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                   .ValueGeneratedOnAdd();

            builder.Property(e => e.Text)
                    .HasMaxLength(500)
                    .IsRequired(true);
         }
    }
}
