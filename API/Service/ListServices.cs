using AutoMapper;
using DataModel.Models;
using DataModel.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using TO.Film;
using TO.List;
using TO.User;

namespace Service
{
    public class ListServices : IListServices
    {
        private ListRepository _listRepository = new ListRepository(false);
        private AuthRepository _authRepository = new AuthRepository(false);
        private FilmRepository _filmRepository = new FilmRepository(false);

        public void InitializeListsCache()
        {
            ApplicationCache<ListDescriptionTO>.FillCache(GetAllLists("").ToList());
        }

        public IEnumerable<ListDescriptionTO> GetAllLists(string userName)
        {
            List<ListDescriptionTO> result = new List<ListDescriptionTO>();

            var cache = ApplicationCache<ListDescriptionTO>.GetCache();

            if (cache.Count() > 0)
            {
                result = cache;
                foreach (var list in result)
                {
                    var filmsToSort = list.Films.ToList();
                    filmsToSort.Sort((x, y) => y.Votes - x.Votes);
                    list.Films = filmsToSort;
                }
            }
            else
            {
                var lists = _listRepository.GetAll();

                List<ListDescriptionTO> listsModel = new List<ListDescriptionTO>();

                if (lists.Any())
                {
                    foreach (var list in lists)
                    {
                        list.Films = _listRepository.GetListFilms(list.Id);
                    }

                    Mapper.CreateMap<Film, FilmListDetailsTO>();
                    Mapper.CreateMap<ApplicationUser, UserNameTO>();
                    Mapper.CreateMap<List, ListDescriptionTO>();

                    listsModel = Mapper.Map<List<List>, List<ListDescriptionTO>>(lists);

                    foreach (var list in listsModel)
                    {
                        foreach (var film in list.Films)
                        {
                            string[] shortDesc = film.Description.Split(' ');
                            Array.Resize(ref shortDesc, 15);
                            film.Description = string.Join(" ", shortDesc);
                        }

                        List<int> filmIds = new List<int>();
                        foreach (var film in list.Films)
                        {
                            filmIds.Add(film.Id);
                        }
                        // calculate votes count
                        Dictionary<int, int> votes = _listRepository.CalculateVotes(list.Id, filmIds);

                        foreach (var film in list.Films)
                        {
                            if (votes.ContainsKey(film.Id))
                            {
                                film.Votes = votes[film.Id];
                            }
                            else
                                film.Votes = 0;
                        }

                        var user = new UserNameTO
                        {
                            UserName = _listRepository.GetListCreator(list.Id)
                        };
                        list.User = user;
                        list.Films.OrderByDescending(d => d.Votes);
                    }
                }
                ApplicationCache<ListDescriptionTO>.FillCache(listsModel);
                result = listsModel;
            }

            if (userName != "")
            {
                string userId = "";
                //get user id
                userId = _authRepository.GetUserId(userName);

                foreach (var list in result)
                {
                    List<int> votedFilmsIds = new List<int>();
                    if (userId != "")
                    {
                        //check if user already voted for films
                        votedFilmsIds = _listRepository.CheckIfFilmsAreVoted(list.Id, userId);
                        // check if user already liked/dislikes list
                        list.Voted = _listRepository.CheckIfListIsVoted(list.Id, userId);
                        // check if user already added list to favourites
                        list.IsFavourite = _authRepository.CheckIfFavouriteExists(list.Id, userId);

                        foreach (var film in list.Films)
                        {
                            if (votedFilmsIds.Contains(film.Id))
                                film.isVoted = true;
                            else
                                film.isVoted = false;
                        }
                    }
                }
            }
            else
            {
                foreach (var list in result)
                {
                    foreach (var film in list.Films)
                    {
                        film.isVoted = false;
                    }
                    list.Voted = 0;
                }
            }

            result.OrderByDescending(d => d.Likes);

            return result;
        }

