using System.Collections.Generic;
using TO.Film;

namespace TO.List
{
    public class SearchTO
    {
        public string SearchString { get; set; }
        public List<FilmShortDescriptionTO> Films { get; set; }
        public List<ListSearchDetailsTO> Lists { get; set; }
    }
}
