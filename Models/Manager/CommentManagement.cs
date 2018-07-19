using FindYourSoulMate.Models.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FindYourSoulMate.Models.Manager
{

    public class CommentManagement
    {
        private DataContext dataContext;
        private CommentLikeManagement lm;
        private PostManagement pm;
        private UserManagement um;
        public CommentManagement()
        {
            dataContext = new DataContext();
            lm = new CommentLikeManagement();
            pm = new PostManagement();
            um = new UserManagement();
        }
        public async Task<Comment> insertComment(string post_id, Comment comment)
        {
            var post_comments_collection = dataContext.getConnection().GetCollection<BsonDocument>("Post_Comment");
            var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(post_id));
            comment._id = ObjectId.GenerateNewId().ToString();
            comment.sub_comments = new List<Comment>();
            comment.date_created = DateTime.Now;
            var update = Builders<BsonDocument>.Update.AddToSet("comments", comment);
            await post_comments_collection.UpdateOneAsync(filter, update);
            await lm.insertCommentLike(new Comment_Like { comment_id = comment._id, like_Records = new List<Like_Record>() });
            return comment;
        }
        public IEnumerable<Comment> GetComments(string post_id, int take, int skip)
        {
            var post_comments_collection = dataContext.getConnection().GetCollection<Post_Comment>("Post_Comment");
            var project = Builders<Post_Comment>.Projection.Slice("comments", skip, take).Exclude("comments.sub_comments");
            var post_Comments = post_comments_collection.Find(x => x.post_id == ObjectId.Parse(post_id)).Project(project).SingleAsync().Result;
            var demo = BsonSerializer.Deserialize<Post_Comment>(post_Comments);
            return demo.comments;
        }
        //      public IEnumerable<Comment> Get_Sub_Comments(string post_id, string comment_id, int take, int skip)
        //      {
        //          var post_comments_collection = dataContext.getConnection().GetCollection<Post_Comment>("Post_Comment");
        //          var filter = Builders<Post_Comment>.Filter.And(
        //Builders<Post_Comment>.Filter.Where(x => x.post_id == ObjectId.Parse(post_id)),
        //Builders<Post_Comment>.Filter.Eq("comments._id", ObjectId.Parse(comment_id)));
        //          var project = Builders<Post_Comment>.Projection.Combine(Builders<Post_Comment>.Projection.Slice("comments", 0, 1), Builders<Post_Comment>.Projection.Slice("comments.sub_comments", skip, take));
        //          var post_Comments = post_comments_collection.Find(filter).Project(project).SingleAsync().Result;
        //          Debug.WriteLine(post_comments_collection.Find(filter).Project(project));
        //          var demo = BsonSerializer.Deserialize<Post_Comment>(post_Comments);
        //          return demo.comments[0].sub_comments;
        //      }
        public List<Comment> Get_Sub_Comments(string post_id, string comment_id, int take, int skip)
        {
            var post_comments_collection = dataContext.getConnection().GetCollection<Post_Comment>("Post_Comment");
            var doc = post_comments_collection.Aggregate().Match(new BsonDocument { { "_id", ObjectId.Parse(post_id) } })
                 .Unwind("comments")
                 .Match(new BsonDocument { { "comments._id", ObjectId.Parse(comment_id) } })
                 .Unwind("comments.sub_comments")
                 .Sort(new BsonDocument { { "comments.sub_comments.dateCreated", 1 } })
                 .Group("{_id: '$_id', 'comments': {'$push': '$comments.sub_comments'}}")
                 .Project("{'comments': { '$slice\' : ['$comments'," + skip + ", " + take + "] }}");
            var sub_comments = BsonSerializer.Deserialize<Post_Comment>(doc.SingleAsync().Result);
            List<Comment> comments = sub_comments.comments;
            foreach (Comment c in comments)
            {
                User u = um.GetUser_Detail(c.owner._id);
                c.owner.user_name = u.first_name + " " + u.last_name;
                c.owner.user_picture = u.profile_img;
            }
            return comments;
        }

        public IEnumerable<Comment> GetAllComments(string post_id)
        {
            var post_comments_collection = dataContext.getConnection().GetCollection<Post_Comment>("Post_Comment");
            Post_Comment post_Comments = post_comments_collection.Find(x => x.post_id == ObjectId.Parse(post_id)).SingleAsync().Result;
            List<Comment> comments = post_Comments.comments;
            foreach (Comment c in comments)
            {
                User u = um.GetUser_Detail(c.owner._id);
                c.owner.user_name = u.first_name + " " + u.last_name;
                c.owner.user_picture = u.profile_img;
            }
            return comments;
        }
        public int noOfComment(int noOfRecords)
        {
            int no = (int)Math.Ceiling(noOfRecords * 1.0 / 5);
            return no;
        }
        public async Task<Comment> insert_subCommentAsync(string post_id, string comment_id, Comment comment)
        {
            comment._id = ObjectId.GenerateNewId().ToString();
            comment.sub_comments = null;
            var post_comments_collection = dataContext.getConnection().GetCollection<Post_Comment>("Post_Comment");
            var filter = Builders<Post_Comment>.Filter.And(
        Builders<Post_Comment>.Filter.Where(x => x.post_id == ObjectId.Parse(post_id)),
        Builders<Post_Comment>.Filter.Eq("comments._id", comment_id));
            var update = Builders<Post_Comment>.Update.Push("comments.$.sub_comments", comment);
            await post_comments_collection.FindOneAndUpdateAsync(filter, update);
            await lm.insertCommentLike(new Comment_Like { comment_id = comment._id, like_Records = new List<Like_Record>() });
            return comment;
        }

        public async Task update_sub_commentAsync(string post_id, string comment_id, string sub_comment_id, string comment_text)
        {
            var post_comments_collection = dataContext.getConnection().GetCollection<Post_Comment>("Post_Comment");
            var filter = Builders<Post_Comment>.Filter.And(
        Builders<Post_Comment>.Filter.Where(x => x.post_id == ObjectId.Parse(post_id)),
        Builders<Post_Comment>.Filter.Eq("comments._id", ObjectId.Parse(comment_id)), Builders<Post_Comment>.Filter.Eq("comments.sub_comments._id", ObjectId.Parse(sub_comment_id)));
            var update = Builders<Post_Comment>.Update.Set("comments.$.sub_comments." +
                getSubCommentPosition(post_id, comment_id, sub_comment_id) +
                ".message", comment_text);
            await post_comments_collection.FindOneAndUpdateAsync(filter, update);
        }
        //public async Task update_sub_commentAsync(string post_id, string comment_id, Comment comment)
        //{
        //    var post_comments_collection = dataContext.getConnection().GetCollection<Post_Comment>("Post_Comment");
        //    var filter = Builders<Post_Comment>.Filter.And(
        //Builders<Post_Comment>.Filter.Where(x => x.post_id == ObjectId.Parse(post_id)),
        //Builders<Post_Comment>.Filter.Eq("comments._id", ObjectId.Parse(comment_id)));
        //    var update = Builders<Post_Comment>.Update.PullFilter("comments.$.sub_comments", Builders<Comment>.Filter.Eq("_id", ObjectId.Parse(comment._id)));
        //    await post_comments_collection.UpdateOneAsync(filter, update);
        //    var add = Builders<Post_Comment>.Update.Push("comments.$.sub_comments", comment);
        //    await post_comments_collection.FindOneAndUpdateAsync(filter, add);
        //}


        public Comment getComment(string post_id, string comment_id)
        {
            var post_comments_collection = dataContext.getConnection().GetCollection<Post_Comment>("Post_Comment");
            var comment = post_comments_collection.AsQueryable().Where(p => p.post_id == ObjectId.Parse(post_id)).
                SelectMany(c => c.comments).Where(p => p._id == comment_id).FirstOrDefault();
            return comment;

        }

        public Comment getCommentWithoutSubComment(string post_id, string comment_id)
        {
            var post_comments_collection = dataContext.getConnection().GetCollection<Post_Comment>("Post_Comment");
            var query = post_comments_collection.Aggregate().Match(new BsonDocument { { "_id", ObjectId.Parse(post_id) } }).
    Unwind("comments").Project("{ \"comments\" : \"$comments\", \"_id\" : 0 }").Project("{\"comments.sub_comments\" : 0 }")
    .Match(new BsonDocument { { "comments._id", ObjectId.Parse(comment_id) } });
            BsonDocument document = query.SingleAsync().Result;
            BsonDocument commentDoc = document["comments"].ToBsonDocument();
            return BsonSerializer.Deserialize<Comment>(commentDoc);
        }
        public Comment getSubComment(string post_id, string comment_id, string sub_comment_id)
        {
            var post_comments_collection = dataContext.getConnection().GetCollection<Post_Comment>("Post_Comment");
            var sub_comment = post_comments_collection.Aggregate().Match(new BsonDocument { { "_id", ObjectId.Parse(post_id) } }).
                 Unwind("comments").Project("{ 'comments' : '$comments', '_id' : 0 } ").
                 Match(new BsonDocument { { "comments._id", ObjectId.Parse(comment_id) } }).
                 Unwind("comments.sub_comments").
                 Project("{ 'sub_comments' : '$comments.sub_comments', '_id' : 0 } ").
                 Match(new BsonDocument { { "sub_comments._id", ObjectId.Parse(sub_comment_id) } }).
                 Project("{ 'comments' : '$sub_comments', '_id' : 0 }");
            BsonDocument document = sub_comment.SingleAsync().Result;
            BsonDocument commentDoc = document["comments"].ToBsonDocument();
            return BsonSerializer.Deserialize<Comment>(commentDoc);
        }

        public async Task update_comment_count(string post_id, string comment_id, int mode)
        {
            var post_comments_collection = dataContext.getConnection().GetCollection<Post_Comment>("Post_Comment");
            var filter = Builders<Post_Comment>.Filter.And(
        Builders<Post_Comment>.Filter.Where(x => x.post_id == ObjectId.Parse(post_id)),
        Builders<Post_Comment>.Filter.Eq("comments._id", ObjectId.Parse(comment_id)));
            if (mode == 0)
            {
                var update = Builders<Post_Comment>.Update.Set("comments.$.comments", getComment(post_id, comment_id).comments + 1);
                await post_comments_collection.FindOneAndUpdateAsync(filter, update);
            }
            else
            {
                Comment c = getComment(post_id, comment_id);
                if (c.comments > 0)
                {
                    var update = Builders<Post_Comment>.Update.Set("comments.$.comments", getComment(post_id, comment_id).comments - 1);
                    await post_comments_collection.FindOneAndUpdateAsync(filter, update);
                }
            }

        }

        public async Task update_commentAsync(string post_id, string comment_id, string comment_text)
        {
            var post_comments_collection = dataContext.getConnection().GetCollection<Post_Comment>("Post_Comment");
            var filter = Builders<Post_Comment>.Filter.And(
        Builders<Post_Comment>.Filter.Where(x => x.post_id == ObjectId.Parse(post_id)),
        Builders<Post_Comment>.Filter.Eq("comments._id", ObjectId.Parse(comment_id)));
            var update = Builders<Post_Comment>.Update.Set("comments.$.message", comment_text);
            await post_comments_collection.FindOneAndUpdateAsync(filter, update);
        }
        public async Task delete_sub_commentAsync(string post_id, string comment_id, string sub_comment_id)
        {
            var post_comments_collection = dataContext.getConnection().GetCollection<Post_Comment>("Post_Comment");
            var filter = Builders<Post_Comment>.Filter.And(
        Builders<Post_Comment>.Filter.Where(x => x.post_id == ObjectId.Parse(post_id)),
        Builders<Post_Comment>.Filter.Eq("comments._id", ObjectId.Parse(comment_id)));
            var update = Builders<Post_Comment>.Update.PullFilter("comments.$.sub_comments", Builders<Comment>.Filter.Eq("_id", ObjectId.Parse(sub_comment_id)));
            await post_comments_collection.UpdateOneAsync(filter, update);
            lm.removeCommentLike(sub_comment_id);
            await pm.decrease_sub_commentsAsync(post_id, 1);
        }

        public async Task delete_commentAsync(string post_id, string comment_id)
        {
            var post_comments_collection = dataContext.getConnection().GetCollection<Post_Comment>("Post_Comment");
            var filter = Builders<Post_Comment>.Filter.Where(x => x.post_id == ObjectId.Parse(post_id));
            var update = Builders<Post_Comment>.Update.PullFilter("comments", Builders<Comment>.Filter.Eq("_id", ObjectId.Parse(comment_id)));
            List<Comment> subComments = getComment(post_id, comment_id).sub_comments;
            foreach (Comment c in subComments)
            {
                lm.removeCommentLike(c._id);
            }
            lm.removeCommentLike(comment_id);
            await post_comments_collection.UpdateOneAsync(filter, update);
            await pm.decrease_commentsAsync(post_id, 1);
        }

        //public int getCommentPosition(string post_id, string comment_id)
        //{
        //    var post_comments_collection = dataContext.getConnection().GetCollection<Post_Comment>("Post_Comment");
        //    string match = "{ \"_id\" : ObjectId(\"" + post_id + "\") }";
        //    string project = "{ " +
        //            "\"matchedIndex\":" +
        //                "{" +
        //                "\"$indexOfArray\": [ \"$comments._id\", ObjectId(\"" + comment_id + "\")]" +
        //                 "}}";
        //    var comment = post_comments_collection.Aggregate().Match(match).Project(project);
        //    return comment.SingleAsync().Result.GetValue(1).ToInt32();
        //}
        public int getSubCommentPosition(string post_id, string comment_id, string sub_comment_id)
        {
            var post_comments_collection = dataContext.getConnection().GetCollection<Post_Comment>("Post_Comment");
            string project = "{ " +
                    "\"matchedIndex\":" +
                        "{" +
                        "\"$indexOfArray\": [ \"$comments._id\", ObjectId(\"" + comment_id + "\")]" +
                         "}}";
            var comment = post_comments_collection.Aggregate()
                .Match(new BsonDocument { { "_id", ObjectId.Parse(post_id) } })
                .Unwind("comments")
                .Project("{ 'comments' : '$comments', '_id' : 0 }")
                .Match(new BsonDocument { { "comments._id", ObjectId.Parse(comment_id) } })
                .Unwind("comments.sub_comments")
                .Project("{ 'sub_comments' : '$comments.sub_comments', '_id' : 0 }")
                .Group("{ '_id' : '$_id' , 'sub_comments' : { '$push' : '$sub_comments' } }")
                .Project("{ 'matchedIndex': { '$indexOfArray': [ '$sub_comments._id', ObjectId('" + sub_comment_id + "')]}}");
            return comment.SingleAsync().Result.GetValue(1).ToInt32();
        }

    }
}
