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
    public class CommentLikeManagement
    {
        private DataContext dataContext;
        public CommentLikeManagement()
        {
            dataContext = new DataContext();
        }
        public async Task insertCommentLike(Comment_Like comment_Like)
        {
            var commment_like_collection = dataContext.getConnection().GetCollection<Comment_Like>("Comment_Like");
            await commment_like_collection.InsertOneAsync(comment_Like);
            Debug.WriteLine("Inserted comment like: " + comment_Like.comment_id);
        }

        public void removeCommentLike(string comment_id)
        {
            var commment_like_collection = dataContext.getConnection().GetCollection<Comment_Like>("Comment_Like");
            var filter = Builders<Comment_Like>.Filter.Eq("_id", ObjectId.Parse(comment_id));
            commment_like_collection.DeleteOne(filter);
        }

        public async Task insertLikeRecord(string comment_id, Like_Record like_Record)
        {
            var comment_like_collection = dataContext.getConnection().GetCollection<Comment_Like>("Comment_Like");
            var filter = Builders<Comment_Like>.Filter.Eq("_id", ObjectId.Parse(comment_id));
            var update = Builders<Comment_Like>.Update.AddToSet("like_records", like_Record);
            await comment_like_collection.UpdateOneAsync(filter, update);
        }
        public bool has_like(string comment_id, Like_Record like_Record)
        {
            var comment_like_collection = dataContext.getConnection().GetCollection<Comment_Like>("Comment_Like");
            var record = comment_like_collection.AsQueryable().Where(p => p.comment_id == comment_id).
       SelectMany(c => c.like_Records).Where(p => p._id == like_Record._id).FirstOrDefault();
            if (record == null)
            {
                return false;
            }
            return true;
        }

        public string get_like(string comment_id, string user_id)
        {
            var comment_like_collection = dataContext.getConnection().GetCollection<Comment_Like>("Comment_Like");
            var record = comment_like_collection.AsQueryable().Where(p => p.comment_id == comment_id).
       SelectMany(c => c.like_Records).Where(p => p._id == user_id).FirstOrDefault();
            if (record == null)
            {
                return "none";
            }
            return record.emoji;
        }

        public async Task update_like_record(string comment_id, Like_Record like_Record)
        {
            var comment_like_collection = dataContext.getConnection().GetCollection<Comment_Like>("Comment_Like");
            var filter = Builders<Comment_Like>.Filter.And(
        Builders<Comment_Like>.Filter.Where(x => x.comment_id == comment_id),
        Builders<Comment_Like>.Filter.Eq("like_records._id", ObjectId.Parse(like_Record._id)));
            var update = Builders<Comment_Like>.Update.Set("like_records.$.emoji", like_Record.emoji);
            await comment_like_collection.FindOneAndUpdateAsync(filter, update);
        }
        public async Task remove_like_record(string comment_id, Like_Record like_Record)
        {
            var comment_like_collection = dataContext.getConnection().GetCollection<Comment_Like>("Comment_Like");
            var filter = Builders<Comment_Like>.Filter.Where(x => x.comment_id == comment_id);
            var update = Builders<Comment_Like>.Update.PullFilter("like_records", Builders<Like_Record>.Filter.Eq("_id", ObjectId.Parse(like_Record._id)));
            await comment_like_collection.UpdateOneAsync(filter, update);
        }

        public async Task update_comment_likeAsync(string post_id, string comment_id, string emo, int value)
        {
            var post_comments_collection = dataContext.getConnection().GetCollection<Post_Comment>("Post_Comment");
            var filter = Builders<Post_Comment>.Filter.And(
        Builders<Post_Comment>.Filter.Where(x => x.post_id == ObjectId.Parse(post_id)),
        Builders<Post_Comment>.Filter.Eq("comments._id", ObjectId.Parse(comment_id)));
            CommentManagement cm = new CommentManagement();
            Comment c = cm.getComment(post_id, comment_id);
            var update = Builders<Post_Comment>.Update.Set("comments.$." + emo, getEmojiCount(c, emo) + value);
            await post_comments_collection.FindOneAndUpdateAsync(filter, update);
        }

        public async Task update_sub_comment_likeAsync(string post_id, string comment_id, string sub_comment_id, string emo, int value)
        {
            CommentManagement cm = new CommentManagement();
            var post_comments_collection = dataContext.getConnection().GetCollection<Post_Comment>("Post_Comment");
            var filter = Builders<Post_Comment>.Filter.And(
        Builders<Post_Comment>.Filter.Where(x => x.post_id == ObjectId.Parse(post_id)),
        Builders<Post_Comment>.Filter.Eq("comments._id", ObjectId.Parse(comment_id)), Builders<Post_Comment>.Filter.Eq("comments.sub_comments._id", ObjectId.Parse(sub_comment_id)));
            Comment c = cm.getSubComment(post_id, comment_id, sub_comment_id);
            var update = Builders<Post_Comment>.Update.Set("comments.$.sub_comments." +
                cm.getSubCommentPosition(post_id, comment_id, sub_comment_id) +
                "." + emo, getEmojiCount(c, emo) + value);
            await post_comments_collection.FindOneAndUpdateAsync(filter, update);
        }
        private int getEmojiCount(Comment c, string emo)
        {
            int count = 0;
            if (emo.Equals("like"))
                count = c.like;
            if (emo.Equals("love"))
                count = c.love;
            if (emo.Equals("angry"))
                count = c.angry;
            if (emo.Equals("scare"))
                count = c.scare;
            if (emo.Equals("haha"))
                count = c.haha;
            if (emo.Equals("sad"))
                count = c.sad;
            if (emo.Equals("amaze"))
                count = c.amaze;
            if (emo.Equals("suprise"))
                count = c.suprise;
            return count;
        }
    }
}
