using API.Helpers;
using NLog;
using Service;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using TO.Film;
using TO.List;

namespace API.Controllers
{
    [EnableCorsAttribute("*", "*", "*")]
    [RoutePrefix("api/Lists")]
    public class ListsController : ApiController
    {
        private readonly IListServices _listService;
        private readonly IFilmServices _filmService;
        private readonly IAuthServices _authService;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ListsController()
        {
            _listService = new ListServices();
            _filmService = new FilmServices();
            _authService = new AuthServices();
        }

        // GET api/lists
        public IHttpActionResult Get()
        {
            string identityUserName = System.Web.HttpContext.Current.User.Identity.Name;

            var lists = _listService.GetAllLists(identityUserName);

            if (lists != null)
                return Ok(lists);
            logger.Log(LogLevel.Warn, "No lists has been found.\n");
            return NotFound();
        }

        // GET api/lists/top
        [HttpGet]
        [Route("top")]
        public IHttpActionResult Top()
        {
            string identityUserName = System.Web.HttpContext.Current.User.Identity.Name;

            var lists = _listService.GetTopLists(identityUserName);

            if (lists != null)
                return Ok(lists);
            logger.Log(LogLevel.Warn, "No lists has been found.\n");
            return NotFound();
        }

        // GET api/list/5
        public IHttpActionResult Get(int id)
        {
            string identityUserName = System.Web.HttpContext.Current.User.Identity.Name;

            var list = _listService.GetListById(identityUserName, id);

            if (list != null)
                return Ok(list);
            logger.Log(LogLevel.Error, "List not found.\n");
            return NotFound();
        }

        // GET /api/lists/search
        [Route("search/{searchString}")]
        [HttpGet]
        public IHttpActionResult Search(string searchString)
        {
            string identityUserName = System.Web.HttpContext.Current.User.Identity.Name;
            var result = _listService.Search(searchString, identityUserName);

            if (result != null)
                return Ok(result);

            return NotFound();
        }

        // GET /api/lists/allTitles
        [Route("allTitles")]
        [HttpGet]
        public IHttpActionResult GetAllTitles()
        {
            var cache = MemoryCache.Default;
            List<string> names;

            if (cache.Get("namesCache") == null)
            {
                var cachePolicy = new CacheItemPolicy();
                cachePolicy.AbsoluteExpiration = DateTime.Now.AddSeconds(3600);

                //IEnumerable
                names = _listService.GetAllTitles();
                cache.Add("namesCache", names, cachePolicy);
            }
            else
            {
                names = (List<string>)cache.Get("namesCache");
            }

            if (names != null)
                return Ok(names);

            return NotFound();
        }

        // GET /api/lists/listsTitles
        [Route("listsTitles")]
        [HttpGet]
        public IHttpActionResult GetListsTitles()
        {
            var listsTitles = _listService.GetListsTitles();

            if (listsTitles != null)
                return Ok(listsTitles);

            return NotFound();
        }

        // DELETE api/list/5
        public IHttpActionResult Delete(int id)
        {
            if (id > 0)
            {
                bool exists = _listService.CheckIfListExists(id);

                if (exists == true)
                {
                    bool isModerator = System.Web.HttpContext.Current.User.IsInRole("Moderator");

                    if (!isModerator)
                    {
                        string identityUserName = System.Web.HttpContext.Current.User.Identity.Name;
                        //check if list belongs to logged user
                        string listCreator = _listService.GetListCreator(id);

                        if (listCreator != identityUserName)
                            return BadRequest(ListENUM.UNABLE_UPDATE_LIST_USER_IS_NOT_CREATOR.ToString());
                    }

                    var success = _listService.DeleteList(id);
                    if (success == true)
                        return StatusCode(HttpStatusCode.NoContent);
                }
            }

            logger.Log(LogLevel.Error, "Wrong list id.\n");
            return NotFound();
        }

