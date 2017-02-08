using System.Collections.Generic;
using TO.Film;

namespace TO.List
{
    public class ListSearchDetailsTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<FilmSearchDetailsTO> Films { get; set; }
        public int Likes { get; set; }
        public int DisLikes { get; set; }
        public int Voted { get; set; }
        public bool IsFavourite { get; set; }
    }
}
