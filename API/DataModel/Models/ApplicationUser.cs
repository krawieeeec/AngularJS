using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataModel.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            this.Lists = new HashSet<List>();
        }

        [Key]
        public override string Id { get; set; }
        public override string UserName { get; set; }
        public override string Email { get; set; }
        public virtual ICollection<List> Lists { get; set; }
        public virtual ICollection<UserFavourite> UserFavourites { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
    }
}
