using FindYourSoulMate.Models.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FindYourSoulMate.Models.Manager
{
    public class NotificationManagement
    {
        private DataContext dataContext;
        private UserManagement um;
        public NotificationManagement()
        {
            dataContext = new DataContext();
            um = new UserManagement();
        }
        public async Task insert_notificationAsync(Notification_Record notification)
        {
            var notification_collection = dataContext.getConnection().GetCollection<Notification_Record>("Notification");
            await notification_collection.InsertOneAsync(notification);
        }

        public async Task insert_notification_recordAsync(string user_id, Notification notification)
        {
            var notification_collection = dataContext.getConnection().GetCollection<Notification_Record>("Notification");
            var filter = Builders<Notification_Record>.Filter.Eq("_id", ObjectId.Parse(user_id));
            var update = Builders<Notification_Record>.Update.AddToSet("notification", notification);
            var record = notification_collection.AsQueryable().Where(p => p._id == user_id).FirstOrDefault();
            if (record == null)
            {
               await insert_notificationAsync(new Notification_Record { _id = user_id, notifications = new List<Notification>() });
            }
            await notification_collection.UpdateOneAsync(filter, update);
        }

        public Notification_Record get_notifications(string user_id)
        {
            var notification_collection = dataContext.getConnection().GetCollection<Notification_Record>("Notification");
            var filter = Builders<Notification_Record>.Filter.Eq("_id", ObjectId.Parse(user_id));
            Notification_Record records = notification_collection.Find(filter).SingleOrDefault();
            foreach (Notification n in records.notifications)
            {
                User u = um.GetUser_Detail(n.sender_id);
                n.sender_name = u.first_name + " " + u.last_name;
                n.sender_pic = u.profile_img;
            }
            return records;
        }
    }
}
