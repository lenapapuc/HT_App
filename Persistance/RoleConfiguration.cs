using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Infrastructure
{
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole<Guid>>
    {
        public void Configure(EntityTypeBuilder<IdentityRole<Guid>> builder)
        {
            var roles = Enum.GetNames(typeof(UserRole))
                .Select(role => new IdentityRole<Guid>() { Id = Guid.NewGuid(), Name = role, NormalizedName = role.Normalize() })
                .ToList();

            builder.HasData(roles);
        }
    }
}
