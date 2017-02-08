using AutoMapper;
using DataModel.Models;
using DataModel.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using TO.List;
using TO.User;

namespace Service
{
    public class AuthServices : IAuthServices
    {
        private ListRepository _listRepository = new ListRepository(false);
        private AuthRepository _authRepository = new AuthRepository(false);

        public void InitializeUsersCache()
        {
            ApplicationCache<UserInfoTO>.FillCache(GetAllUsers());
        }

        #region Register user & confirm email
        public async Task<bool> RegisterUser(RegisterUserTO registerUserTO)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var userModel = new ApplicationUser
                {
                    UserName = registerUserTO.UserName,
                    Email = registerUserTO.Email
                };

                var resultUserId = await _authRepository.RegisterUser(userModel, registerUserTO.Password);
                if (resultUserId != "")
                {
                    scope.Complete();

                    ApplicationCache<UserInfoTO>.AddCacheItem(new UserInfoTO()
                    {
                        Id = resultUserId,
                        UserName = registerUserTO.UserName,
                        Email = registerUserTO.Email,
                        Roles = new List<string>()
                    });

                    return true;
                }

                return false;
            }
        }

        public async Task<bool> ConfirmEmail(string userId, string code)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var result = await _authRepository.ConfirmEmail(userId, code);

                if (result)
                {
                    scope.Complete();
                    return true;
                }

                return false;
            }
        }
        #endregion

        #region Account recovery
        public async Task<bool> ForgotPassword(string userEmail)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var result = await _authRepository.ForgotPassword(userEmail);

                if (result)
                {
                    scope.Complete();
                    return true;
                }

                return false;
            }
        }

        public async Task<bool> ResetPassword(string userId, string code)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var result = await _authRepository.ResetPassword(userId, code);

                if (result)
                {
                    scope.Complete();
                    return true;
                }

                return false;
            }
        }
        #endregion

        #region change user email & password
        public bool ChangeEmail(ChangeEmailTO emailDetails)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var result = _authRepository.ChangeEmail(emailDetails.OldEmail, emailDetails.NewEmail);

                if (result)
                {
                    scope.Complete();
                    var cacheItem = ApplicationCache<UserInfoTO>.GetCacheItem(d => d.Email == emailDetails.OldEmail);

                    if (cacheItem != null)
                    {
                        var newCacheItem = new UserInfoTO()
                        {
                            Id = cacheItem.Id,
                            Email = emailDetails.NewEmail,
                            UserName = cacheItem.UserName,
                            Roles = cacheItem.Roles
                        };

                        ApplicationCache<UserInfoTO>.RemoveCacheItem(cacheItem);
                        ApplicationCache<UserInfoTO>.AddCacheItem(newCacheItem);
                    }

                    return true;
                }

                return false;
            }
        }

        public async Task<bool> ChangePassword(ChangePasswordTO passwordDetails)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var result = await _authRepository.ChangePassword(passwordDetails.UserName, passwordDetails.CurrentPassword, passwordDetails.NewPassword);

                if (result)
                {
                    scope.Complete();
                    return true;
                }

                return false;
            }
        }
        #endregion

        #region Operations on user lists
        public bool CheckIfFavouriteExists(int listId, string userName)
        {
            var cacheItem = ApplicationCache<UserFavouriteListIdsTO>.GetCacheItem(d => d.UserName == userName);
            if (cacheItem != null)
            {
                bool listExists = cacheItem.ListIds.Contains(listId);
                if (listExists == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            var userId = _authRepository.GetUserId(userName);

            return _authRepository.CheckIfFavouriteExists(listId, userId);
        }

        public bool AddListToFavourites(string userName, int listId)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (userName != "")
                {
                    var result = _authRepository.AddListToFavourites(userName, listId);

                    if (result)
                    {
                        scope.Complete();

                        var cache = ApplicationCache<UserFavouriteListIdsTO>.GetCache();
                        if (cache != null)
                        {
                            var cacheItem = ApplicationCache<UserFavouriteListIdsTO>.GetCacheItem(d => d.UserName == userName);

                            if (cacheItem != null)
                            {
                                var cacheItemIndex = cache.FindIndex(d => d.UserName == userName);
                                cacheItem.ListIds.Add(listId);
                                cache[cacheItemIndex] = cacheItem;
                                ApplicationCache<UserFavouriteListIdsTO>.FillCache(cache);
                            }
                            else
                            {
                                var newUserFavs = new UserFavouriteListIdsTO()
                                {
                                    UserName = userName,
                                    ListIds = new List<int>()

                                };
                                newUserFavs.ListIds.Add(listId);

                                ApplicationCache<UserFavouriteListIdsTO>.AddCacheItem(newUserFavs);
                            }
                        }

                        return true;
                    }

                    return false;
                }
                return false;
            }
        }

        public List<ListFavouriteTO> GetUserFavourites(string userName)
        {
            var listIds = new List<int>();

            var cacheItem = ApplicationCache<UserFavouriteListIdsTO>.GetCacheItem(d => d.UserName == userName);

            if (cacheItem != null)
            {
                listIds = cacheItem.ListIds;
            }
            else
            {
                listIds = _authRepository.GetUserFavourites(userName);
                ApplicationCache<UserFavouriteListIdsTO>.AddCacheItem(new UserFavouriteListIdsTO()
                {
                    UserName = userName,
                    ListIds = listIds
                });
            }

            List<ListFavouriteTO> listsModel = new List<ListFavouriteTO>();
            if (listIds.Count > 0)
            {
                var lists = _listRepository.GetMany(listIds);

                if (lists != null)
                {
                    Mapper.CreateMap<List, ListFavouriteTO>();
                    listsModel = Mapper.Map<List<List>, List<ListFavouriteTO>>(lists);
                }
            }

            return listsModel;
        }

        public List<ListUserCreated> GetUserCreatedLists(string userName)
        {
            var listsModel = new List<ListUserCreated>();

            var cacheItems = ApplicationCache<ListDescriptionTO>.GetCacheItems(d => d.User.UserName == userName);
            if (cacheItems.Count > 0)
            {
                Mapper.CreateMap<ListDescriptionTO, ListUserCreated>();
                listsModel = Mapper.Map<List<ListDescriptionTO>, List<ListUserCreated>>(cacheItems);
            }
            else
            {
                var lists = _authRepository.GetUserCreatedLists(userName);

                if (lists != null)
                {
                    Mapper.CreateMap<List, ListUserCreated>();
                    listsModel = Mapper.Map<List<List>, List<ListUserCreated>>(lists);
                }
            }

            return listsModel;
        }

        public bool RemoveUserFavourite(int listId, string userName)
        {
            bool status = _authRepository.RemoveUserFavourite(listId, userName);
            if (status == true)
            {
                var cache = ApplicationCache<UserFavouriteListIdsTO>.GetCache();
                if (cache.Count > 0)
                {
                    var cacheItem = ApplicationCache<UserFavouriteListIdsTO>.GetCacheItem(d => d.UserName == userName);
                    if (cacheItem != null)
                    {
                        int cacheItemIndex = cache.FindIndex(d => d.UserName == userName);
                        cacheItem.ListIds.Remove(listId);
                        cache[cacheItemIndex] = cacheItem;
                        ApplicationCache<UserFavouriteListIdsTO>.FillCache(cache);
                    }
                }

                return true;
            }
            return false;
        }
        #endregion

        #region Roles
        public bool AssignRolesToUser(AddRoleToUserTO rolesToAssign)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var result = _authRepository.AssignRolesToUser(rolesToAssign.UserName, rolesToAssign.Roles);

                if (result)
                {
                    scope.Complete();
                    return true;
                }

                return false;
            }
        }

        public List<string> GetUserRoles(string userName)
        {
            var result = _authRepository.GetUserRoles(userName);

            return result;
        }

        public List<string> GetUserRolesByUserId(string userId)
        {
            var result = _authRepository.GetUserRolesByUserId(userId);

            return result;
        }
        #endregion

        #region User info / Find user / Get all users / delete user
        public UserInfoTO GetUserInfo(string userName)
        {
            var cacheItem = ApplicationCache<UserInfoTO>.GetCacheItem(d => d.UserName == userName);

            if (cacheItem != null)
            {
                return cacheItem;
            }

            var user = _authRepository.FindUserByUserName(userName);

            if (user != null)
            {
                var userRoles = _authRepository.GetUserRoles(user.UserName);

                UserInfoTO userInfo = new UserInfoTO
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = userRoles
                };

                ApplicationCache<UserInfoTO>.AddCacheItem(userInfo);
                return userInfo;
            }
            return null;
        }

        public string GetUserId(string userName)
        {
            var cacheItem = ApplicationCache<UserInfoTO>.GetCacheItem(d => d.UserName == userName);
            if (cacheItem != null)
            {
                return cacheItem.Id;
            }

            return _authRepository.GetUserId(userName);
        }

        public string GetUserEmail(string userName)
        {
            var cacheItem = ApplicationCache<UserInfoTO>.GetCacheItem(d => d.UserName == userName);
            if (cacheItem != null)
            {
                return cacheItem.Email;
            }

            return _authRepository.GetUserEmail(userName);
        }

        // TO-DO sprobowac zaimplementowac cache dla logowania
        // Find user method for SimpleAuthorizationServerProvider
        public async Task<string> FindUser(string userName, string password)
        {
            var userIdentity = await _authRepository.FindUser(userName, password);
            if (userIdentity != null)
            {
                return userIdentity.Id;
            }
            return "";
        }

        public List<UserInfoTO> GetAllUsers()
        {
            var cache = ApplicationCache<UserInfoTO>.GetCache();
            if (cache.Count > 0)
            {
                return cache;
            }

            List<ApplicationUser> users = _authRepository.GetAllUsers();

            List<UserInfoTO> usersModel = new List<UserInfoTO>();

            if (users != null)
            {
                Mapper.CreateMap<ApplicationUser, UserInfoTO>();
                usersModel = Mapper.Map<List<ApplicationUser>, List<UserInfoTO>>(users);
            }

            foreach (var user in usersModel)
            {
                user.Roles = _authRepository.GetUserRoles(user.UserName);
            }

            ApplicationCache<UserInfoTO>.FillCache(usersModel);
            return usersModel;
        }

        public List<UserIdNameTO> GetAllUsersNames()
        {
            List<UserIdNameTO> usersModel = new List<UserIdNameTO>();
            var cache = ApplicationCache<UserInfoTO>.GetCache();
            if (cache.Count > 0)
            {
                Mapper.CreateMap<UserInfoTO, UserIdNameTO>();
                usersModel = Mapper.Map<List<UserInfoTO>, List<UserIdNameTO>>(cache);

                return usersModel;
            }

            List<ApplicationUser> users = _authRepository.GetAllUsers();
            Mapper.CreateMap<ApplicationUser, UserIdNameTO>();
            usersModel = Mapper.Map<List<ApplicationUser>, List<UserIdNameTO>>(users);

            return usersModel;

        }

        public bool DeleteUser(string userName)
        {
            var result = _authRepository.DeleteUser(userName);

            if (result)
            {
                var cacheItem = ApplicationCache<UserInfoTO>.GetCacheItem(d => d.UserName == userName);
                if (cacheItem != null)
                {
                    ApplicationCache<UserInfoTO>.RemoveCacheItem(cacheItem);
                }
                return true;
            }

            return false;
        }

        public async Task<bool> CheckCredentials(string userName, string password)
        {
            var result = await _authRepository.CheckCredentials(userName, password);
            return (result) ? true : false;
        }

        public async Task<bool> CheckIfRoleExists(string role)
        {
            var result = await _authRepository.CheckIfRoleExists(role);
            return (result) ? true : false;
        }

        public async Task<bool> CheckIfUserExists(string userName)
        {
            var cacheItem = ApplicationCache<UserInfoTO>.GetCacheItem(d => d.UserName == userName);
            if (cacheItem != null)
            {
                return true;
            }

            var result = await _authRepository.CheckIfUserExists(userName);

            return (result) ? true : false;
        }

        public bool CheckIfEmailExists(string email)
        {
            var cacheItem = ApplicationCache<UserInfoTO>.GetCacheItem(d => d.Email == email);
            if (cacheItem != null)
            {
                return true;
            }
            return _authRepository.CheckIfEmailExists(email);
        }
        #endregion
    }
}