        public IEnumerable<ListDescriptionTO> GetTopLists(string userName)
        {
            List<ListDescriptionTO> result = new List<ListDescriptionTO>();

            var cache = ApplicationCache<ListDescriptionTO>.GetCache();

            if (cache.Count() > 0)
            {
                result = cache.OrderByDescending(d => d.Likes).ToList();
                result = result.Take(3).ToList();
                foreach (var list in result)
                {
                    var filmsToSort = list.Films.ToList();
                    filmsToSort.Sort((x, y) => y.Votes - x.Votes);
                    list.Films = filmsToSort;
                }
            }
            else
            {
                var lists = _listRepository.GetTopLists();

                List<ListDescriptionTO> listsModel = new List<ListDescriptionTO>();

                if (lists.Any())
                {
                    foreach (var list in lists)
                    {
                        list.Films = _listRepository.GetListFilms(list.Id);
                    }

                    Mapper.CreateMap<Film, FilmListDetailsTO>();
                    Mapper.CreateMap<ApplicationUser, UserNameTO>();
                    Mapper.CreateMap<List, ListDescriptionTO>();

                    listsModel = Mapper.Map<List<List>, List<ListDescriptionTO>>(lists);

                    foreach (var list in listsModel)
                    {
                        foreach (var film in list.Films)
                        {
                            string[] shortDesc = film.Description.Split(' ');
                            Array.Resize(ref shortDesc, 15);
                            film.Description = string.Join(" ", shortDesc);
                        }

                        List<int> filmIds = new List<int>();
                        foreach (var film in list.Films)
                        {
                            filmIds.Add(film.Id);
                        }
                        // calculate votes count
                        Dictionary<int, int> votes = _listRepository.CalculateVotes(list.Id, filmIds);

                        foreach (var film in list.Films)
                        {
                            if (votes.ContainsKey(film.Id))
                            {
                                film.Votes = votes[film.Id];
                            }
                            else
                                film.Votes = 0;
                        }

                        var user = new UserNameTO
                        {
                            UserName = _listRepository.GetListCreator(list.Id)
                        };
                        list.User = user;
                        list.Films.OrderByDescending(d => d.Votes);
                    }
                }
                ApplicationCache<ListDescriptionTO>.FillCache(listsModel);
                result = listsModel;
            }

            if (userName != "")
            {
                string userId = "";
                //get user id
                userId = _authRepository.GetUserId(userName);

                foreach (var list in result)
                {
                    List<int> votedFilmsIds = new List<int>();
                    if (userId != "")
                    {
                        //check if user already voted for films
                        votedFilmsIds = _listRepository.CheckIfFilmsAreVoted(list.Id, userId);
                        // check if user already liked/dislikes list
                        list.Voted = _listRepository.CheckIfListIsVoted(list.Id, userId);
                        // check if user already added list to favourites
                        list.IsFavourite = _authRepository.CheckIfFavouriteExists(list.Id, userId);

                        foreach (var film in list.Films)
                        {
                            if (votedFilmsIds.Contains(film.Id))
                                film.isVoted = true;
                            else
                                film.isVoted = false;
                        }
                    }
                }
            }
            else
            {
                foreach (var list in result)
                {
                    foreach (var film in list.Films)
                    {
                        film.isVoted = false;
                    }
                    list.Voted = 0;
                }
            }

            return result;
        }

        public ListDescriptionTO GetListById(string userName, int listId)
        {
            ListDescriptionTO result = new ListDescriptionTO();

            var cacheItem = ApplicationCache<ListDescriptionTO>.GetCacheItem(d => d.Id == listId);

            if (cacheItem != null)
            {
                result = cacheItem;
            }
            else
            {
                result = getListDetails(userName, listId);
            }

            if (userName != "")
            {
                string userId = "";
                //get user id
                userId = _authRepository.GetUserId(userName);

                List<int> votedFilmsIds = new List<int>();
                if (userId != "")
                {
                    //check if user already voted for films
                    votedFilmsIds = _listRepository.CheckIfFilmsAreVoted(result.Id, userId);
                    // check if user already liked/dislikes list
                    result.Voted = _listRepository.CheckIfListIsVoted(result.Id, userId);
                    // check if user already added list to favourites
                    result.IsFavourite = _authRepository.CheckIfFavouriteExists(result.Id, userId);

                    foreach (var film in result.Films)
                    {
                        if (votedFilmsIds.Contains(film.Id))
                            film.isVoted = true;
                        else
                            film.isVoted = false;
                    }
                }
            }
            else
            {
                foreach (var film in result.Films)
                {
                    film.isVoted = false;
                }
                result.Voted = 0;
            }

            return result;
        }

