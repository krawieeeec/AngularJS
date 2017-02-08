using System.ComponentModel.DataAnnotations;

namespace DataModel.Models
{
    public class UserFavourite
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public virtual ApplicationUser User { get; set; }
        public int ListId { get; set; }
    }
}
