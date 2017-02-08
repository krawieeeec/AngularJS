using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataModel.Models
{
    public class List
    {
        public List()
        {
            this.Films = new HashSet<Film>();
        }

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public virtual ICollection<Film> Films { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
    }
}
