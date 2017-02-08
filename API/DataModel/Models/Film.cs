using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataModel.Models
{
    public class Film
    {
        public Film()
        {
            this.Directors = new HashSet<Director>();
            this.Actors = new HashSet<Actor>();
            this.Lists = new HashSet<List>();
        }

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string OriginalName { get; set; }
        public string ReleaseDate { get; set; }
        public string Genre { get; set; }
        public string Description { get; set; }
        public string Cover { get; set; }
        public string FilmwebUrl { get; set; }

        public virtual ICollection<Director> Directors { get; set; }
        public virtual ICollection<Actor> Actors { get; set; }
        public virtual ICollection<List> Lists { get; set; }
    }
}
