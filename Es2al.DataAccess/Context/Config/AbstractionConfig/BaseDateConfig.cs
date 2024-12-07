using Es2al.Models.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Es2al.DataAccess.Context.Config.AbstractConfig
{
    public class BaseDateConfig<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseDate
    {
        virtual public void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.Property(e => e.Date)
                   .HasColumnType("datetime2")
                   .HasPrecision(0);
        }
    }
}
