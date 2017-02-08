using API.Helpers;
using NLog;
using Service;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using TO.List;
using TO.User;

namespace API.Controllers
{
    [RoutePrefix("api/Accounts")]
    [EnableCorsAttribute("*", "*", "*")]
    public class AccountsController : ApiController
    {
        private readonly IAuthServices _authService;
        private readonly IListServices _listService;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public AccountsController()
        {
            _authService = new AuthServices();
            _listService = new ListServices();
        }

        #region get info & all users

        // GET api/accounts/userInfo
        [Route("userInfo")]
        [HttpGet]
        [Authorize]
        public IHttpActionResult GetUserInfo()
        {
            string userName = HttpContext.Current.User.Identity.Name;
            var user = _authService.GetUserInfo(userName);
            if (user != null)
                return Ok(user);
            logger.Log(LogLevel.Error, "User was not found.\n");
            return NotFound();
        }

        [Route("UsersNames")]
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult GetAllUsersNames()
        {
            var usersNames = _authService.GetAllUsersNames();
            if (usersNames != null)
                return Ok(usersNames);
            logger.Log(LogLevel.Error, "Users were not found.\n");
            return NotFound();
        }

        // GET api/accounts/getAllUsers
        [Route("getAllUsers")]
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult GetAllUsers()
        {
            var cache = MemoryCache.Default;
            List<UserInfoTO> users;

            if (cache.Get("usersCache") == null)
            {
                var cachePolicy = new CacheItemPolicy();
                cachePolicy.AbsoluteExpiration = DateTime.Now.AddSeconds(14400);

                users = _authService.GetAllUsers();
                cache.Add("usersCache", users, cachePolicy);
            }
            else
            {
                users = (List<UserInfoTO>)cache.Get("usersCache");
            }

            if (users != null)
                return Ok(users);
            logger.Log(LogLevel.Error, "No users found!\n");
            return NotFound();
        }
        #endregion

        #region register

        // POST api/Accounts/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterUserTO registerUserTO)
        {
            if (registerUserTO.Password != registerUserTO.ConfirmPassword)
            {
                return BadRequest(UserENUM.DIFFERENT_PASSWORDS.ToString());
            }

            if (await _authService.CheckIfUserExists(registerUserTO.UserName))
                return BadRequest(UserENUM.USERNAME_ALREADY_TAKEN.ToString());

            bool emailExists = _authService.CheckIfEmailExists(registerUserTO.Email);

            if (emailExists)
                return BadRequest(UserENUM.EMAIL_ALREADY_TAKEN.ToString());

            bool result = await _authService.RegisterUser(registerUserTO);

            if (!result)
            {
                logger.Log(LogLevel.Error, "Unable to create user with name " + registerUserTO.UserName);
                return BadRequest(UserENUM.UNABLE_REGISTER.ToString());
            }

            logger.Log(LogLevel.Info, "User " + registerUserTO.UserName + " was created.\n");
            return StatusCode(HttpStatusCode.Created);
        }

        // GET api/accounts/ConfirmEmail?userid=xxxx&code=yyyy
        [HttpGet]
        [Route("~/ConfirmEmail")]
        public async Task<IHttpActionResult> ConfirmEmail(string userId = "", string code = "")
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                logger.Log(LogLevel.Error, "User ID or/and Code were not set.\n");
                return BadRequest(UserENUM.USER_ID_AND_CODE_REQUIRED.ToString());
            }

            bool result = await _authService.ConfirmEmail(userId, code);

            if (!result)
            {
                logger.Log(LogLevel.Error, "Unable to confirm email with code: " + code);
                return BadRequest(UserENUM.UNABLE_CONFIRM_MAIL.ToString());
            }

