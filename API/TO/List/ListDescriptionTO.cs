using System.Collections.Generic;
using TO.Film;
using TO.User;

namespace TO.List
{
    public class ListDescriptionTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<FilmListDetailsTO> Films { get; set; }

        public UserNameTO User { get; set; }

        public int Likes { get; set; }
        public int DisLikes { get; set; }
        //1 - like
        //0 - no rating found
        //-1 - dislike
        public int Voted { get; set; }
        public bool IsFavourite { get; set; }
    }
}
