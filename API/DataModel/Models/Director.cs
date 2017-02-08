using System.ComponentModel.DataAnnotations;

namespace DataModel.Models
{
    public class Director
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual Film Film { get; set; }
    }
}
