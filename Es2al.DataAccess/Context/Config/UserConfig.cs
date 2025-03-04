using Es2al.Models.Entites;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection;


namespace Es2al.DataAccess.Context.Config
{
    public class UserConfig : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
          
            builder.Property(e => e.Bio)
                   .IsRequired(false);

           
            
        }
    }
}
