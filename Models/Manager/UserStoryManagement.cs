using FindYourSoulMate.Models;
using FindYourSoulMateAngular.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindYourSoulMateAngular.Models.Manager
{
    public class UserStoryManagement
    {
        private DataContext dataContext;
        public UserStoryManagement()
        {
            dataContext = new DataContext();
        }

        public async Task insert_userStoryAsync(UserStory userStory)
        {
            var notification_collection = dataContext.getConnection().GetCollection<UserStory>("User_Story");
            await notification_collection.InsertOneAsync(userStory);
        }
    }
}
