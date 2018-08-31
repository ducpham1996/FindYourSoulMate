using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FindYourSoulMate.Models;
using FindYourSoulMate.Models.Entities;
using FindYourSoulMate.Models.Manager;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace FindYourSoulMateAngular.Controllers
{

    [Route("api/Post")]
    public class PostController : Controller
    {
        private IHostingEnvironment _env;
        private Helper helper;
        private PostManagement pm;
        private UserManagement um;
        private PostLikeManagement plm;
        public PostController(IHostingEnvironment env)
        {
            _env = env;
            helper = new Helper();
            pm = new PostManagement();
            um = new UserManagement();
            plm = new PostLikeManagement();
        }

        [HttpGet]
        [Produces("application/json")]
        public List<Post> getPosts()
        {
            try{
                Owner owner = um.GetUser_Cookie(Request);
                List<Post> posts = pm.GetPosts();
                foreach (Post p in posts)
                {
                    if (p.video_image != null)
                    {
                        p.video_image = p.video_image.Take(5).ToList();
                        foreach (Post sp in p.video_image)
                        {
                            sp._id = helper.EncodeTo64(sp._id);
                        }
                    }
                    User u = um.GetUser_Detail(p.owner._id);
                    p.owner.user_name = u.first_name + " " + u.last_name;
                    p.owner.user_picture = u.profile_img;
                    p.no_of_comment = (int)Math.Ceiling(p.comments * 1.0 / 5);
                    p.has_like = plm.get_like(p._id, owner._id);
                    p._id = helper.EncodeTo64(p._id);
                }
                return posts;
            }
            catch(Exception e)
            {
                return new List<Post>();
            }
       
        }

        [HttpPost("PostSubmission")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> postSubmission([FromForm] Post_Detail post_Detail)
        {
            try
            {
                Owner owner = um.GetUser_Cookie(Request);
                Post post = new Post { description = post_Detail.content };
                post.date_created = DateTime.Now;
                post.owner = owner;
                post.modify_date = DateTime.Now;
                var path = _env.WebRootPath;
                var uploads = Path.Combine(path, "files");
                List<Post> VideoImages = new List<Post>();
                if (post_Detail.files != null)
                {
                    foreach (IFormFile file in post_Detail.files)
                    {
                        string[] tokens = file.ContentType.Split("/");
                        string extension = System.IO.Path.GetExtension(file.FileName);
                        string _id = ObjectId.GenerateNewId().ToString();
                        await save_fileAsync(_id + System.IO.Path.GetFileNameWithoutExtension(file.FileName) + extension, uploads, file);
                        Post videoImage = new Post
                        {
                            _id = _id,
                            type = tokens[0],
                            link = "/files/" + _id + System.IO.Path.GetFileNameWithoutExtension(file.FileName) + extension,
                            date_created = DateTime.Now,
                            modify_date = DateTime.Now
                        };
                        VideoImages.Add(videoImage);
                    }
                    post.video_image = VideoImages;
                }
                await pm.insertPostAsync(post);
                return Ok("Submit successfully");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return BadRequest("Error from server");
            }
        }
        private async Task<string> save_fileAsync(string file_name, string save_folder, IFormFile file)
        {
            var filePath = Path.Combine(save_folder, file_name);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
                stream.Close();
            }
            return file_name;
        }

        [HttpGet("GetSubPost")]
        [Produces("application/json")]
        public List<Post> getVideoImage(string post_id, string sub_post_id)
        {
            Owner owner = um.GetUser_Cookie(Request);
            string decoded_post_id = helper.DecodeFrom64(post_id);
            string decoded_sub_post_id = helper.DecodeFrom64(sub_post_id);
            Post post = pm.GetVideoImages(decoded_post_id, decoded_sub_post_id);
            User u = um.GetUser_Detail(post.owner._id);
            post.owner.user_name = u.first_name + " " + u.last_name;
            post.owner.user_picture = u.profile_img;
            PostLikeManagement plm = new PostLikeManagement();
            foreach (Post p in post.video_image)
            {
                p.has_like = plm.get_like(p._id, owner._id);
                if (p._id != null)
                {
                    p._id = helper.EncodeTo64(p._id);
                }
                p.no_of_comment = (int)Math.Ceiling(p.comments * 1.0 / 5);
            }
            return post.video_image;
        }
        [HttpPost("PostEditting")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> postEditting([FromForm] Edit_Post edit_Post)
        {
            try
            {
                Owner owner = um.GetUser_Cookie(Request);
                var path = _env.WebRootPath;
                var uploads = Path.Combine(path, "files");
                string decoded_post_id = helper.DecodeFrom64(edit_Post.post_id);
                List<Post> newFiles = new List<Post>();
                if (edit_Post.news != null)
                {
                    foreach (IFormFile file in edit_Post.news)
                    {
                        string[] tokens = file.ContentType.Split("/");
                        string extension = System.IO.Path.GetExtension(file.FileName);
                        string _id = ObjectId.GenerateNewId().ToString();
                        await save_fileAsync(_id + System.IO.Path.GetFileNameWithoutExtension(file.FileName) + extension, uploads, file);
                        Post videoImage = new Post
                        {
                            _id = _id,
                            type = tokens[0],
                            link = "/files/" + _id + System.IO.Path.GetFileNameWithoutExtension(file.FileName) + extension,
                            date_created = DateTime.Now,
                            modify_date = DateTime.Now
                        };
                        newFiles.Add(videoImage);
                    }
                }
                Post p = pm.getPost(decoded_post_id);
                List<string> ori = new List<string>();
                foreach (Post sp in p.video_image)
                {
                    ori.Add(sp._id);
                }
                List<string> left = new List<string>();
                foreach (string sp in edit_Post.olds)
                {
                    left.Add(helper.DecodeFrom64(sp));
                }
                List<string> delete = new List<string>();
                delete = ori.Except(left).ToList();
                await pm.editPostAsync(decoded_post_id, edit_Post.content, delete, newFiles);
                return Ok("Edit successfully");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return BadRequest("Error from server");
            }
        }
        [HttpPost("RemovePost")]
        [Produces("application/json")]
        public async Task<IActionResult> removePost([FromBody] Remove_Post remove_Post)
        {
            try
            {
                await pm.removePost(helper.DecodeFrom64(remove_Post.post_id));
                return Ok("remove successfully");
            }
            catch (Exception e)
            {
                return BadRequest("Error from server");
            }

        }
        public class Post_Detail
        {
            public string content { get; set; }

            public List<IFormFile> files { get; set; }

        }
        public class Edit_Post
        {
            public string post_id { get; set; }
            public string content { get; set; }
            public List<string> olds { get; set; }
            public List<IFormFile> news { get; set; }
        }
        public class Remove_Post
        {
            public string post_id { get; set; }
        }
    }
}