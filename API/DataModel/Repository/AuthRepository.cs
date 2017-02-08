using DataModel.Helpers;
using DataModel.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
// additional packages
// Microsoft.Owin.Security
// Microsoft.AspNet.Identity.Owin

using NLog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace DataModel.Repository
{
    public class AuthRepository : IAuthRepository
    {
        internal WebApiDbEntities _context;
        string _connectionString;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private UserManager<ApplicationUser> _userManager;
        private MachineKeyProtectionProvider _provider;
        private RoleManager<IdentityRole> _roleManager;

        public AuthRepository(bool isTest)
        {
            _context = new WebApiDbEntities();
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context));
            _roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_context));
            _provider = new MachineKeyProtectionProvider();
            _userManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(_provider.Create("EmailConfirmation"));

            if (isTest)
                _connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=DB_A15604_sportoweswiry;Integrated Security=True;";
            else
                _connectionString = @"Data Source=SQL5025.SmarterASP.NET;Initial Catalog=DB_A15604_sportoweswiry;User Id=DB_A15604_sportoweswiry_admin;Password=haslo123;";
        }

        #region Register user & confirm email

        public async Task<string> RegisterUser(ApplicationUser applicationUser, string password)
        {
            var result = new IdentityResult();

            try
            {
                // check if incorrect data causes Exception or result = failed
                result = await _userManager.CreateAsync(applicationUser, password);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, "Unable create user: " + ex);
                return "";
            }

            var foundUserId = "";

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("select Id from AspNetUsers where Email='" + applicationUser.Email + "'", con);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        foundUserId = (string)rdr["Id"];
                    }
                }
            }

            if (foundUserId == "")
            {
                logger.Log(LogLevel.Error, "Unable to find user with email: " + applicationUser.Email);
                return "";
            }

            string code;

            if (result.Succeeded && foundUserId != "")
            {
                try
                {
                    code = await _userManager.GenerateEmailConfirmationTokenAsync(foundUserId);
                }
                catch (Exception ex)
                {
                    logger.Log(LogLevel.Error, "Unable generate activation code: " + ex);
                    return "";
                }

                //ConfirmEmail?userid=xxxx&code=xxxx
                string url;
                url = "http://moodies.pl/ConfirmEmail?userid=" + foundUserId + "&code=" + HttpUtility.UrlEncode(code);

                string email = applicationUser.Email;

                SendEmail("Aktywuj swoje konto na moodies.pl",
                      "Witaj " + applicationUser.UserName + ". Prosze potwierdz rejestracje klikajac w ten link: " + url,
                      email);
            }

            return foundUserId;
        }

        public async Task<bool> ConfirmEmail(string userId, string code)
        {
            var result = await _userManager.ConfirmEmailAsync(userId, code);

            return (result.Succeeded) ? true : false;
        }

        #endregion

        #region Account recovery
        public async Task<bool> ForgotPassword(string email)
        {
            var foundUser = _userManager.FindByEmail(email);

            if (foundUser != null && (await _userManager.IsEmailConfirmedAsync(foundUser.Id)))
            {
                var code = await _userManager.GeneratePasswordResetTokenAsync(foundUser.Id);
                string url;
                url = "http://moodies.pl/ResetPassword?userid=" + foundUser.Id + "&code=" + HttpUtility.UrlEncode(code);

                SendEmail("Odzyskiwanie hasla dla konta moodies.pl",
                          "Witaj " + foundUser.UserName + ". Aby zresetować twoje hasło kliknij tutaj: " + url,
                          foundUser.Email);

                return true;
            }
            return false;
        }

        public async Task<bool> ResetPassword(string userId, string code)
        {
            var foundUser = await _userManager.FindByIdAsync(userId);

            if (foundUser != null)
            {
                Random random = new Random();
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                string newPassword = new string(Enumerable.Repeat(chars, 6)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                string hashedNewPassword = _userManager.PasswordHasher.HashPassword(newPassword);
                foundUser.PasswordHash = hashedNewPassword;
                var result = await _userManager.UpdateAsync(foundUser);

                if (result.Succeeded)
                {
                    SendEmail("Twoje nowe haslo do konta moodies.pl",
          "Witaj " + foundUser.UserName + ". Twoje nowe haslo to: " + newPassword,
          foundUser.Email);

                    return true;
                }
            }
            return false;
        }
        #endregion

        #region change user email & password

        public bool ChangeEmail(string oldEmail, string newEmail)
        {
            int result = 0;
            string query = "update AspNetUsers set Email=@newEmail where Email=@oldEmail";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@oldEmail", oldEmail);
                cmd.Parameters.AddWithValue("@newEmail", newEmail);
                result = cmd.ExecuteNonQuery();
            }

            if (result > 0)
                return true;

            return false;
        }

        // TO-DO
        public async Task<bool> ChangePassword(string userName, string currentPassword, string newPassword)
        {
            var userId = GetUserId(userName);

            var result = await _userManager.ChangePasswordAsync(userId, currentPassword, newPassword);

            if (result.Succeeded)
                return true;

            return false;
        }
        #endregion

        #region Operations on user lists
        public bool CheckIfFavouriteExists(int listId, string userId)
        {
            int exists = 0;
            var query = "select Id from dbo.UserFavourite where ListId=" + listId + " and User_Id='" + userId + "'";

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        exists = (int)rdr["Id"];
                    }
                }
            }

            if (exists != 0)
                return true;

            return false;
        }

        public bool AddListToFavourites(string userName, int listId)
        {
            var userId = GetUserId(userName);
            int result = 0;
            string query = "insert into dbo.UserFavourite(User_Id, ListId) values (@userId, @listId)";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@listId", listId);
                cmd.Parameters.AddWithValue("@userId", userId);
                result = cmd.ExecuteNonQuery();
            }

            if (result > 0)
                return true;

            return false;
        }

        public List<int> GetUserFavourites(string userName)
        {
            List<int> listIds = new List<int>();

            var query = "select ListId from UserFavourite where User_Id=(select Id from AspNetUsers where UserName=@userName)";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@userName", userName);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        listIds.Add((int)rdr["ListId"]);
                    }
                }
            }

            return listIds;
        }

        public List<List> GetUserCreatedLists(string userName)
        {
            List<List> userCreatedLists = new List<List>();
            var query = "select * from List where User_Id=(select Id from AspNetUsers where UserName=@userName)";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@userName", userName);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        userCreatedLists.Add(new List()
                        {
                            Id = (int)rdr["Id"],
                            Name = (string)rdr["Name"]
                        });
                    }
                }
            }

            return userCreatedLists;
        }

        public bool RemoveUserFavourite(int listId, string userName)
        {
            var userId = GetUserId(userName);
            int result = 0;
            string query = "delete from dbo.UserFavourite where User_Id=@userId and ListId=@listId";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@listId", listId);
                cmd.Parameters.AddWithValue("@userId", userId);
                result = cmd.ExecuteNonQuery();
            }

            if (result > 0)
                return true;

            return false;
        }
        #endregion

        #region Roles
        // TO-DO
        public bool AssignRolesToUser(string userName, List<string> rolesToAssign)
        {
            List<bool> results = new List<bool>();

            var foundUser = _userManager.FindByName(userName);

            if (foundUser != null)
            {
                foreach (var item in rolesToAssign)
                {
                    IdentityResult result = _userManager.AddToRoles(foundUser.Id, item);
                    if (result.Succeeded)
                        results.Add(true);
                }

                foreach (var result in results)
                {
                    if (result == false)
                        return false;
                }

                return true;
            }
            return false;
        }

        public List<string> GetUserRoles(string userName)
        {
            var userId = GetUserId(userName);

            List<string> userRoles = new List<string>();
            var query = "select b.Name from AspNetRoles b JOIN AspNetUserRoles a ON b.Id=a.RoleId where UserId=@userId";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@userId", userId);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        userRoles.Add((string)rdr["Name"]);
                    }
                }
            }


            return userRoles;
        }

        public List<string> GetUserRolesByUserId(string userId)
        {
            List<string> userRoles = new List<string>();
            var query = "select b.Name from AspNetRoles b JOIN AspNetUserRoles a ON b.Id=a.RoleId where UserId=@userId";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@userId", userId);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        userRoles.Add((string)rdr["Name"]);
                    }
                }
            }

            return userRoles;
        }
        #endregion

        #region User info / Find user / Get all users / delete user
        public bool DeleteUser(string userName)
        {
            var userId = GetUserId(userName);

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                SqlCommand deleteClaimsCmd = new SqlCommand("delete from AspNetUserClaims where UserId=@userId", con);
                deleteClaimsCmd.Parameters.AddWithValue("@userId", userId);
                deleteClaimsCmd.ExecuteNonQuery();

                SqlCommand deleteUserLoginsCmd = new SqlCommand("delete from AspNetUserLogins where UserId=@userId", con);
                deleteUserLoginsCmd.Parameters.AddWithValue("@userId", userId);
                deleteUserLoginsCmd.ExecuteNonQuery();

                SqlCommand deleteUserRolesCmd = new SqlCommand("delete from AspNetUserRoles where UserId=@userId", con);
                deleteUserRolesCmd.Parameters.AddWithValue("@userId", userId);
                deleteUserRolesCmd.ExecuteNonQuery();

                SqlCommand deleteUserFavsCmd = new SqlCommand("delete from UserFavourite where User_Id=@userId", con);
                deleteUserFavsCmd.Parameters.AddWithValue("@userId", userId);
                deleteUserFavsCmd.ExecuteNonQuery();

                SqlCommand deleteUserRatingsCmd = new SqlCommand("delete from Rating where User_Id=@userId", con);
                deleteUserRatingsCmd.Parameters.AddWithValue("@userId", userId);
                deleteUserRatingsCmd.ExecuteNonQuery();


                List<int> listIds = new List<int>();

                SqlCommand selectListsIdCmd = new SqlCommand("select Id from List where User_Id='" + userId + "'", con);
                using (SqlDataReader rdr = selectListsIdCmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        listIds.Add((int)rdr["Id"]);
                    }
                }

                string ids = "";
                int result = 0;
                if (listIds.Count > 0)
                {
                    foreach (var id in listIds)
                    {
                        ids += id;
                        ids += ",";
                    }
                    //remove last comma
                    ids = ids.Remove(ids.Length - 1);

                    SqlCommand deleteUserListFilmsCmd = new SqlCommand("delete from ListFilm where List_Id IN (" + ids + ")", con);
                    deleteUserListFilmsCmd.ExecuteNonQuery();
                    SqlCommand deleteUserListsCmd = new SqlCommand("delete from List where User_Id='" + userId + "'", con);
                    deleteUserListsCmd.ExecuteNonQuery();
                }

                SqlCommand deleteUserCmd = new SqlCommand("delete from AspNetUsers where UserName='" + userName + "'", con);
                result = deleteUserCmd.ExecuteNonQuery();
                if (result > 0)
                    return true;

                return false;
            }
        }

        // TO-DO
        public ApplicationUser FindUserByUserName(string userName)
        {
            var foundUser = _userManager.FindByName(userName);

            if (foundUser != null)
            {
                return foundUser;
            }
            return null;
        }

        public string GetUserId(string userName)
        {
            string userId = "";
            var query = "select Id from AspNetUsers where UserName=@userName";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@userName", userName);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        userId = (string)rdr["Id"];
                    }
                }
            }

            return userId;
        }

        public string GetUserEmail(string userName)
        {
            string email = "";
            var query = "select Email from AspNetUsers where UserName=@userName";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@userName", userName);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        email = (string)rdr["Email"];
                    }
                }
            }

            return email;
        }

        // TO-DO
        public async Task<IdentityUser> FindUser(string userName, string password)
        {
            IdentityUser user = await _userManager.FindAsync(userName, password);

            return user;
        }

        public List<ApplicationUser> GetAllUsers()
        {
            List<ApplicationUser> users = new List<ApplicationUser>();

            var query = "select Id, UserName, Email from AspnetUsers";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        users.Add(new ApplicationUser()
                        {
                            Id = (string)rdr["Id"],
                            UserName = (string)rdr["UserName"],
                            Email = (string)rdr["Email"]
                        });
                    }
                }
            }

            return users;
        }

        // TO-DO
        public async Task<bool> CheckCredentials(string userName, string password)
        {
            IdentityUser user = await _userManager.FindAsync(userName, password);
            return (user != null) ? true : false;
        }

        // TO-DO
        public async Task<bool> CheckIfRoleExists(string role)
        {
            var result = await _roleManager.FindByNameAsync(role);
            return (result != null) ? true : false;
        }

        // TO-DO
        public async Task<bool> CheckIfUserExists(string userName)
        {
            var result = await _userManager.FindByNameAsync(userName);
            return (result != null) ? true : false;
        }

        public bool CheckIfEmailExists(string email)
        {
            string exists = "";
            var query = "select Email from AspNetUsers where Email=@email";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@email", email);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        exists = (string)rdr["Email"];
                    }
                }
            }

            if (exists != "")
                return true;

            return false;
        }
        #endregion

        public void SendEmail(string subject, string emailBody, string email)
        {
            string fromEmail = "noreply@moodies.pl";
            string body = emailBody;
            MailMessage mailMessage = new MailMessage(fromEmail, email, subject, body);
            SmtpClient smtpClient = new SmtpClient("smtp.forpsi.com", 587);
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(fromEmail, "mkultest");
            try
            {
                smtpClient.Send(mailMessage);
                logger.Log(LogLevel.Info, "Activation mail was sent to: " + email);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, "Unable to send activation mail: " + ex);
            }
        }
    }
}
