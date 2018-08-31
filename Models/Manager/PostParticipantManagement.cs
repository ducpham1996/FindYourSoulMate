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
    public class PostParticipantManagement
    {
        private DataContext dataContext;
        Helper helper = new Helper();
        PostManagement pm = new PostManagement();
        NotificationManagement nm = new NotificationManagement();
        MailManager mm = new MailManager();
        public PostParticipantManagement()
        {
            dataContext = new DataContext();
            helper = new Helper();
            pm = new PostManagement();
            nm = new NotificationManagement();
            mm = new MailManager();
        }
        public async Task insertPostParticipantAsync(PostParticipant postParticipant)
        {
            var post_participant_collection = dataContext.getConnection().GetCollection<PostParticipant>("Post_Participant");
            await post_participant_collection.InsertOneAsync(postParticipant);
        }

        public async Task insertPostParticipantRecord(string post_id, Participant participant)
        {
            var post_participant_collection = dataContext.getConnection().GetCollection<PostParticipant>("Post_Participant");
            var filter = Builders<PostParticipant>.Filter.Eq("_id", ObjectId.Parse(post_id));
            var update = Builders<PostParticipant>.Update.AddToSet("participants", participant);
            await post_participant_collection.UpdateOneAsync(filter, update);
        }
        public bool has_participated(string post_id, string user_id)
        {
            var post_participant_collection = dataContext.getConnection().GetCollection<PostParticipant>("Post_Participant");
            var record = post_participant_collection.AsQueryable().Where(p => p._id == post_id).
SelectMany(c => c.participants).Where(p => p._id == user_id).FirstOrDefault();
            if (record == null)
            {
                return false;
            }
            return true;
        }
        public async Task send_email_to_participant_post_comment(string post_id, Owner user, int activity)
        {
            var post_participant_collection = dataContext.getConnection().GetCollection<PostParticipant>("Post_Participant");
            var record = post_participant_collection.AsQueryable().Where(p => p._id == post_id).
SelectMany(c => c.participants).Where(p => p._id != user._id).Where(p2 => p2.status == true).ToArray();
            Post post = pm.getPost(post_id);
            string link = "/Users/Profile/" + helper.EncodeTo64(post.owner._id) + "?p_ref=" + helper.EncodeTo64(post_id);
            foreach (Participant p in record)
            {
                mm.sendEmail(p.email, user.user_name, user.user_name + " commented about the post" + "<br/>" +
                    "<a href='" + link + "'>Click here to see</a>");
                await nm.insert_notification_recordAsync(p._id, new Notification
                {
                    _id = ObjectId.GenerateNewId().ToString(),
                    time_sent = DateTime.Now,
                    sender_id = user._id,
                    description = "comment about the post",
                    activity = activity,
                    url = link
                });
            }
        }

        public async Task send_email_to_participant_sub_post_comment(string post_id, string sub_post_id, Owner user, int activity)
        {
            var post_participant_collection = dataContext.getConnection().GetCollection<PostParticipant>("Post_Participant");
            var record = post_participant_collection.AsQueryable().Where(p => p._id == post_id).
SelectMany(c => c.participants).Where(p => p._id != user._id).Where(p2 => p2.status == true).ToArray();
            Post mainPost = pm.getPost(post_id);
            Post post = pm.getSubPost(post_id, sub_post_id);
            string link = "/Users/Profile/" + helper.EncodeTo64(mainPost.owner._id) + "?p_ref=" + helper.EncodeTo64(post_id) + "&sp_ref=" + helper.EncodeTo64(sub_post_id);
            foreach (Participant p in record)
            {
                mm.sendEmail(p.email, user.user_name, user.user_name + " commented about the post" + "<br/>" +
                    "<a href='" + link + "'>Click here to see</a>");
                await nm.insert_notification_recordAsync(p._id, new Notification
                {
                    _id = ObjectId.GenerateNewId().ToString(),
                    time_sent = DateTime.Now,
                    sender_id = user._id,
                    description = "comment about the post",
                    activity = activity,
                    url = link
                });
            }
        }
        public async Task send_email_to_participant_post_like(string post_id, Owner user, int activity)
        {
            var post_participant_collection = dataContext.getConnection().GetCollection<PostParticipant>("Post_Participant");
            var record = post_participant_collection.AsQueryable().Where(p => p._id == post_id).
SelectMany(c => c.participants).Where(p => p._id != user._id).Where(p2 => p2.status == true).ToArray();
            Post mainPost = pm.getPost(post_id);
            string link = "/Users/Profile/" + helper.EncodeTo64(mainPost.owner._id) + "?p_ref=" + helper.EncodeTo64(post_id);
            foreach (Participant p in record)
            {
                mm.sendEmail(p.email, user.user_name, user.user_name + " commented about the post" + "<br/>" +
                    "<a href='" + link + "'>Click here to see</a>");
                await nm.insert_notification_recordAsync(p._id, new Notification
                {
                    _id = ObjectId.GenerateNewId().ToString(),
                    time_sent = DateTime.Now,
                    sender_id = user._id,
                    description = "comment about the post",
                    activity = activity,
                    url = link
                });
            }
        }
    }
}
