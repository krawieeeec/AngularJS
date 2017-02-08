using System.ComponentModel.DataAnnotations;
namespace TO.Film
{
    public class DirectorEntityTO
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string Name { get; set; }
    }
}