            logger.Log(LogLevel.Info, "User with ID: " + userId + " confirmed email.\n");
            return Ok(new { message = UserENUM.ACCOUNT_ACTIVATED.ToString() });
        }
        #endregion

        #region lists

        // POST api/accounts/AddToFavourites
        [HttpPost]
        [Authorize]
        [Route("AddToFavourites")]
        public IHttpActionResult AddListToFavourites(ListAddToFavouriteTO listAddToFavouriteTO)
        {
            string identityUserName = HttpContext.Current.User.Identity.Name;

            bool listExists = _listService.CheckIfListExists(listAddToFavouriteTO.ListId);

            if (!listExists)
            {
                logger.Log(LogLevel.Warn, "List with id: " + listAddToFavouriteTO.ListId + "doesn't exist!\n");
                return BadRequest(ListENUM.LIST_NOT_FOUND.ToString());
            }

            bool favouriteExists = _authService.CheckIfFavouriteExists(listAddToFavouriteTO.ListId, identityUserName);

            if (!favouriteExists)
            {
                bool result = _authService.AddListToFavourites(identityUserName, listAddToFavouriteTO.ListId);
                if (result)
                {
                    logger.Log(LogLevel.Info, "User: " + identityUserName + " added list with ID: " + listAddToFavouriteTO.ListId + " to favourites.\n");
                    return Ok();
                }
                else
                {
                    logger.Log(LogLevel.Warn, "Unable to add list: " + listAddToFavouriteTO.ListId + " to favourites for user: " + identityUserName + "!\n");
                    return BadRequest(ListENUM.UNABLE_ADD_LIST_TO_FAVOURITES.ToString());
                }
            }
            else
            {
                logger.Log(LogLevel.Warn, "User: " + identityUserName + " has already added list with ID: " + listAddToFavouriteTO.ListId + " to favourites!\n");
                return BadRequest(ListENUM.LIST_ALREADY_MARKED_AS_FAVOURITE.ToString());
            }
        }

        // GET api/accounts/{userName}/favourites
        [HttpGet]
        [Authorize]
        [Route("{userName}/favourites")]
        public IHttpActionResult GetUserFavourites(string userName)
        {
            string identityUserName = HttpContext.Current.User.Identity.Name;
            if (identityUserName == userName)
            {
                var lists = _authService.GetUserFavourites(userName);

                return Ok(lists);
            }
            logger.Log(LogLevel.Error, "Unauthorized access for user: " + userName + ".\n");
            return Unauthorized();
        }

        [HttpGet]
        [Authorize]
        [Route("{userName}/created")]
        public IHttpActionResult GetUserCreatedLists(string userName)
        {
            string identityUserName = HttpContext.Current.User.Identity.Name;
            if (identityUserName == userName)
            {
                var lists = _authService.GetUserCreatedLists(userName);

                return Ok(lists);
            }
            logger.Log(LogLevel.Error, "Unauthorized access for user: " + userName + ".\n");
            return Unauthorized();
        }

        // POST api/accounts/removeFavourite/{listId}
        [HttpDelete]
        [Authorize]
        [Route("removeFavourite/{listId}")]
        public IHttpActionResult RemoveUserFavourite(int listId)
        {
            string identityUserName = HttpContext.Current.User.Identity.Name;

            var result = _authService.RemoveUserFavourite(listId, identityUserName);
            if (result)
                return StatusCode(HttpStatusCode.NoContent);

            return NotFound();
        }
        #endregion

        #region recover account
        // GET api/accounts/ForgotPassword
        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IHttpActionResult> ForgotPassword(ForgotPasswordTO model)
        {
            bool emailExists = _authService.CheckIfEmailExists(model.Email);

            if (emailExists)
            {
                bool result = await _authService.ForgotPassword(model.Email);

                if (!result)
                    return BadRequest(UserENUM.ACCOUNT_NOT_CONFIRMED.ToString());

                logger.Log(LogLevel.Info, "Reset password request was sent for user with email: " + model.Email + ".\n");
                return Ok();
            }

            return BadRequest(UserENUM.EMAIL_NOT_EXISTS.ToString());
        }

        // GET api/accounts/ResetPassword?userid=xxxx&code=yyyy
        [HttpGet]
        [Route("~/ResetPassword")]
        public async Task<IHttpActionResult> ResetPassword(string userId = "", string code = "")
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                return BadRequest(UserENUM.USER_ID_AND_CODE_REQUIRED.ToString());
            }

            bool result = await _authService.ResetPassword(userId, code);

            if (!result)
                return BadRequest(UserENUM.UNABLE_RESET_PASSWORD.ToString());

            logger.Log(LogLevel.Info, "Password was changed for user with ID: " + userId + ".\n");
            return Ok(new { message = UserENUM.PASSWORD_CHANGED.ToString() });
        }
        #endregion

        #region change user details
        // POST api/accounts/changeemail/
        [HttpPost]
        [Authorize]
        [Route("changeemail")]
        public IHttpActionResult ChangeEmail(ChangeEmailTO emailDetails)
        {
            string identityUserName = HttpContext.Current.User.Identity.Name;

            bool emailExists = _authService.CheckIfEmailExists(emailDetails.NewEmail);

            if (emailExists)
                return BadRequest(UserENUM.EMAIL_ALREADY_TAKEN.ToString());


            emailDetails.OldEmail = _authService.GetUserEmail(identityUserName);

            bool result = _authService.ChangeEmail(emailDetails);

            if (result)
            {
                logger.Log(LogLevel.Info, "Email was changed for user: " + identityUserName + ".\n");
                return Ok();
            }
            return BadRequest(UserENUM.UNABLE_CHANGE_EMAIL.ToString());
        }

        // POST api/accounts/changePassword/
        [HttpPost]
        [Authorize]
        [Route("changepassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordTO passwordDetails)
        {
            if (passwordDetails.NewPassword != passwordDetails.ConfirmNewPassword)
                return BadRequest(UserENUM.DIFFERENT_PASSWORDS.ToString());

            if (passwordDetails.NewPassword.Length < 5)
                return BadRequest(UserENUM.PASSWORD_TOO_SHORT.ToString());

            string identityUserName = HttpContext.Current.User.Identity.Name;
            passwordDetails.UserName = identityUserName;

            // Additional check if user session is not hijacked
            bool isAuth = await _authService.CheckCredentials(identityUserName, passwordDetails.CurrentPassword);
            if (!isAuth)
                return BadRequest(UserENUM.WRONG_PASSWORD.ToString());

            bool result = await _authService.ChangePassword(passwordDetails);

            if (result)
            {
                logger.Log(LogLevel.Info, "Password was changed for user: " + identityUserName + ".\n");
                return Ok();
            }
            return BadRequest(UserENUM.UNABLE_CHANGE_PASSWORD.ToString());
        }
        #endregion

        #region roles
        // POST api/accounts/AssignRolesToUser
        [Route("AssignRolesToUser")]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IHttpActionResult> AssignRolesToUser(AddRoleToUserTO rolesToAssign)
        {
            foreach (var item in rolesToAssign.Roles)
            {
                bool roleExists = await _authService.CheckIfRoleExists(item);
                if (!roleExists)
                {
                    logger.Log(LogLevel.Info, "Role: " + item + " doesn't exist!\n");
                    return BadRequest(UserENUM.ROLE_NOT_FOUND.ToString());
                }
            }

            var userRoles = _authService.GetUserRoles(rolesToAssign.UserName);

            foreach (var role in userRoles)
            {
                if (rolesToAssign.Roles.Contains(role))
                {
                    return BadRequest(UserENUM.ROLE_ALREADY_ADDED_FOR_USER.ToString());
                }
            }

            bool result = _authService.AssignRolesToUser(rolesToAssign);
            if (result)
            {
                foreach (var item in rolesToAssign.Roles)
                {
                    logger.Log(LogLevel.Info, "Role: " + item + " was added for user: " + rolesToAssign.UserName + ".\n");
                }

                return Ok(result);
            }

            return BadRequest(UserENUM.UNABLE_ADD_ROLE.ToString());
        }
        #endregion

        // DELETE api/account/deleteUser/{userName}
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        [Route("deleteUser/{userName}")]
        public async Task<IHttpActionResult> DeleteUser(string userName)
        {
            var user = await _authService.CheckIfUserExists(userName);
            if (!user)
            {
                logger.Log(LogLevel.Error, userName + "doesn't exists!");
                return NotFound();
            }

            var result = _authService.DeleteUser(userName);
            if (result)
            {
                logger.Log(LogLevel.Info, "User: " + userName + " was deleted.\n");
                return StatusCode(HttpStatusCode.NoContent);
            }

            logger.Log(LogLevel.Error, "Unable to delete user with userName: " + userName + ".\n");
            return BadRequest(UserENUM.UNABLE_DELETE_USER.ToString());
        }
    }
}
