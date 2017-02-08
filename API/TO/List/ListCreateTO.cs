using System.Collections.Generic;

namespace TO.List
{
    public class ListCreateTO
    {
        public string Name { get; set; }
        public List<int> FilmIds { get; set; }
        public string UserId { get; set; }
    }
}
