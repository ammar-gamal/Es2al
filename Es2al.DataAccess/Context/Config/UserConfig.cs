using Es2al.Models.Entites;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Es2al.DataAccess.Context.Config
{
    public class UserConfig : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            string fullPath = Path.GetFullPath(@"..\Es2al\wwwroot\images\Default_pfp.jpg");
            builder.Property(e => e.Bio)
                   .IsRequired(false);

            builder.Property(e => e.Image)
                   .HasDefaultValue(File.ReadAllBytes(fullPath));
            
        }
    }
}
