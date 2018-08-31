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

    public class PostLikeManagement
    {
        private DataContext dataContext;
        public async Task insertPostLike(Post_Like post_Like)
        {
            dataContext = new DataContext();
            var commment_like_collection = dataContext.getConnection().GetCollection<Post_Like>("Post_Like");
            await commment_like_collection.InsertOneAsync(post_Like);
            Debug.WriteLine("Inserted post like: " + post_Like.post_id);
        }
        public bool has_like(string post_id, Like_Record like_Record)
        {
            dataContext = new DataContext();
            var post_like_collection = dataContext.getConnection().GetCollection<Post_Like>("Post_Like");
            var record = post_like_collection.AsQueryable().Where(p => p.post_id == post_id).
       SelectMany(c => c.like_Records).Where(p => p._id == like_Record._id).FirstOrDefault();
            if (record == null)
            {
                return false;
            }
            return true;
        }
        public string get_like(string post_id, string user_id)
        {
            dataContext = new DataContext();
            var post_like_collection = dataContext.getConnection().GetCollection<Post_Like>("Post_Like");
            var record = post_like_collection.AsQueryable().Where(p => p.post_id == post_id).
       SelectMany(c => c.like_Records).Where(p => p._id == user_id).FirstOrDefault();
            if (record == null)
            {
                return "none";
            }
            return record.emoji;
        }

        public async Task update_post_like_record(string post_id, Like_Record like_Record)
        {
            dataContext = new DataContext();
            var post_like_collection = dataContext.getConnection().GetCollection<Post_Like>("Post_Like");
            var filter = Builders<Post_Like>.Filter.And(
        Builders<Post_Like>.Filter.Where(x => x.post_id == post_id),
        Builders<Post_Like>.Filter.Eq("like_records._id", ObjectId.Parse(like_Record._id)));
            var update = Builders<Post_Like>.Update.Set("like_records.$.emoji", like_Record.emoji);
            await post_like_collection.FindOneAndUpdateAsync(filter, update);
        }
        public async Task remove_post_like_record(string post_id, Like_Record like_Record)
        {
            dataContext = new DataContext();
            var comment_like_collection = dataContext.getConnection().GetCollection<Post_Like>("Post_Like");
            var filter = Builders<Post_Like>.Filter.Where(x => x.post_id == post_id);
            var update = Builders<Post_Like>.Update.PullFilter("like_records", Builders<Like_Record>.Filter.Eq("_id", ObjectId.Parse(like_Record._id)));
            await comment_like_collection.UpdateOneAsync(filter, update);
        }
        public async Task update_post_like_count(string post_id, string emo, int value)
        {
            dataContext = new DataContext();
            var post_collection = dataContext.getConnection().GetCollection<Post>("Post");
            var filter = Builders<Post>.Filter.Where(x => x._id == post_id);
            PostManagement pm = new PostManagement();
            Post p = pm.getPost(post_id);
            var update = Builders<Post>.Update.Set(emo, getEmojiCount(p, emo) + value);
            await post_collection.FindOneAndUpdateAsync(filter, update);
        }

        public async Task insertLikeRecord(string post_id, Like_Record like_Record)
        {
            dataContext = new DataContext();
            var post_like_collection = dataContext.getConnection().GetCollection<Post_Like>("Post_Like");
            var filter = Builders<Post_Like>.Filter.Eq("_id", ObjectId.Parse(post_id));
            var update = Builders<Post_Like>.Update.AddToSet("like_records", like_Record);
            await post_like_collection.UpdateOneAsync(filter, update);
        }

        public async Task update_sub_post_like_count(string post_id, string sub_post_id, string emo, int value)
        {
            dataContext = new DataContext();
            var post_collection = dataContext.getConnection().GetCollection<Post>("Post");
            var filter = Builders<Post>.Filter.And(
Builders<Post>.Filter.Where(x => x._id == post_id),
Builders<Post>.Filter.Eq("video_image._id", sub_post_id));
            PostManagement pm = new PostManagement();
            Post sub_post = post_collection.AsQueryable().Where(p => p._id == post_id).
                SelectMany(v => v.video_image).Where(i => i._id == sub_post_id).FirstOrDefault();
            var update = Builders<Post>.Update.Set("video_image.$." + emo, getEmojiCount(sub_post, emo) + value);
            await post_collection.FindOneAndUpdateAsync(filter, update);
        }
        public async Task remove_post_like(string _id)
        {
            dataContext = new DataContext();
            var post_like_collection = dataContext.getConnection().GetCollection<Post_Like>("Post_Like");
            var filter = Builders<Post_Like>.Filter.Eq("_id", ObjectId.Parse(_id));
            await post_like_collection.DeleteOneAsync(filter);
        }

        private int getEmojiCount(Post p, string emo)
        {
            int count = 0;
            if (emo.Equals("like"))
                count = p.like;
            if (emo.Equals("love"))
                count = p.love;
            if (emo.Equals("angry"))
                count = p.angry;
            if (emo.Equals("scare"))
                count = p.scare;
            if (emo.Equals("haha"))
                count = p.haha;
            if (emo.Equals("sad"))
                count = p.sad;
            if (emo.Equals("amaze"))
                count = p.amaze;
            if (emo.Equals("suprise"))
                count = p.suprise;
            return count;
        }
    }

}
