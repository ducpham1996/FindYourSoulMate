using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    [Route("api/Comment")]
    public class CommentController : Controller
    {
        private Helper _helper;
        private PostManagement pm;
        private CommentManagement cm;
        private UserManagement um;
        private CommentLikeManagement clm;
        public CommentController()
        {
            _helper = new Helper();
            pm = new PostManagement();
            cm = new CommentManagement();
            um = new UserManagement();
            clm = new CommentLikeManagement();
        }

        [HttpPost("InsertComment")]
        [Produces("application/json")]
        public async Task<Comment> Create_CommentAsync([FromBody]InsertComment comment)
        {
            Comment c = new Comment();
            if (ModelState.IsValid)
            {
                Owner owner = um.GetUser_Cookie(Request);
                string decoded_post_id = _helper.DecodeFrom64(comment.post_id);
                PostParticipantManagement ppm = new PostParticipantManagement();
                if (comment.sub_post_id != null)
                {
                    decoded_post_id = _helper.DecodeFrom64(comment.sub_post_id);
                    await ppm.send_email_to_participant_sub_post_comment(_helper.DecodeFrom64(comment.post_id), decoded_post_id, owner, 0);
                    await pm.increase_sub_post_comment_countAsync(_helper.DecodeFrom64(comment.post_id), decoded_post_id, 1);
                }
                else
                {
                    await ppm.send_email_to_participant_post_comment(decoded_post_id, owner, 0);
                    await pm.increase_commentsAsync(decoded_post_id, 1);
                }
                c = await cm.insertComment(decoded_post_id, new Comment
                {
                    message = comment.comment_text,
                    date_created = DateTime.Now,
                    owner = owner
                });
                c._id = _helper.EncodeTo64(c._id);
                c.has_like = "none";
                c.is_own = true;
                MailManager mm = new MailManager();
                if (!ppm.has_participated(decoded_post_id, owner._id))
                {
                    await ppm.insertPostParticipantRecord(decoded_post_id, new Participant { _id = owner._id, email = owner.email, status = true });
                }
            }
            return c;
        }


        [HttpGet("GetComments")]
        [Produces("application/json")]
        public List<Comment> GetComments(string post_id, string sub_post_id, int noOfcomment)
        {
            string decoded_id = _helper.DecodeFrom64(post_id);
            if (sub_post_id != null)
            {
                decoded_id = _helper.DecodeFrom64(sub_post_id);
            }
            int skip = 5 * (noOfcomment - 1);
            IEnumerable<Comment> comments = cm.GetComments(decoded_id, 5, skip);
            Owner owner = um.GetUser_Cookie(Request);
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
                c.no_of_comment = (int)Math.Ceiling(c.comments * 1.0 / 5);
            }
            return comments.ToList();
        }

        [HttpPost("DeleteComment")]
        public async Task<IActionResult> delete_comment([FromBody]DeleteComment deleteComment)
        {
            if (ModelState.IsValid)
            {
                Owner owner = um.GetUser_Cookie(Request);
                string decoded_post_id = _helper.DecodeFrom64(deleteComment.post_id);
                string decoded_parent_comment_id = _helper.DecodeFrom64(deleteComment.comment_id);
                if (deleteComment.sub_post_id != null)
                {
                    decoded_post_id = _helper.DecodeFrom64(deleteComment.sub_post_id);
                }
                Comment comment = cm.getComment(decoded_post_id, decoded_parent_comment_id);
                if (comment.owner._id != owner._id)
                {
                    return BadRequest(new { Message = "You are not the owner of the comment" });
                }
                if (deleteComment.sub_post_id != null)
                {
                    decoded_post_id = _helper.DecodeFrom64(deleteComment.sub_post_id);
                    await pm.increase_sub_post_comment_countAsync(_helper.DecodeFrom64(deleteComment.post_id), decoded_post_id, -1);
                }
                else
                {
                    await pm.decrease_commentsAsync(decoded_post_id, 1);
                    if (comment.comments > 0)
                    {
                        await pm.decrease_sub_commentsAsync(decoded_post_id, comment.comments);
                    }
                }
                await cm.delete_commentAsync(decoded_post_id, decoded_parent_comment_id);
                return Ok(new { Message = "Delete sucessfullly" });
            }
            else
            {
                return BadRequest(new { Message = "Please fill in something" });
            }
        }
        [HttpPost("EditComment")]
        public async Task<IActionResult> edit_comment([FromBody] EditComment editComment)
        {
            if (editComment.comment_text.Equals(""))
            {
                return BadRequest(new { Message = "Please fill in something" });
            }
            if (ModelState.IsValid)
            {
                string decoded_post_id = _helper.DecodeFrom64(editComment.post_id);
                if (editComment.sub_post_id != null)
                {
                    decoded_post_id = _helper.DecodeFrom64(editComment.sub_post_id);
                }
                string decoded_parent_comment_id = _helper.DecodeFrom64(editComment.comment_id);
                Owner owner = um.GetUser_Cookie(Request);
                Comment comment = cm.getComment(decoded_post_id, decoded_parent_comment_id);
                if (comment.owner._id != owner._id)
                {
                    return BadRequest(new { Message = "You are not the owner of the comment" });
                }
                await cm.update_commentAsync(decoded_post_id, decoded_parent_comment_id, editComment.comment_text);
                return Ok("Edit successfully");
            }
            else
            {
                return BadRequest(new { Message = "Please fill all fields" });
            }
        }

    }



    public class InsertComment
    {

        public string post_id { get; set; }
        public string sub_post_id { get; set; }
        public string comment_text { get; set; }
    }
    public class DeleteComment
    {
        public string post_id { get; set; }
        public string sub_post_id { get; set; }
        public string comment_id { get; set; }
    }

    public class Cookie
    {
        [JsonProperty("contentType")]
        public string contentType { get; set; }
        [JsonProperty("serializerSettings")]
        public string serializerSettings { get; set; }
        [JsonProperty("statusCode")]
        public string statusCode { get; set; }
        [JsonProperty("value")]
        public Cooike_Value value { get; set; }
    }
    public class Cooike_Value
    {
        [JsonProperty("data")]
        public Owner data { get; set; }
    }
    public class EditComment
    {
        public string comment_text { get; set; }
        public string post_id { get; set; }
        public string sub_post_id { get; set; }
        public string comment_id { get; set; }
    }
}