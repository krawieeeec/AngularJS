using DataModel.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace DataModel
{
    public class WebApiDbEntities : IdentityDbContext<ApplicationUser>
    {
        public WebApiDbEntities()
            : base("WebApiDbEntities")
        {
            this.Configuration.ProxyCreationEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // disable plural form of naming convention for database tables
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
