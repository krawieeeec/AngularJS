using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TO.Film
{
    public class FilmEntityTO
    {
        public int Id { get; set; }

        [MaxLength(40)]
        public string Name { get; set; }

        [MaxLength(40)]
        public string OriginalName { get; set; }

        [MaxLength(70)]
        public string ReleaseDate { get; set; }

        [MaxLength(20)]
        public string Genre { get; set; }
        public string Description { get; set; }

        [Url]
        public string Cover { get; set; }

        [Url]
        public string FilmwebUrl { get; set; }

        public ICollection<DirectorEntityTO> Directors { get; set; }
        public ICollection<ActorEntityTO> Actors { get; set; }
    }
}
