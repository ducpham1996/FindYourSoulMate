using FindYourSoulMate.Models;
using FindYourSoulMate.Models.Entities;
using FindYourSoulMate.Models.Manager;
using FindYourSoulMateAngular.Controllers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace FindYourSoulMateAngular.Models.Manager
{
    public class RegistraionManagement
    {
        private DataContext dataContext;
        public RegistraionManagement()
        {
            dataContext = new DataContext();
        }
        public Boolean checkEmailIsExist(string email)
        {
            var users = dataContext.getConnection().GetCollection<User_Authorization>("User");
            var check_user = users.Find(x => x.authorization.username == email).SingleAsync();
            try
            {
                User_Authorization au = check_user.Result;
                return true;
            }
            catch (AggregateException ae)
            {
                return false;
            }
        }
        public async Task registrationAsync(User_Authorization user_auth)
        {
            NotificationManagement nm = new NotificationManagement();
            UserStoryManagement usm = new UserStoryManagement();
            var users = dataContext.getConnection().GetCollection<User_Authorization>("User");
            await users.InsertOneAsync(user_auth);
            await nm.insert_notificationAsync(new Notification_Record { _id = user_auth._id.ToString(),notifications = new List<Notification>() });
            await usm.insert_userStoryAsync(new Entities.UserStory { _id = user_auth._id.ToString(), user_stories = new List<Entities.Story>() });
        }
    }
}
