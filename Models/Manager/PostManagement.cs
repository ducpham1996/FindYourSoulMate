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
            Debug.WriteLine("Inserted post: " + post.title);
            var post_comment = dataContext.getConnection().GetCollection<Post_Comment>("Post_Comment");
            await post_comment.InsertOneAsync(new Post_Comment { post_id = ObjectId.Parse(post._id), comments = new List<Comment>() });
            Debug.WriteLine("Post_comment inserted: " + post.title);
            PostLikeManagement plm = new PostLikeManagement();
            await plm.insertPostLike(new Post_Like { post_id = post._id, like_Records = new List<Like_Record>() });
            foreach(VideoImage v in post.video_image)
            {
                await plm.insertPostLike(new Post_Like { post_id = v._id, like_Records = new List<Like_Record>() });
                await post_comment.InsertOneAsync(new Post_Comment { post_id = ObjectId.Parse(v._id), comments = new List<Comment>() });
            }
        }
        public List<Post> GetPosts()
        {
            var post_collection = dataContext.getConnection().GetCollection<Post>("Post");
            return post_collection.AsQueryable<Post>().ToList();
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

        public VideoImage getVideoImage(string post_id, string view_image_id)
        {
            var post_collection = dataContext.getConnection().GetCollection<Post>("Post");
            var video = post_collection.AsQueryable().Where(p => p._id == ObjectId.Parse(post_id).ToString()).
                SelectMany(p => p.video_image).Where(v => v._id == view_image_id).FirstOrDefault();
            return video;
        }
        public Post GetVideoImages(string post_id, string view_image_id)
        {
            var post_collection = dataContext.getConnection().GetCollection<Post>("Post");
            List<VideoImage> videoImages = new List<VideoImage>();
            int take = getVideoImagesPosition(post_id, view_image_id) - 1;
            if (take < 0)
            {
                take = 0;
                videoImages.Add(new VideoImage { link = "" });
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

    }
}
