using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FindYourSoulMate.Models;
using FindYourSoulMate.Models.Entities;
using FindYourSoulMate.Models.Manager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FindYourSoulMateAngular.Controllers
{
    [Produces("application/json")]
    [Route("api/SubComment")]
    public class SubCommentController : Controller
    {
        private Helper _helper;
        private PostManagement pm;
        private CommentManagement cm;
        private UserManagement um;
        private CommentLikeManagement clm;
        public SubCommentController()
        {
            _helper = new Helper();
            pm = new PostManagement();
            cm = new CommentManagement();
            um = new UserManagement();
            clm = new CommentLikeManagement();
        }
        [HttpPost("CreateSubComment")]
        public async Task<Comment> Create_SubComment([FromBody]InsertSubComment comment)
        {
            if (ModelState.IsValid)
            {
                Owner owner = um.GetUser_Cookie(Request);
                string decoded_post_id = _helper.DecodeFrom64(comment.post_id);
                if (comment.sub_post_id != null)
                {
                    await pm.increase_sub_post_sub_comment_countAsync(decoded_post_id, _helper.DecodeFrom64(comment.sub_post_id), 1);
                    decoded_post_id = _helper.DecodeFrom64(comment.sub_post_id);
                }
                else
                {
                    await pm.increase_sub_commentsAsync(decoded_post_id, 1);

                }
                string decoded_parent_comment_id = _helper.DecodeFrom64(comment.parent_comment_id);
                Comment c = await cm.insert_subCommentAsync(decoded_post_id, decoded_parent_comment_id, new Comment
                {
                    message = comment.reply_text,
                    date_created = DateTime.Now,
                    owner = owner
                });
                c.has_like = "none";
                c.is_own = true;
                c._id = _helper.EncodeTo64(c._id);
                await cm.update_comment_count(decoded_post_id, decoded_parent_comment_id, 1);
                return c;
            }
            else
            {
                return null;
            }
        }

        [HttpGet("GetSubComments")]
        [Produces("application/json")]
        public List<Comment> GetSubComments(string post_id,string sub_post_id, string comment_id, int noOfcomment)
        {
            Owner owner = um.GetUser_Cookie(Request);
            string decoded_post_id = _helper.DecodeFrom64(post_id);
            if (sub_post_id != null)
            {
                decoded_post_id = _helper.DecodeFrom64(sub_post_id);
            }
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
                c.has_like = clm.get_like(c._id, owner._id);
                c._id = _helper.EncodeTo64(c._id);
            }
            return comments.ToList();
        }

        [HttpPost("DeleteSubComment")]
        public async Task<IActionResult> delete_sub_comment([FromBody]DeleteSubComment deleteSubComment)
        {
            if (ModelState.IsValid)
            {
                Owner owner = um.GetUser_Cookie(Request);
                string decoded_post_id = _helper.DecodeFrom64(deleteSubComment.post_id);
                string decoded_parent_comment_id = _helper.DecodeFrom64(deleteSubComment.comment_id);
                string decoded_sub_comment_id = _helper.DecodeFrom64(deleteSubComment.sub_comment_id);
                if (deleteSubComment.sub_post_id != null)
                {
                    await pm.increase_sub_post_sub_comment_countAsync(decoded_post_id, _helper.DecodeFrom64(deleteSubComment.sub_post_id), -1);
                    decoded_post_id = _helper.DecodeFrom64(deleteSubComment.sub_post_id);
                }
                else
                {
                    await pm.decrease_sub_commentsAsync(decoded_post_id, 1);
                }
                Comment comment = cm.getSubComment(decoded_post_id, decoded_parent_comment_id, decoded_sub_comment_id);
                if (comment.owner._id != owner._id)
                {
                    return BadRequest(new { Message = "You are not the owner of the comment" });
                }
                await cm.delete_sub_commentAsync(decoded_post_id, decoded_parent_comment_id, decoded_sub_comment_id);
                await cm.update_comment_count(decoded_post_id, decoded_parent_comment_id, -1);
                return Ok(new { Message = "Delete subcomment sucessfully" });
            }
            else
            {
                return BadRequest(new { Message = "Please fill in something" });
            }
        }
        [HttpPost("EditSubComment")]
        public async Task<IActionResult> edit_SubComment([FromBody]EditSubComment editSubComment)
        {
            if (editSubComment.comment_text.Equals(""))
            {
                return BadRequest(new { Message = "Please fill in something" });
            }
            if (ModelState.IsValid)
            {
                Owner owner = um.GetUser_Cookie(Request);
                string decoded_post_id = _helper.DecodeFrom64(editSubComment.post_id);
                if (editSubComment.sub_post_id != null)
                {
                    decoded_post_id = _helper.DecodeFrom64(editSubComment.sub_post_id);
                }
                string decoded_parent_comment_id = _helper.DecodeFrom64(editSubComment.comment_id);
                string decoded_sub_comment_id = _helper.DecodeFrom64(editSubComment.sub_comment_id);
                Comment comment = cm.getSubComment(decoded_post_id, decoded_parent_comment_id, decoded_sub_comment_id);
                if (comment.owner._id != owner._id)
                {
                    return BadRequest(new { Message = "You are not the owner of the comment" });
                }
                cm.getSubCommentPosition(decoded_post_id, decoded_parent_comment_id, decoded_sub_comment_id);
                await cm.update_sub_commentAsync(decoded_post_id, decoded_parent_comment_id, decoded_sub_comment_id, editSubComment.comment_text);
                return Ok("Edit successfully");
            }
            else
            {
                return BadRequest(new { Message = "Please fill in something" });

            }
        }
    }


    public class InsertSubComment
    {
        public string post_id { get; set; }
        public string sub_post_id { get; set; }
        public string parent_comment_id { get; set; }
        public string reply_text { get; set; }
    }
    public class EditSubComment
    {
        public string post_id { get; set; }
        public string sub_post_id { get; set; }
        public string comment_id { get; set; }
        public string sub_comment_id { get; set; }
        public string comment_text { get; set; }
    }
    public class DeleteSubComment
    {
        public string post_id { get; set; }
        public string sub_post_id { get; set; }
        public string comment_id { get; set; }
        public string sub_comment_id { get; set; }
    }
}