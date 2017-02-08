using System.ComponentModel.DataAnnotations;

namespace DataModel.Models
{
    public class Rating
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public virtual ApplicationUser User { get; set; }
        [Required]
        public virtual List List { get; set; }
        // 1 - like
        // 0 - dislike
        public int Rate { get; set; }
    }
}
