using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FindYourSoulMate.Models;
using FindYourSoulMate.Models.Entities;
using FindYourSoulMate.Models.Manager;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FindYourSoulMate.Controllers
{
    public class CommentController : Controller
    {
        private CommentManagement cm;
        private PostManagement pm;
        private UserManagement um;
        private CommentLikeManagement lm;
        private Helper _helper;
        public CommentController()
        {
            cm = new CommentManagement();
            um = new UserManagement();
            pm = new PostManagement();
            lm = new CommentLikeManagement();
            _helper = new Helper();
        }
        [HttpPost]
        public string GetComments(string post_id, int noOfcomment)
        {
            string decoded_id = _helper.DecodeFrom64(post_id);
            int skip = 5 * (noOfcomment - 1);
            IEnumerable<Comment> comments = cm.GetComments(decoded_id, 5, skip);
            Owner owner = um.GetUser_Session(Request);
            foreach (Comment c in comments)
            {
                User u = um.GetUser_Detail(c.owner._id);
                c.owner.user_name = u.first_name + " " + u.last_name;
                c.owner.user_picture = u.profile_img;
                if (c.owner._id == owner._id)
                {
                    c.is_own = true;
                }
                c.has_like = lm.get_like(c._id, owner._id);
            }
            return JsonConvert.SerializeObject(comments);
        }

        [HttpPost]
        public string GetSubComments(string post_id, string comment_id, int noOfcomment)
        {
            Owner owner = um.GetUser_Session(Request);
            string decoded_post_id = _helper.DecodeFrom64(post_id);
            string decoded_comment_id = _helper.DecodeFrom64(comment_id);
            int skip = 5 * (noOfcomment - 1);
            IEnumerable<Comment> comments =
                cm.Get_Sub_Comments(decoded_post_id, decoded_comment_id, 5, skip);
            foreach (Comment c in comments)
            {
                User u = um.GetUser_Detail(c.owner._id);
                c.owner.user_name = u.first_name + " " + u.last_name;
                c.owner.user_picture = u.profile_img;
                if (c.owner._id == owner._id)
                {
                    c.is_own = true;
                }
                c.has_like = lm.get_like(c._id, owner._id);
            }
            return JsonConvert.SerializeObject(comments);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create_SubComment(string comment_text, string post_id, string parent_comment_id)
        {
            if (comment_text == null)
            {
                return BadRequest(new { Message = "Please fill in something" });
            }
            if (ModelState.IsValid)
            {
                Owner user_Session = um.GetUser_Session(Request);
                string decoded_post_id = _helper.DecodeFrom64(post_id);
                string decoded_parent_comment_id = _helper.DecodeFrom64(parent_comment_id);
                Comment c = await cm.insert_subCommentAsync(decoded_post_id, decoded_parent_comment_id, new Models.Comment
                {
                    message = comment_text,
                    date_created = DateTime.Now,
                    owner = user_Session
                });
                c.has_like = "none";
                c.is_own = true;
                await pm.increase_sub_commentsAsync(decoded_post_id, 1);
                await cm.update_comment_count(decoded_post_id, decoded_parent_comment_id, 0);
                return Ok(JsonConvert.SerializeObject(c));
            }
            else
            {
                return BadRequest(new { Message = "Please fill in something" });

            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> delete_comment(string post_id, string comment_id)
        {
            if (ModelState.IsValid)
            {

                Owner user_Session = um.GetUser_Session(Request);
                string decoded_post_id = _helper.DecodeFrom64(post_id);
                string decoded_parent_comment_id = _helper.DecodeFrom64(comment_id);
                Comment comment = cm.getComment(decoded_post_id, decoded_parent_comment_id);
                if (comment.owner._id != user_Session._id)
                {
                    return BadRequest(new { Message = "You are not the owner of the comment" });
                }
                await pm.decrease_sub_commentsAsync(decoded_post_id, comment.comments);
                await cm.delete_commentAsync(decoded_post_id, decoded_parent_comment_id);
                return Ok(new { Message = "Delete sucessfullly" });
            }
            else
            {
                return BadRequest(new { Message = "Please fill in something" });
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> delete_sub_comment(string post_id, string comment_id, string sub_comment_id)
        {
            if (ModelState.IsValid)
            {
                Owner user_Session = um.GetUser_Session(Request);
                string decoded_post_id = _helper.DecodeFrom64(post_id);
                string decoded_parent_comment_id = _helper.DecodeFrom64(comment_id);
                string decoded_sub_comment_id = _helper.DecodeFrom64(sub_comment_id);
                Comment comment = cm.getSubComment(decoded_post_id, decoded_parent_comment_id, decoded_sub_comment_id);
                if (comment.owner._id != user_Session._id)
                {
                    return BadRequest(new { Message = "You are not the owner of the comment" });
                }
                await cm.delete_sub_commentAsync(decoded_post_id, decoded_parent_comment_id, decoded_sub_comment_id);
                await cm.update_comment_count(decoded_post_id, decoded_parent_comment_id, 1);
                return Ok(new { Message = "Delete subcomment sucessfullly" });
            }
            else
            {
                return BadRequest(new { Message = "Please fill in something" });
            }
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string comment_text, string post_id)
        {
            if (comment_text.Equals(""))
            {
                return BadRequest(new { Message = "Please fill in something" });
            }
            if (ModelState.IsValid)
            {
                Owner user_Session = um.GetUser_Session(Request);
                string decoded_post_id = _helper.DecodeFrom64(post_id);
                Comment c = await cm.insertComment(decoded_post_id, new Models.Comment
                {
                    message = comment_text,
                    date_created = DateTime.Now,
                    owner = user_Session
                });
                c.has_like = lm.get_like(c._id, user_Session._id);
                await pm.increase_commentsAsync(decoded_post_id, 1);
                return Ok(JsonConvert.SerializeObject(c));
            }
            else
            {
                return BadRequest(new { Message = "Please fill all fields" });
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> edit_comment(string comment_text, string post_id, string comment_id)
        {
            if (comment_text.Equals(""))
            {
                return BadRequest(new { Message = "Please fill in something" });
            }
            if (ModelState.IsValid)
            {
                string decoded_post_id = _helper.DecodeFrom64(post_id);
                string decoded_parent_comment_id = _helper.DecodeFrom64(comment_id);
                Owner user_Session = um.GetUser_Session(Request);
                Comment comment = cm.getComment(decoded_post_id, decoded_parent_comment_id);
                if (comment.owner._id != user_Session._id)
                {
                    return BadRequest(new { Message = "You are not the owner of the comment" });
                }
                await cm.update_commentAsync(decoded_post_id, decoded_parent_comment_id, comment_text);
                return Ok(comment_text);
            }
            else
            {
                return BadRequest(new { Message = "Please fill all fields" });
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> edit_SubComment(string comment_text, string post_id, string comment_id, string sub_comment_id, DateTime create_time)
        {
            if (comment_text.Equals(""))
            {
                return BadRequest(new { Message = "Please fill in something" });
            }
            if (ModelState.IsValid)
            {
                Owner user_Session = um.GetUser_Session(Request);
                string decoded_post_id = _helper.DecodeFrom64(post_id);
                string decoded_parent_comment_id = _helper.DecodeFrom64(comment_id);
                string decoded_sub_comment_id = _helper.DecodeFrom64(sub_comment_id);
                Comment comment = cm.getSubComment(decoded_post_id, decoded_parent_comment_id, decoded_sub_comment_id);
                if (comment.owner._id != user_Session._id)
                {
                    return BadRequest(new { Message = "You are not the owner of the comment" });
                }
                cm.getSubCommentPosition(decoded_post_id, decoded_parent_comment_id, decoded_sub_comment_id);
                await cm.update_sub_commentAsync(decoded_post_id, decoded_parent_comment_id, decoded_sub_comment_id, comment_text);
                return Ok(comment_text);
            }
            else
            {
                return BadRequest(new { Message = "Please fill in something" });

            }
        }

        [HttpPost]
        public string GetComment(string post_id, string comment_id)
        {
            string decoded_post_id = _helper.DecodeFrom64(post_id);
            string decoded_comment_id = _helper.DecodeFrom64(comment_id);
            Comment comment = cm.getCommentWithoutSubComment(decoded_post_id, decoded_comment_id);
            Owner owner = um.GetUser_Session(Request);
            return JsonConvert.SerializeObject(comment);
        }
    }

}