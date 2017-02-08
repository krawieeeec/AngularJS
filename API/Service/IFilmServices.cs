using System.Collections.Generic;
using TO.Film;

namespace Service
{
    public interface IFilmServices
    {
        FilmEntityTO GetFilmById(int filmId);
        IEnumerable<FilmShortDescriptionTO> GetAllFilms();
        bool CheckIfFilmExists(int filmId);
        bool UpdateFilm(int filmId, FilmEntityTO filmEntity);
        void InitializeFilmsCache();
    }
}
