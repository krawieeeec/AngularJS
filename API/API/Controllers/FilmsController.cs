using API.Helpers;
using NLog;
using Service;
using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;
using TO.Film;

namespace API.Controllers
{
    // 1 parametr - podajemy adres, ktoremu zezwalamy dostep - w tym przypadku bedzie to adres angulara, daje gwiazdke bo kazdy ma inny port na localhoscie
    // 2 parametr - jakie dopuszczamy headery - * - wszystkie
    // 3 parametr - jakie metody sa zezwolone
    // PAMIETAC ZEBY ZMIENIC PO WRZUCENIU NA SERWER
    [EnableCorsAttribute("*", "*", "*")]
    [RoutePrefix("api/Films")]
    public class FilmsController : ApiController
    {
        private readonly IFilmServices _filmServices;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public FilmsController()
        {
            _filmServices = new FilmServices();
        }

        // GET api/film
        public IHttpActionResult Get()
        {
            var films = _filmServices.GetAllFilms();

            if (films != null)
                return Ok(films);
            logger.Log(LogLevel.Warn, "No films has been found.\n");
            return NotFound();
        }

        // GET api/film/5
        public IHttpActionResult Get(int id)
        {
            var film = _filmServices.GetFilmById(id);

            if (film != null)
                return Ok(film);
            logger.Log(LogLevel.Warn, "Film not found.\n");
            return NotFound();
        }

        // PUT api/film/5
        [HttpPut]
        [Authorize(Roles = "Moderator")]
        public IHttpActionResult Put(int id, FilmEntityTO FilmEntity)
        {
            if (ModelState.IsValid)
            {
                if (id > 0)
                {
                    var success = _filmServices.UpdateFilm(id, FilmEntity);
                    if (success == true)
                    {
                        logger.Log(LogLevel.Info, "Film with id: " + id + " was updated.\n");
                        return StatusCode(HttpStatusCode.Created);
                    }
                }
                logger.Log(LogLevel.Error, "Unable update film - wrong film id.\n");
                return BadRequest(FilmENUM.INCORRECT_FILM_ID.ToString());
            }

            return BadRequest(FilmENUM.UNABLE_UPDATE_FILM.ToString());
        }
    }
}
