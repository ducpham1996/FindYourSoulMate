using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FindYourSoulMate.Models;
using FindYourSoulMate.Models.Manager;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Security.Principal;
using System.Diagnostics;
using System.Threading;
using Microsoft.AspNetCore.Http;
using FindYourSoulMate.Models.Entities;
using MongoDB.Bson;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace FindYourSoulMate.Controllers
{
    [Authorize(Roles = "User")]
    public class PostsController : Controller
    {
        private readonly FindYourSoulMateContext _context;
        private PostManagement pm;
        private UserManagement um;
        private IHostingEnvironment _env;
        private Helper helper;
        public PostsController(IHostingEnvironment env)
        {
            pm = new PostManagement();
            um = new UserManagement();
            _env = env;
            helper = new Helper();
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            List<Post> posts = pm.GetPosts();
            foreach (Post p in posts)
            {
                User u = um.GetUser_Detail(p.owner._id);
                p.owner.user_name = u.first_name + " " + u.last_name;
                p.owner.user_picture = u.profile_img;
            }
            return View(posts);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .SingleOrDefaultAsync(m => m.id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public string getVideoImage(string post_id, string sub_post_id)
        {
            string decoded_post_id = helper.DecodeFrom64(post_id);
            string decoded_sub_post_id = helper.DecodeFrom64(sub_post_id);
            Post p = pm.GetVideoImages(decoded_post_id, decoded_sub_post_id);
            User u = um.GetUser_Detail(p.owner._id);
            p.owner.user_name = u.first_name + " " + u.last_name;
            p.owner.user_picture = u.profile_img;
            return JsonConvert.SerializeObject(p);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,title,description")] Post post)
        {
            if (ModelState.IsValid)
            {
                Owner user_Session = um.GetUser_Session(Request);
                post.date_created = DateTime.Now;
                post.owner = user_Session;
                post.modify_date = DateTime.Now;
                var path = _env.WebRootPath;
                var uploads = Path.Combine(path, "img");
                List<VideoImage> VideoImages = new List<VideoImage>();
                var files = HttpContext.Request.Form.Files;
                foreach (IFormFile file in files)
                {
                    string extension = System.IO.Path.GetExtension(file.FileName);
                    string _id = ObjectId.GenerateNewId().ToString();
                    await save_fileAsync(_id + extension, uploads, file);
                    VideoImage videoImage = new VideoImage
                    {
                        _id = _id,
                        type = extension,
                        link = "/img/" + _id + extension
                    };
                    VideoImages.Add(videoImage);
                }
                post.video_image = VideoImages;
                await pm.insertPostAsync(post);
                return Ok(new { Message = "Post created sucessfully" });
            }
            return BadRequest(new { Message = "Please fill all fields" });
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

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.SingleOrDefaultAsync(m => m.id == id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("id,title,description,date_created,likes")] Post post)
        {
            if (id != post.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .SingleOrDefaultAsync(m => m.id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var post = await _context.Post.SingleOrDefaultAsync(m => m.id == id);
            _context.Post.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(string id)
        {
            return _context.Post.Any(e => e.id == id);
        }
    }
}
