using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DopAdoption.Domain.DomainModels;
using DopAdoption.Domain.Identity;


namespace DopAdoptipon.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Breed> Breeds { get; set; }
        public DbSet<Adopter> Adopters { get; set; }
        public DbSet<AdoptionApplication> Applications { get; set; }
        public DbSet<Dog> Dogs { get; set; }

    }
}
