using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static UniversityEvents.Core.Entities.Auth.IdentityModel;

namespace UniversityEvents.Infrastructure.Configuration.AuthConfiguration;

public class UserRoleConfiguration: IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasData(new UserRole
        {
            RoleId = 1,
            UserId = 1,
        }, new UserRole
        {
            RoleId = 2,
            UserId = 2,
        }, new UserRole
        {
            RoleId = 3,
            UserId = 3,
        }, new UserRole
        {
            RoleId = 4,
            UserId = 4,
        });
    }
}
