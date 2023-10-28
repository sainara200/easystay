using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SlnWeb.IdentityApp;

namespace SlnWeb.DBContext
{
    public class ApplicationDbContext : IdentityDbContext 
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
         
        public DbSet<ApplicationUser> UsuarioApp { get; set; }



    }
}

