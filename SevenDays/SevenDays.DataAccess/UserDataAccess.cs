﻿using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Configuration;
using SevenDays.Entities;
using SevenDays.Util;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SevenDays.DataAccess
{
    public class UserDataAccess
    {

        private readonly IConfiguration Configuration;

        public UserDataAccess(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Get the user with a specific email
        /// </summary>
        /// <param name="idUser">ID User</param>
        /// <returns>Result object</returns>
        public DBResult<User> GetUser(string email)
        {
            DBResult<User> dbResult = new DBResult<User>();

            try
            {
                using (SevenDaysContext db = new SevenDaysContext(Configuration))
                {
                    var user = db.User.Where(u => u.Email == email).FirstOrDefault();
                    if (user == null)
                    {
                        dbResult.Message = "User not found";
                    }
                    else
                    {
                        dbResult.Success = true;
                        dbResult.Item = user;
                    }
                }
            }
            catch(Exception ex)
            {
                dbResult.Message = Common.GetMessageError(ex);
            }
           

            return dbResult;
        }

        /// <summary>
        /// Get a user by Id
        /// </summary>
        /// <param name="idUser">ID User</param>
        /// <returns>Result object</returns>
        public DBResult<User> GetUserById(int idUser)
        {
            DBResult<User> dbResult = new DBResult<User>();

            try
            {
                using (SevenDaysContext db = new SevenDaysContext(Configuration))
                {
                    var user = db.User.Find(idUser);
                    if (user == null)
                    {
                        dbResult.Message = "User not found";
                    }
                    else
                    {
                        dbResult.Success = true;
                        dbResult.Item = user;
                    }
                }
            }
            catch (Exception ex)
            {
                dbResult.Message = Common.GetMessageError(ex);
            }

            return dbResult;
        }

        /// <summary>
        /// Createing a new user
        /// </summary>
        /// <param name="user">User object</param>
        /// <returns>Result object</returns>
        public DBResult<User> CreateUser(User user)
        {
            DBResult<User> dbResult = new DBResult<User>();

            if (user == null)
            {
                dbResult.Message = "Invalid user";
                return dbResult;
            }
            try
            {
                using (SevenDaysContext db = new SevenDaysContext(Configuration))
                {
                    db.User.Add(user);
                    db.SaveChanges();
                    dbResult.Success = true;
                    // Get user with ID
                    user = db.User.Where(u => u.Email == user.Email).FirstOrDefault();
                    dbResult.Item = user;
                }                
            }
            catch (Exception ex)
            {
                dbResult.Message = Common.GetMessageError(ex);
            }


            return dbResult;
        }

        /// <summary>
        /// Patch User
        /// </summary>
        /// <param name="patchUser">patch document</param>
        /// <param name="idUser">Id User</param>
        /// <returns>Result object</returns>
        public async Task<DBResult<User>> PatchUser(JsonPatchDocument<User> patchUser, int idUser)
        {
            DBResult<User> dbResult = new DBResult<User>();

            try
            {
                using (SevenDaysContext db = new SevenDaysContext(Configuration))
                {
                    // Get our original object from the database.
                    var user = await db.User.FindAsync(idUser);

                    if (user == null)
                    {
                        dbResult.Message = "Movie not found";
                        return dbResult;
                    }

                    // Applying Path to DB Object
                    patchUser.ApplyTo(user);
                    db.User.Update(user);
                    await db.SaveChangesAsync();
                    dbResult.Item = user;
                    dbResult.Success = true;
                }
            }
            catch (Exception ex)
            {
                dbResult.Message = Common.GetMessageError(ex);
            }


            return dbResult;
        }

    }
}
