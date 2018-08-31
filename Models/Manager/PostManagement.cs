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

    public class PostManagement
    {
        DataContext dataContext;
        public PostManagement()
        {
            dataContext = new DataContext();
        }
        public async Task insertPostAsync(Post post)
        {
            var post_collection = dataContext.getConnection().GetCollection<Post>("Post");
            await post_collection.InsertOneAsync(post);
            var post_comment = dataContext.getConnection().GetCollection<Post_Comment>("Post_Comment");
            await post_comment.InsertOneAsync(new Post_Comment { post_id = ObjectId.Parse(post._id), comments = new List<Comment>() });
            PostLikeManagement plm = new PostLikeManagement();
            PostParticipantManagement ppm = new PostParticipantManagement();
            await ppm.insertPostParticipantAsync(new PostParticipant { _id = post._id ,participants = new List<Participant>()});
            await ppm.insertPostParticipantRecord(post._id, new Participant { _id = post.owner._id, email = post.owner.email, status = true });
            await plm.insertPostLike(new Post_Like { post_id = post._id, like_Records = new List<Like_Record>() });
            if(post.video_image != null)
            {
                foreach (Post v in post.video_image)
                {
                    await plm.insertPostLike(new Post_Like { post_id = v._id, like_Records = new List<Like_Record>() });
                    await post_comment.InsertOneAsync(new Post_Comment { post_id = ObjectId.Parse(v._id), comments = new List<Comment>() });
                }
            }
        }
        public List<Post> GetPosts()
        {
            var post_collection = dataContext.getConnection().GetCollection<Post>("Post");
            return post_collection.AsQueryable<Post>().Where(p => p.status == 0).ToList();
        }

        public Post getPost(string post_id)
        {
            var post_collection = dataContext.getConnection().GetCollection<Post>("Post");
            Post post = post_collection.Find(x => x._id == ObjectId.Parse(post_id).ToString()).SingleAsync().Result;
            return post;
        }
        public async Task update_PostAsync(Post post)
        {
            var post_collection = dataContext.getConnection().GetCollection<Post>("Post");
            var filter = Builders<Post>.Filter.Eq("_id", post._id);
            var update = Builders<Post>.Update.Set(p => p.comments, post.comments);
            await post_collection.UpdateOneAsync(filter, update);
        }

        public async Task increase_commentsAsync(string post_id, int value)
        {
            Post post = getPost(post_id);
            var post_collection = dataContext.getConnection().GetCollection<Post>("Post");
            var filter = Builders<Post>.Filter.Eq("_id", ObjectId.Parse(post_id));
            var update = Builders<Post>.Update.Set(p => p.comments, post.comments + value);
            await post_collection.UpdateOneAsync(filter, update);
        }
        public async Task decrease_commentsAsync(string post_id, int value)
        {
            Post post = getPost(post_id);
            var post_collection = dataContext.getConnection().GetCollection<Post>("Post");
            var filter = Builders<Post>.Filter.Eq("_id", ObjectId.Parse(post_id));
            var update = Builders<Post>.Update.Set(p => p.comments, post.comments - value);
            await post_collection.UpdateOneAsync(filter, update);
        }
        public async Task increase_sub_commentsAsync(string post_id, int value)
        {
            Post post = getPost(post_id);
            var post_collection = dataContext.getConnection().GetCollection<Post>("Post");
            var filter = Builders<Post>.Filter.Eq("_id", ObjectId.Parse(post_id));
            var update = Builders<Post>.Update.Set(p => p.sub_comments, post.sub_comments + value);
            await post_collection.UpdateOneAsync(filter, update);
        }

        public async Task decrease_sub_commentsAsync(string post_id, int value)
        {
            Post post = getPost(post_id);
            var post_collection = dataContext.getConnection().GetCollection<Post>("Post");
            var filter = Builders<Post>.Filter.Eq("_id", ObjectId.Parse(post_id));
            var update = Builders<Post>.Update.Set(p => p.sub_comments, post.sub_comments - value);
            await post_collection.UpdateOneAsync(filter, update);
        }

        public Post getVideoImage(string post_id, string view_image_id)
        {
            var post_collection = dataContext.getConnection().GetCollection<Post>("Post");
            var video = post_collection.AsQueryable().Where(p => p._id == ObjectId.Parse(post_id).ToString()).
                SelectMany(p => p.video_image).Where(v => v._id == view_image_id).FirstOrDefault();
            return video;
        }
        public Post GetVideoImages(string post_id, string view_image_id)
        {
            var post_collection = dataContext.getConnection().GetCollection<Post>("Post");
            List<Post> videoImages = new List<Post>();
            int take = getVideoImagesPosition(post_id, view_image_id) - 1;
            if (take == -1)
            {
                take = 0;
                videoImages.Add(new Post { link = "" });
            }
            var project = Builders<Post>.Projection.Slice("video_image", take, 5);
            var post_video = post_collection.Find(x => x._id == ObjectId.Parse(post_id).ToString()).Project(project).SingleAsync().Result;
            var result = BsonSerializer.Deserialize<Post>(post_video);
            videoImages.AddRange(result.video_image);
            result.video_image = videoImages;
            return result;
        }

        public int getVideoImagesPosition(string post_id, string view_image_id)
        {
            var post_collection = dataContext.getConnection().GetCollection<Post>("Post");
            string match = "{ \"_id\" : ObjectId(\"" + post_id + "\") }";
            string project = "{ " +
                    "\"matchedIndex\":" +
                        "{" +
                        "\"$indexOfArray\": [ \"$video_image._id\", ObjectId(\"" + view_image_id + "\")]" +
                         "}}";
            var results = post_collection.Aggregate().Match(match).Project(project);
            return results.SingleAsync().Result.GetValue(1).ToInt32();
        }

        public async Task deletePostAsync(string post_id)
        {
            var post_collection = dataContext.getConnection().GetCollection<Post>("Post");
            var filter = Builders<Post>.Filter.Eq("_id", ObjectId.Parse(post_id));
            await post_collection.DeleteOneAsync(filter);
        }
        public async Task increase_sub_post_comment_countAsync(string post_id, string sub_post_id, int value)
        {
            var post_collection = dataContext.getConnection().GetCollection<Post>("Post");
            var filter = Builders<Post>.Filter.And(Builders<Post>.Filter.Where(x => x._id == post_id),
Builders<Post>.Filter.Eq("video_image._id", ObjectId.Parse(sub_post_id)));
            var update = Builders<Post>.Update.Set("video_image.$.comments", getSubPost(post_id, sub_post_id).comments + value);
            await post_collection.FindOneAndUpdateAsync(filter, update);
        }

        public async Task increase_sub_post_sub_comment_countAsync(string post_id, string sub_post_id, int value)
        {
            var post_collection = dataContext.getConnection().GetCollection<Post>("Post");
            var filter = Builders<Post>.Filter.And(Builders<Post>.Filter.Where(x => x._id == post_id),
Builders<Post>.Filter.Eq("video_image._id", ObjectId.Parse(sub_post_id)));
            var update = Builders<Post>.Update.Set("video_image.$.sub_comments", getSubPost(post_id, sub_post_id).sub_comments + value);
            await post_collection.FindOneAndUpdateAsync(filter, update);
        }
        public Post getSubPost(string post_id, string sub_post_id)
        {
            var post_collection = dataContext.getConnection().GetCollection<Post>("Post");
            var sub_post = post_collection.AsQueryable().Where(p => p._id == post_id).
                SelectMany(c => c.video_image).Where(p => p._id == sub_post_id).FirstOrDefault();
            return sub_post;
        }

        public async Task editPostAsync(string post_id, string content, List<string> delete, List<Post> news)
        {
            var post_collection = dataContext.getConnection().GetCollection<Post>("Post");
            var post_comment = dataContext.getConnection().GetCollection<Post_Comment>("Post_Comment");
            PostLikeManagement plm = new PostLikeManagement();
            var filter = Builders<Post>.Filter.Eq("_id", ObjectId.Parse(post_id));
            var update = Builders<Post>.Update.Set(p => p.description, content);
            if(news != null)
            {
                foreach (Post p in news)
                {
                    await addSubPost(post_id, p);
                    await plm.insertPostLike(new Post_Like { post_id = p._id, like_Records = new List<Like_Record>() });
                    await post_comment.InsertOneAsync(new Post_Comment { post_id = ObjectId.Parse(p._id), comments = new List<Comment>() });
                }
            }
            if(delete != null)
            {
                foreach (string _id in delete)
                {
                    await removeSubPost(post_id, _id);
                }
            }
            await post_collection.UpdateOneAsync(filter, update);
        }
        public async Task addSubPost(string post_id, Post subPost)
        {
            var post_collection = dataContext.getConnection().GetCollection<Post>("Post");
            var filter = Builders<Post>.Filter.Eq("_id", ObjectId.Parse(post_id));
            var update = Builders<Post>.Update.AddToSet("video_image", subPost);
            await post_collection.UpdateOneAsync(filter, update);
        }
        public async Task removeSubPost(string post_id, string sub_post_id)
        {
            PostLikeManagement plm = new PostLikeManagement();
            CommentManagement cm = new CommentManagement();
            var post_collection = dataContext.getConnection().GetCollection<Post>("Post");
            var filter = Builders<Post>.Filter.Eq("_id", ObjectId.Parse(post_id));
            var update = Builders<Post>.Update.PullFilter("video_image", Builders<Post>.Filter.Eq("_id", ObjectId.Parse(sub_post_id)));
            await post_collection.UpdateOneAsync(filter, update);
            await plm.remove_post_like(sub_post_id);
            await cm.remove_post_comment(sub_post_id);
        }
        public async Task removePost(string post_id)
        {
            var post_collection = dataContext.getConnection().GetCollection<Post>("Post");
            var filter = Builders<Post>.Filter.Eq("_id", ObjectId.Parse(post_id));
            var update = Builders<Post>.Update.Set("status", 1);
            await post_collection.UpdateOneAsync(filter, update);
        }
    }
}
