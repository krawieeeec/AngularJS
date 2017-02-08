using System.ComponentModel.DataAnnotations;

namespace DataModel.Models
{
    public class FilmVote
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public virtual Film Film { get; set; }
        [Required]
        public virtual ApplicationUser User { get; set; }
        [Required]
        public virtual List List { get; set; }
    }
}
