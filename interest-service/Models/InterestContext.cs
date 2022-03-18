using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace interest_service.Models
{
    public class InterestContext : DbContext
    {
        public InterestContext(DbContextOptions<InterestContext> options)
            : base(options)
        {
        }

        public DbSet<Interest> Interests { get; set; } = null!;
    }
}
