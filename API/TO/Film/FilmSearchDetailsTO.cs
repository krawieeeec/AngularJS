﻿namespace TO.Film
{
    public class FilmSearchDetailsTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string OriginalName { get; set; }
        public string ReleaseDate { get; set; }
        public string Genre { get; set; }
        public string Description { get; set; }
        public string Cover { get; set; }
        public string FilmwebUrl { get; set; }
        public int Votes { get; set; }
        public bool isVoted { get; set; }
    }
}