        // POST api/list
        [Authorize]
        public IHttpActionResult Post(ListCreateTO listCreate)
        {
            string[] words = { "kurwa", "skurwysyn", "chuj", "huj", "pierdol", "jebać", "jebac", "jebany" };

            foreach (var word in words)
            {
                if ((listCreate.Name.ToLower()).Contains(word))
                    return BadRequest(ListENUM.UNABLE_ADD_LIST.ToString());
            }

            string identityUserName = System.Web.HttpContext.Current.User.Identity.Name;

            var userId = _authService.GetUserId(identityUserName);
            if (userId == "")
            {
                logger.Log(LogLevel.Error, "Error during creating list: " + identityUserName + "doesn't exists!");
                return BadRequest(UserENUM.USER_NOT_FOUND.ToString());
            }

            listCreate.UserId = userId;

            foreach (var film in listCreate.FilmIds)
            {
                bool exists = _filmService.CheckIfFilmExists(film);
                if (!exists)
                    return BadRequest(FilmENUM.FILM_NOT_FOUND.ToString());
            }

            var createdListId = _listService.CreateList(listCreate);
            if (createdListId != 0)
                return StatusCode(HttpStatusCode.Created);

            logger.Log(LogLevel.Error, "Unable to create list.\n");
            return BadRequest(ListENUM.UNABLE_ADD_LIST.ToString());
        }

        // PUT api/list/5
        [Authorize]
        public IHttpActionResult Put(int id, ListUpdateTO listUpdate)
        {
            if (id > 0)
            {
                string identityUserName = System.Web.HttpContext.Current.User.Identity.Name;

                //check if list exists
                bool exists = _listService.CheckIfListExists(id);
                if (!exists)
                    return BadRequest(ListENUM.LIST_NOT_FOUND.ToString());

                bool isModerator = System.Web.HttpContext.Current.User.IsInRole("Moderator");

                if (!isModerator)
                {
                    //check if list belongs to logged user
                    string listCreator = _listService.GetListCreator(id);

                    if (listCreator != identityUserName)
                        return BadRequest(ListENUM.UNABLE_UPDATE_LIST_USER_IS_NOT_CREATOR.ToString());
                }

                //check if all films exists
                if (listUpdate.FilmIds != null)
                {
                    foreach (var film in listUpdate.FilmIds)
                    {
                        bool filmExists = _filmService.CheckIfFilmExists(film);
                        if (!filmExists)
                            return BadRequest(FilmENUM.FILM_NOT_FOUND.ToString());
                    }
                }

                try
                {
                    var success = _listService.UpdateList(id, listUpdate);
                    if (success == true)
                    {
                        logger.Log(LogLevel.Info, "List with id: " + id + " was updated.\n");
                        return StatusCode(HttpStatusCode.Created);
                    }
                }
                catch (Exception ex)
                {
                    logger.Log(LogLevel.Error, ex);
                }

            }
            logger.Log(LogLevel.Error, "Unable to update list.\n");
            return BadRequest(ListENUM.UNABLE_UPDATE_LIST.ToString());
        }

