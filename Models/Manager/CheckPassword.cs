using FindYourSoulMate.Models.Entities;
using FindYourSoulMate.Models.Exception;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace FindYourSoulMate.Models
{
    public class CheckPassword
    {
        private DataContext data;
        public CheckPassword()
        {
            data = new DataContext();
        }

        public User_Authorization check_password(string username,string password)
        {
            var authorization = data.getConnection().GetCollection<User_Authorization>("User");
            var user = authorization.Find(x => x.authorization.username == username).SingleAsync();
            User_Authorization au;
            try
            {
               au  = user.Result;
            }
            catch(AggregateException ae)
            {
                throw new AccountIsNotExistException("Your account is not exist");
            }
            byte[] salt = au.authorization.salt;
            byte[] key = au.authorization.key;
            // load salt and key from database
            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt))
            {
                byte[] newKey = deriveBytes.GetBytes(20);  // derive a 20-byte key

                if (!newKey.SequenceEqual(key))
                {
                    throw new InvalidUserNameAndPasswordException("Username or Password is invalid!");
                }
                else
                {
                    return au;
                }
            }
        }
    }
}
