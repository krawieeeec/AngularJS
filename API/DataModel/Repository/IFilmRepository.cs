using DataModel.Models;
using System.Collections.Generic;

namespace DataModel.Repository
{
    public interface IFilmRepository
    {
        IEnumerable<Film> GetAll();
        Film GetSingleFilm(int filmId);
        List<Film> GetMany(List<int> ids);
        ICollection<Actor> GetFilmActors(int filmId);
        List<int> GetActorFilmIds(string name);
        ICollection<Director> GetFilmDirectors(int filmId);
        bool CheckIfFilmExists(int filmId);
        void UpdateFilm(Film film);
    }
}
