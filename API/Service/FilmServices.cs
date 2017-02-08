using AutoMapper;
using DataModel.Models;
using DataModel.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using TO.Film;

namespace Service
{
    public class FilmServices : IFilmServices
    {
        private FilmRepository _filmRepository = new FilmRepository(false);

        public void InitializeFilmsCache()
        {
            ApplicationCache<Film>.FillCache(_filmRepository.GetAll().ToList());
        }

        public FilmEntityTO GetFilmById(int filmId)
        {
            var film = new Film();

            var cacheItem = ApplicationCache<Film>.GetCacheItem(d => d.Id == filmId);
            if (cacheItem != null)
            {
                film = cacheItem;
            }
            else
            {
                film = _filmRepository.GetSingleFilm(filmId);
            }

            if (film.Id != 0)
            {
                var actors = _filmRepository.GetFilmActors(filmId);
                var directors = _filmRepository.GetFilmDirectors(filmId);

                film.Actors = actors;
                film.Directors = directors;

                Mapper.CreateMap<Actor, ActorEntityTO>();
                Mapper.CreateMap<Director, DirectorEntityTO>();
                Mapper.CreateMap<Film, FilmEntityTO>();

                var filmModel = Mapper.Map<Film, FilmEntityTO>(film);
                return filmModel;
            }
            return null;
        }

        public bool CheckIfFilmExists(int filmId)
        {
            var cacheItem = ApplicationCache<Film>.GetCacheItem(d => d.Id == filmId);
            if (cacheItem != null)
            {
                return true;
            }

            return _filmRepository.CheckIfFilmExists(filmId);
        }

        public IEnumerable<FilmShortDescriptionTO> GetAllFilms()
        {
            var films = new List<Film>();
            var cache = ApplicationCache<Film>.GetCache();
            if (cache.Count() > 0)
            {
                films = cache;
            }
            else
            {
                films = _filmRepository.GetAll().ToList();
                ApplicationCache<Film>.FillCache(films);
            }

            List<FilmShortDescriptionTO> filmsModel = new List<FilmShortDescriptionTO>();

            if (films.Any())
            {
                Mapper.CreateMap<Film, FilmShortDescriptionTO>();
                filmsModel = Mapper.Map<List<Film>, List<FilmShortDescriptionTO>>(films);
            }
            return filmsModel;
        }

        public bool UpdateFilm(int filmId, FilmEntityTO filmEntity)
        {
            var success = false;
            // check post message if is not empty
            if (filmEntity != null)
            {
                using (var scope = new TransactionScope())
                {
                    // check if given film exists in database
                    // zmienic na metode getbyId....
                    var film = _filmRepository.GetSingleFilm(filmId);

                    if (film != null)
                    {
                        if ((film.Name != filmEntity.Name) && filmEntity.Name != null)
                            film.Name = filmEntity.Name;
                        if ((film.OriginalName != filmEntity.OriginalName) && filmEntity.OriginalName != null)
                            film.OriginalName = filmEntity.OriginalName;
                        if ((film.ReleaseDate != filmEntity.ReleaseDate) && filmEntity.ReleaseDate != null)
                            film.ReleaseDate = filmEntity.ReleaseDate;
                        if ((film.Genre != filmEntity.Genre) && filmEntity.Genre != null)
                            film.Genre = filmEntity.Genre;
                        if ((film.Description != filmEntity.Description) && filmEntity.Description != null)
                            film.Description = filmEntity.Description;
                        if ((film.Cover != filmEntity.Cover) && filmEntity.Cover != null)
                            film.Cover = filmEntity.Cover;
                        if ((film.FilmwebUrl != filmEntity.FilmwebUrl) && filmEntity.FilmwebUrl != null)
                            film.FilmwebUrl = filmEntity.FilmwebUrl;

                        _filmRepository.UpdateFilm(film);
                        scope.Complete();
                        success = true;
                    }

                    // update cache
                    var cache = ApplicationCache<Film>.GetCache();
                    if (cache.Count() > 0)
                    {
                        var cacheItem = ApplicationCache<Film>.GetCacheItem(d => d.Id == filmId);
                        if (cacheItem != null)
                        {
                            int cacheItemIndex = cache.FindIndex(d => d.Id == filmId);

                            #region update properties
                            if ((film.Name != filmEntity.Name) && filmEntity.Name != null)
                                cacheItem.Name = filmEntity.Name;
                            if ((film.OriginalName != filmEntity.OriginalName) && filmEntity.OriginalName != null)
                                cacheItem.OriginalName = filmEntity.OriginalName;
                            if ((film.ReleaseDate != filmEntity.ReleaseDate) && filmEntity.ReleaseDate != null)
                                cacheItem.ReleaseDate = filmEntity.ReleaseDate;
                            if ((film.Genre != filmEntity.Genre) && filmEntity.Genre != null)
                                cacheItem.Genre = filmEntity.Genre;
                            if ((film.Description != filmEntity.Description) && filmEntity.Description != null)
                                cacheItem.Description = filmEntity.Description;
                            if ((film.Cover != filmEntity.Cover) && filmEntity.Cover != null)
                                cacheItem.Cover = filmEntity.Cover;
                            if ((film.FilmwebUrl != filmEntity.FilmwebUrl) && filmEntity.FilmwebUrl != null)
                                cacheItem.FilmwebUrl = filmEntity.FilmwebUrl;
                            #endregion

                            cache[cacheItemIndex] = cacheItem;
                            ApplicationCache<Film>.FillCache(cache);
                        }
                    }
                }
            }
            return success;
        }
    }
}