        private ListDescriptionTO getListDetails(string userName, int listId)
        {
            var list = _listRepository.GetSingle(listId);

            if (list != null)
            {
                list.Films = _listRepository.GetListFilms(listId);

                Mapper.CreateMap<Film, FilmListDetailsTO>();
                Mapper.CreateMap<ApplicationUser, UserNameTO>();
                Mapper.CreateMap<List, ListDescriptionTO>();

                var listModel = Mapper.Map<List, ListDescriptionTO>(list);

                foreach (var film in listModel.Films)
                {
                    string[] shortDesc = film.Description.Split(' ');
                    Array.Resize(ref shortDesc, 15);
                    film.Description = string.Join(" ", shortDesc);
                }

                List<int> filmIds = new List<int>();
                foreach (var film in listModel.Films)
                {
                    filmIds.Add(film.Id);
                }
                // calculate votes count
                Dictionary<int, int> votes = _listRepository.CalculateVotes(list.Id, filmIds);

                foreach (var film in listModel.Films)
                {
                    if (votes.ContainsKey(film.Id))
                    {
                        film.Votes = votes[film.Id];
                    }
                    else
                        film.Votes = 0;
                }

                listModel.Films.OrderByDescending(d => d.Votes);

                var user = new UserNameTO
                {
                    UserName = _listRepository.GetListCreator(listId)
                };
                listModel.User = user;

                ApplicationCache<ListDescriptionTO>.AddCacheItem(listModel);
                return listModel;
            }

            return null;
        }