        [Route("rate")]
        [HttpPost]
        [Authorize]
        public IHttpActionResult RateList(RatingShortDescriptionTO addRating)
        {
            if (ModelState.IsValid)
            {
                string identityUserName = System.Web.HttpContext.Current.User.Identity.Name;
                addRating.UserName = identityUserName;

                var userId = _authService.GetUserId(identityUserName);
                if (userId == null || userId == "")
                {
                    logger.Log(LogLevel.Error, "Error during rating list: " + identityUserName + " doesn't exists!");
                    return BadRequest(UserENUM.USER_NOT_FOUND.ToString());
                }

                var list = _listService.CheckIfListExists(addRating.ListId);

                if (list == false)
                {
                    logger.Log(LogLevel.Error, "Error during rating list - List with id: " + addRating.ListId + " doesn't exists!");
                    return BadRequest(ListENUM.LIST_NOT_FOUND.ToString());
                }

                var rating = _listService.GetRating(addRating.ListId, identityUserName);

                if (rating != null)
                {
                    if (rating.Rate != addRating.Rate)
                    {
                        addRating.UserName = identityUserName;
                        bool updateRate = _listService.UpdateRate(addRating);

                        if (!updateRate)
                        {
                            logger.Log(LogLevel.Error, "Unable to update rating for list id: " + addRating.ListId + " and userName: " + identityUserName + ".\n");
                            return BadRequest(RateENUM.UNABLE_UPDATE_RATE.ToString());
                        }

                        return StatusCode(HttpStatusCode.Created);
                    }
                    else if (rating.Rate == addRating.Rate)
                    {
                        bool deleteRate = _listService.DeleteRating(addRating);

                        if (!deleteRate)
                        {
                            logger.Log(LogLevel.Error, "Unable to delete rating for list id: " + addRating.ListId + " and userName: " + identityUserName + ".\n");
                            return BadRequest(RateENUM.UNABLE_DELETE_RATE.ToString());
                        }
                    }
                }
                else
                {
                    var addRatingResult = _listService.AddRating(addRating.ListId, addRating.Rate, userId);

                    if (!addRatingResult)
                    {
                        logger.Log(LogLevel.Error, "Unable to add rating with list id: " + addRating.ListId + " and userName: " + identityUserName + ".\n");
                        return BadRequest(RateENUM.UNABLE_ADD_RATE.ToString());
                    }

                    logger.Log(LogLevel.Info, "Rating with list id: " + addRating.ListId + " and userName: " + identityUserName + " was added.\n");
                    return StatusCode(HttpStatusCode.Created);
                }
            }

            return BadRequest(RateENUM.UNABLE_ADD_RATE.ToString());
        }

        [Route("getCurrentRate/{listId}")]
        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> GetCurrentRate(int listId)
        {
            string identityUserName = System.Web.HttpContext.Current.User.Identity.Name;

            var user = await _authService.CheckIfUserExists(identityUserName);
            if (user == false)
            {
                return BadRequest(UserENUM.USER_NOT_FOUND.ToString());
            }

            var rating = _listService.GetRating(listId, identityUserName);

            if (rating == null)
            {
                var rate = new RatingShortDescriptionTO
                {
                    ListId = listId,
                    UserName = identityUserName,
                    Rate = 0
                };

                return Ok(rate);
            }

            rating.UserName = identityUserName;
            rating.ListId = listId;

            return Ok(rating);
        }

        [Route("rateFilm")]
        [HttpPost]
        [Authorize]
        public IHttpActionResult RateFilm(RateFilmTO addFilmRating)
        {
            if (ModelState.IsValid)
            {
                string identityUserName = System.Web.HttpContext.Current.User.Identity.Name;

                var userId = _authService.GetUserId(identityUserName);
                if (userId == null)
                {
                    logger.Log(LogLevel.Error, "Error during rating film: " + identityUserName + " doesn't exists!");
                    return BadRequest(UserENUM.USER_NOT_FOUND.ToString());
                }

                var list = _listService.CheckIfListExists(addFilmRating.ListId);
                if (list == false)
                {
                    logger.Log(LogLevel.Error, "Error during rating list - List with id: " + addFilmRating.ListId + " doesn't exists!");
                    return BadRequest(ListENUM.LIST_NOT_FOUND.ToString());
                }

                var film = _filmService.CheckIfFilmExists(addFilmRating.FilmId);
                if (film == false)
                {
                    logger.Log(LogLevel.Error, "Error during adding vote for film - film with id: " + addFilmRating.FilmId + " doesn't exists!");
                    return BadRequest(FilmENUM.FILM_NOT_FOUND.ToString());
                }

                var voteExists = _listService.CheckIfListFilmRatingExists(addFilmRating.ListId, addFilmRating.FilmId, userId);
                if (voteExists == true)
                {
                    //usun plusa
                    var result = _listService.DeleteFilmVote(addFilmRating.ListId, addFilmRating.FilmId, userId);
                    return Ok();
                }
                else
                {
                    //dodaj plusa
                    _listService.AddFilmVote(addFilmRating.ListId, addFilmRating.FilmId, userId);
                    return StatusCode(HttpStatusCode.Created);
                }
            }

            return BadRequest();
        }
    }
}