        public SearchTO Search(string searchString, string userName)
        {
            var films = new List<Film>();
            List<FilmShortDescriptionTO> filmsModel = new List<FilmShortDescriptionTO>();

            var filmsCache = ApplicationCache<Film>.GetCache();

            if (searchString.ToLower().Contains("filmy z"))
            {
                // get actor name
                var actorName = searchString.Remove(0, 8);
                // get search actor id
                var filmIds = _filmRepository.GetActorFilmIds(actorName);
                // get films with mentioned above actor
                if (filmIds.Count > 0)
                    films = _filmRepository.GetMany(filmIds);
            }
            else
            {
                var cachedFilms = filmsCache.Where(d => d.Name.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                var cachedFilmsByGenre = filmsCache.Where(d => d.Genre.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0).Take(30).ToList();
                if (cachedFilms.Count > 0)
                {
                    films = cachedFilms;
                    // get count of search films
                    int filmsCount = films.Count;
                    // calculate how many films we need to download (by genre)
                    int additionalFilmsGet = 30 - filmsCount;
                    if (additionalFilmsGet > 0)
                        films.AddRange(cachedFilmsByGenre.Take(additionalFilmsGet));
                }
                else if (cachedFilmsByGenre.Count > 0)
                {
                    films = cachedFilmsByGenre;
                }
                else
                {
                    films = _listRepository.SearchFilms(searchString);
                }
            }

            foreach (var film in films)
            {
                filmsModel.Add(new FilmShortDescriptionTO()
                {
                    Id = film.Id,
                    Name = film.Name,
                    OriginalName = film.OriginalName,
                    ReleaseDate = film.ReleaseDate,
                    Genre = film.Genre,
                    Description = film.Description,
                    Cover = film.Cover,
                    FilmwebUrl = film.FilmwebUrl
                });
            }

            List<ListSearchDetailsTO> listsResult = new List<ListSearchDetailsTO>();
            // get lists
            var lists = _listRepository.SearchLists(searchString);
            foreach (var list in lists)
            {
                // get list films
                list.Films = _listRepository.GetListFilms(list.Id);

                // map List to ListSearchDetailsTO
                var singleListDetails = new ListSearchDetailsTO();
                singleListDetails.Id = list.Id;
                singleListDetails.Name = list.Name;
                singleListDetails.Likes = list.Likes;
                singleListDetails.DisLikes = list.Dislikes;
                singleListDetails.Films = new List<FilmSearchDetailsTO>();

                // calculate votes count for each film
                List<int> filmIds = new List<int>();
                foreach (var film in list.Films)
                {
                    filmIds.Add(film.Id);
                }
                Dictionary<int, int> votes = _listRepository.CalculateVotes(singleListDetails.Id, filmIds);

                foreach (var film in list.Films)
                {
                    FilmSearchDetailsTO singleFilmSearchDetails = new FilmSearchDetailsTO();
                    singleFilmSearchDetails.Id = film.Id;
                    singleFilmSearchDetails.Name = film.Name;
                    singleFilmSearchDetails.OriginalName = film.OriginalName;
                    singleFilmSearchDetails.ReleaseDate = film.ReleaseDate;
                    singleFilmSearchDetails.Genre = film.Genre;
                    singleFilmSearchDetails.Description = film.Description;
                    singleFilmSearchDetails.Cover = film.Cover;
                    singleFilmSearchDetails.FilmwebUrl = film.FilmwebUrl;

                    if (votes.ContainsKey(film.Id))
                    {
                        singleFilmSearchDetails.Votes = votes[film.Id];
                    }
                    else
                        singleFilmSearchDetails.Votes = 0;

                    singleListDetails.Films.Add(singleFilmSearchDetails);
                }

                listsResult.Add(singleListDetails);
            }

            if (userName != "")
            {
                string userId = "";
                //get user id
                userId = _authRepository.GetUserId(userName);

                foreach (var list in listsResult)
                {
                    List<int> votedFilmsIds = new List<int>();
                    if (userId != "")
                    {
                        //check if user already voted for films
                        votedFilmsIds = _listRepository.CheckIfFilmsAreVoted(list.Id, userId);
                        // check if user already liked/dislikes list
                        list.Voted = _listRepository.CheckIfListIsVoted(list.Id, userId);
                        // check if user already added list to favourites
                        list.IsFavourite = _authRepository.CheckIfFavouriteExists(list.Id, userId);

                        foreach (var film in list.Films)
                        {
                            if (votedFilmsIds.Contains(film.Id))
                                film.isVoted = true;
                            else
                                film.isVoted = false;
                        }
                    }
                    list.Films.OrderByDescending(d => d.Votes);
                }
            }

            listsResult.OrderByDescending(d => d.Likes);

            var searchResult = new SearchTO()
            {
                SearchString = searchString,
                Films = filmsModel,
                Lists = listsResult
            };

            return searchResult;
        }

        public bool CheckIfListExists(int listId)
        {
            var cacheItem = ApplicationCache<ListDescriptionTO>.GetCacheItem(d => d.Id == listId);

            if (cacheItem != null)
            {
                return true;
            }
            else
            {
                var list = _listRepository.GetSingle(listId);

                if (list != null)
                    return true;
            }

            return false;
        }

        public string GetListCreator(int listId)
        {
            string result = "";

            var cacheItem = ApplicationCache<ListDescriptionTO>.GetCacheItem(d => d.Id == listId);

            if (cacheItem != null)
            {
                result = cacheItem.User.UserName;
            }
            else
            {
                result = _listRepository.GetListCreator(listId);
            }

            return result;
        }

        public RatingShortDescriptionTO GetRating(int listId, string userName)
        {
            var result = new RatingShortDescriptionTO();

            var cacheItem = ApplicationCache<Rating>.GetCacheItem(d => d.Id == listId && d.User.UserName == userName);

            if (cacheItem != null)
            {
                Mapper.CreateMap<Rating, RatingShortDescriptionTO>();

                result = Mapper.Map<Rating, RatingShortDescriptionTO>(cacheItem);
            }
            else
            {
                result = getRating(listId, userName);
            }

            return result;
        }

        private RatingShortDescriptionTO getRating(int listId, string userName)
        {
            var rating = _listRepository.GetRating(listId, userName);

            if (rating.List.Id != 0)
            {
                ApplicationCache<Rating>.AddCacheItem(rating);
                Mapper.CreateMap<Rating, RatingShortDescriptionTO>();

                var ratingModel = Mapper.Map<Rating, RatingShortDescriptionTO>(rating);
                ratingModel.UserName = userName;
                return ratingModel;
            }

            return null;
        }

        public bool UpdateRate(RatingShortDescriptionTO rating)
        {
            var success = false;
            if (rating.Rate == -1 || rating.Rate == 1)
            {
                using (var scope = new TransactionScope())
                {
                    var rate = new Rating();

                    var cache = ApplicationCache<Rating>.GetCache();

                    if (cache.Count() > 0)
                    {
                        var cacheItem = cache.Where(d => d.List.Id == rating.ListId && d.User.UserName == rating.UserName).FirstOrDefault();

                        if (cacheItem != null)
                        {
                            rate = cacheItem;
                        }
                        else
                        {
                            rate = _listRepository.GetRating(rating.ListId, rating.UserName);
                        }
                    }
                    else
                    {
                        rate = _listRepository.GetRating(rating.ListId, rating.UserName);
                    }


                    if (rate != null)
                    {
                        //remove old rating cache item
                        ApplicationCache<Rating>.RemoveCacheItem(rate);

                        rate.Rate = rating.Rate;
                        // add updated rating to cache
                        ApplicationCache<Rating>.AddCacheItem(rate);

                        //update cache in db
                        _listRepository.UpdateRate(rating.ListId, rating.UserName, rating.Rate);
                        scope.Complete();
                    }
                }

                using (var scope = new TransactionScope())
                {
                    _listRepository.UpdateLikesDislikesCount(rating.ListId, false, false, true);

                    //update cache ListDescriptionTO
                    var cache = ApplicationCache<ListDescriptionTO>.GetCache();
                    if (cache.Count() > 0)
                    {
                        var cacheItem = cache.FirstOrDefault(d => d.Id == rating.ListId);
                        if (cacheItem != null)
                        {
                            //update likes
                            cacheItem.Likes = _listRepository.GetLikesCount(rating.ListId);
                            //update dislikes
                            cacheItem.DisLikes = _listRepository.GetDislikesCount(rating.ListId);
                            var listIndex = cache.FindIndex(d => d.Id == rating.ListId);
                            // replace object
                            cache[listIndex] = cacheItem;

                            ApplicationCache<ListDescriptionTO>.FillCache(cache);
                        }
                    }

                    scope.Complete();
                    success = true;
                }
            }
            return success;
        }

        public bool DeleteRating(RatingShortDescriptionTO rating)
        {
            var success = false;
            if (rating.Rate == -1 || rating.Rate == 1)
            {
                using (var scope = new TransactionScope())
                {
                    var rate = new Rating();

                    var cache = ApplicationCache<Rating>.GetCache();

                    if (cache.Count() > 0)
                    {
                        var cacheItem = cache.Where(d => d.List.Id == rating.ListId && d.User.UserName == rating.UserName).FirstOrDefault();

                        if (cacheItem != null)
                        {
                            ApplicationCache<Rating>.RemoveCacheItem(cacheItem);
                        }
                    }

                    success = _listRepository.DeleteRating(rating.ListId, rating.UserName);
                    scope.Complete();
                }

                if (success == true)
                {
                    using (var scope = new TransactionScope())
                    {
                        _listRepository.UpdateLikesDislikesCount(rating.ListId, false, false, true);

                        //update cache ListDescriptionTO
                        var cache = ApplicationCache<ListDescriptionTO>.GetCache();
                        if (cache.Count() > 0)
                        {
                            var cacheItem = cache.FirstOrDefault(d => d.Id == rating.ListId);
                            if (cacheItem != null)
                            {
                                //update likes
                                cacheItem.Likes = _listRepository.GetLikesCount(rating.ListId);
                                //update dislikes
                                cacheItem.DisLikes = _listRepository.GetDislikesCount(rating.ListId);
                                var listIndex = cache.FindIndex(d => d.Id == rating.ListId);
                                // replace object
                                cache[listIndex] = cacheItem;

                                ApplicationCache<ListDescriptionTO>.FillCache(cache);
                            }
                        }

                        scope.Complete();
                    }
                    return true;
                }
            }
            return false;
        }

        public bool AddRating(int listId, int rate, string userId)
        {
            if (rate == -1 || rate == 1)
            {
                using (var scope = new TransactionScope())
                {
                    _listRepository.AddRating(listId, rate, userId);
                    scope.Complete();
                }

                var cache = ApplicationCache<ListDescriptionTO>.GetCache();
                if (cache.Count() > 0)
                {
                    var cacheItem = cache.FirstOrDefault(d => d.Id == listId);
                    if (cacheItem != null)
                    {
                        //update likes
                        cacheItem.Likes = _listRepository.GetLikesCount(listId);
                        //update dislikes
                        cacheItem.DisLikes = _listRepository.GetDislikesCount(listId);
                        var listIndex = cache.FindIndex(d => d.Id == listId);
                        // replace object
                        cache[listIndex] = cacheItem;

                        ApplicationCache<ListDescriptionTO>.FillCache(cache);
                    }
                }
                return true;
            }
            return false;
        }

        public bool CheckIfListFilmRatingExists(int listId, int filmId, string userId)
        {
            var cacheItem = ApplicationCache<ListFilmRating>.GetCacheItem(d => d.ListId == listId && d.FilmId == filmId && d.UserId == userId);

            if (cacheItem != null)
            {
                return true;
            }
            else
            {
                return checkIfListFilmRatingExists(listId, filmId, userId);
            }
        }

        private bool checkIfListFilmRatingExists(int listId, int filmId, string userId)
        {
            bool voteExists = _listRepository.CheckIfListFilmVoteExists(listId, filmId, userId);

            if (voteExists == true)
            {
                var listFilmRating = new ListFilmRating()
                {
                    ListId = listId,
                    FilmId = filmId,
                    UserId = userId
                };
                ApplicationCache<ListFilmRating>.AddCacheItem(listFilmRating);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AddFilmVote(int listId, int filmId, string userId)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    _listRepository.AddFilmVote(listId, filmId, userId);
                    scope.Complete();

                    var cache = ApplicationCache<ListDescriptionTO>.GetCache();
                    if (cache.Count() > 0)
                    {
                        // find list
                        var list = cache.Find(d => d.Id == listId);
                        var listIndex = cache.FindIndex(d => d.Id == listId);
                        // find film
                        list.Films.Where(d => d.Id == filmId).FirstOrDefault().Votes += 1;
                        list.Films.Where(d => d.Id == filmId).FirstOrDefault().isVoted = true;
                        // replace object
                        cache[listIndex] = list;

                        ApplicationCache<ListDescriptionTO>.FillCache(cache);
                        ApplicationCache<ListFilmRating>.AddCacheItem(new ListFilmRating()
                        {
                            ListId = listId,
                            FilmId = filmId,
                            UserId = userId
                        });
                    }

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteFilmVote(int listId, int filmId, string userId)
        {
            var success = false;
            if (listId > 0)
            {
                using (var scope = new TransactionScope())
                {
                    _listRepository.DeleteFilmVote(listId, filmId, userId);
                    scope.Complete();
                    success = true;

                    var cache = ApplicationCache<ListDescriptionTO>.GetCache();
                    if (cache.Count() > 0)
                    {
                        // find list
                        var list = cache.Find(d => d.Id == listId);
                        var listIndex = cache.FindIndex(d => d.Id == listId);
                        // find film
                        list.Films.Where(d => d.Id == filmId).FirstOrDefault().Votes -= 1;
                        list.Films.Where(d => d.Id == filmId).FirstOrDefault().isVoted = false;
                        // replace object
                        cache[listIndex] = list;

                        ApplicationCache<ListDescriptionTO>.FillCache(cache);

                        var listFilmRatingCache = ApplicationCache<ListFilmRating>.GetCache();
                        var cacheItem = listFilmRatingCache.SingleOrDefault(d => d.ListId == listId && d.FilmId == filmId && d.UserId == userId);
                        if (cacheItem != null)
                        {
                            listFilmRatingCache.Remove(cacheItem);
                        }
                        ApplicationCache<ListFilmRating>.FillCache(listFilmRatingCache);
                    }

                }
            }
            return success;
        }

        public bool DeleteList(int listId)
        {
            List list = new List();
            if (listId > 0)
            {
                using (var scope = new TransactionScope())
                {
                    var cachedList = ApplicationCache<ListDescriptionTO>.GetCacheItem(d => d.Id == listId);
                    if (cachedList != null)
                    {
                        ApplicationCache<ListDescriptionTO>.RemoveCacheItem(cachedList);
                    }

                    var cachedRating = ApplicationCache<ListFilmRating>.GetCacheItem(d => d.ListId == listId);
                    if (cachedRating != null)
                    {
                        ApplicationCache<ListFilmRating>.RemoveCacheItem(cachedRating);
                    }

                    bool result = _listRepository.Delete(listId);
                    if (result == true)
                    {
                        scope.Complete();
                        return true;
                    }
                }
            }
            return false;
        }

        public int CreateList(ListCreateTO listCreate)
        {
            int createdListId = 0;
            using (var scope = new TransactionScope())
            {
                createdListId = _listRepository.Insert(listCreate.FilmIds, listCreate.Name, listCreate.UserId);

                addNewestListToCache(createdListId, false);
                scope.Complete();
            }

            return createdListId;
        }

        private void addNewestListToCache(int listId, bool? isUpdate)
        {
            var list = _listRepository.GetSingle(listId);

            if (list != null)
            {
                list.Films = _listRepository.GetListFilms(listId);

                Mapper.CreateMap<Film, FilmListDetailsTO>();
                Mapper.CreateMap<ApplicationUser, UserNameTO>();
                Mapper.CreateMap<List, ListDescriptionTO>();

                var listModel = Mapper.Map<List, ListDescriptionTO>(list);

                foreach (var film in listModel.Films)
                {
                    string[] shortDesc = film.Description.Split(' ');
                    Array.Resize(ref shortDesc, 15);
                    film.Description = string.Join(" ", shortDesc);
                }

                List<int> filmIds = new List<int>();
                foreach (var film in listModel.Films)
                {
                    filmIds.Add(film.Id);
                }

                foreach (var film in listModel.Films)
                {
                    film.Votes = 0;
                    film.isVoted = false;
                }

                listModel.Films.OrderByDescending(d => d.Votes);

                var user = new UserNameTO
                {
                    UserName = _listRepository.GetListCreator(listId)
                };
                listModel.User = user;
                listModel.Likes = 0;
                listModel.DisLikes = 0;
                listModel.Voted = 0;

                var cache = ApplicationCache<ListDescriptionTO>.GetCache();
                if (isUpdate == false)
                {
                    if (cache.Count() > 0)
                    {
                        ApplicationCache<ListDescriptionTO>.AddCacheItem(listModel);
                    }
                }
                else
                {
                    if (cache.Count() > 0)
                    {
                        var cacheItem = cache.FirstOrDefault(d => d.Id == listId);
                        if (cacheItem != null)
                        {
                            int cacheItemIndex = cache.IndexOf(cacheItem);
                            cache[cacheItemIndex] = listModel;
                        }
                    }
                }
            }
        }

        public bool UpdateList(int listId, ListUpdateTO listUpdate)
        {
            if (listUpdate != null)
            {
                using (var scope = new TransactionScope())
                {
                    var result = _listRepository.UpdateList(listId, listUpdate.Name, listUpdate.FilmIds);
                    if (result == false)
                        return false;

                    addNewestListToCache(listId, true);
                    scope.Complete();
                    return true;
                }
            }
            return false;
        }

        public List<string> GetAllTitles()
        {
            return _listRepository.GetAllTitles();
        }

        public List<ListIdTitleTO> GetListsTitles()
        {
            var lists = _listRepository.GetListsTitles();

            if (lists != null)
            {
                Mapper.CreateMap<List, ListIdTitleTO>();

                var listsModel = Mapper.Map<List<List>, List<ListIdTitleTO>>(lists);
                return listsModel;
            }

            return null;
        }
    }
}