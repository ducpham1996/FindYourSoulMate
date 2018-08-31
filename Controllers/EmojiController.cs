using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FindYourSoulMate.Models.Entities;
using FindYourSoulMate.Models.Manager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FindYourSoulMateAngular.Controllers
{
    [Produces("application/json")]
    [Route("api/Emoji")]
    public class EmojiController : Controller
    {
        private Helper _helper;
        private PostManagement pm;
        private CommentManagement cm;
        private UserManagement um;
        private CommentLikeManagement clm;
        public EmojiController()
        {
            _helper = new Helper();
            pm = new PostManagement();
            cm = new CommentManagement();
            um = new UserManagement();
            clm = new CommentLikeManagement();
        }
        [HttpPost("PostLike")]
        [Produces("application/json")]
        public async Task<IActionResult> post_like([FromBody] PostLike postLike)
        {
            if (ModelState.IsValid)
            {
                Owner user_Session = um.GetUser_Cookie(Request);
                string decoded_post_id = _helper.DecodeFrom64(postLike.post_id);
                Like_Record like_Record = new Like_Record
                {
                    emoji = postLike.emo,
                    user_name = user_Session.user_name,
                    user_picture = user_Session.user_picture,
                    _id = user_Session._id
                };
                if (postLike.sub_post_id != null)
                {
                    await sub_post_like(decoded_post_id, _helper.DecodeFrom64(postLike.sub_post_id), postLike.emo, user_Session);
                }
                else
                {
                    PostLikeManagement plm = new PostLikeManagement();
                    if (plm.has_like(decoded_post_id, like_Record))
                    {
                        if (plm.get_like(decoded_post_id, user_Session._id) == postLike.emo)
                        {
                            await plm.remove_post_like_record(decoded_post_id, like_Record);
                            await plm.update_post_like_count(decoded_post_id, postLike.emo, -1);
                        }
                        else
                        {
                            await plm.update_post_like_count(decoded_post_id, plm.get_like(decoded_post_id, user_Session._id), -1);
                            await plm.update_post_like_count(decoded_post_id, postLike.emo, 1);
                            await plm.update_post_like_record(decoded_post_id, like_Record);
                        }
                    }
                    else
                    {
                        await plm.insertLikeRecord(decoded_post_id, like_Record);
                        await plm.update_post_like_count(decoded_post_id, postLike.emo, 1);
                    }
                }


                return Ok();
            }
            else
            {
                return BadRequest(new { Message = "Missing fields" });
            }
        }

        public async Task sub_post_like(string post_id, string sub_post_id, string emo, Owner user_Session)
        {
            Like_Record like_Record = new Like_Record
            {
                emoji = emo,
                user_name = user_Session.user_name,
                user_picture = user_Session.user_picture,
                _id = user_Session._id
            };
            PostLikeManagement plm = new PostLikeManagement();
            if (plm.has_like(sub_post_id, like_Record))
            {
                if (plm.get_like(sub_post_id, user_Session._id) == emo)
                {
                    await plm.remove_post_like_record(sub_post_id, like_Record);
                    await plm.update_sub_post_like_count(post_id, sub_post_id, emo, -1);
                }
                else
                {
                    await plm.update_sub_post_like_count(post_id, sub_post_id, plm.get_like(sub_post_id, user_Session._id), -1);
                    await plm.update_sub_post_like_count(post_id, sub_post_id, emo, 1);
                    await plm.update_post_like_record(sub_post_id, like_Record);
                }
            }
            else
            {
                await plm.insertLikeRecord(sub_post_id, like_Record);
                await plm.update_sub_post_like_count(post_id, sub_post_id, emo, 1);
            }
        }


        [HttpPost("CommentLike")]
        [Produces("application/json")]
        public async Task<IActionResult> comment_like([FromBody] CommentLike commentLike)
        {
            if (ModelState.IsValid)
            {
                Owner user_Session = um.GetUser_Cookie(Request);
                string decoded_post_id = _helper.DecodeFrom64(commentLike.post_id);
                if (commentLike.sub_post_id != null)
                {
                    decoded_post_id = _helper.DecodeFrom64(commentLike.sub_post_id);
                }
                string decoded_comment_id = _helper.DecodeFrom64(commentLike.comment_id);
                Like_Record like_Record = new Like_Record
                {
                    emoji = commentLike.emo,
                    user_name = user_Session.user_name,
                    user_picture = user_Session.user_picture,
                    _id = user_Session._id
                };
                if (clm.has_like(decoded_comment_id, like_Record))
                {
                    if (clm.get_like(decoded_comment_id, user_Session._id) == commentLike.emo)
                    {
                        await clm.remove_like_record(decoded_comment_id, like_Record);
                        await clm.update_comment_likeAsync(decoded_post_id, decoded_comment_id, commentLike.emo, -1);
                    }
                    else
                    {
                        await clm.update_comment_likeAsync(decoded_post_id, decoded_comment_id, clm.get_like(decoded_comment_id, user_Session._id), -1);
                        await clm.update_comment_likeAsync(decoded_post_id, decoded_comment_id, commentLike.emo, 1);
                        await clm.update_like_record(decoded_comment_id, like_Record);
                    }
                }
                else
                {
                    await clm.insertLikeRecord(decoded_comment_id, like_Record);
                    await clm.update_comment_likeAsync(decoded_post_id, decoded_comment_id, commentLike.emo, 1);
                }
                return Ok();
            }
            else
            {
                return BadRequest(new { Message = "Missing fields" });
            }
        }
        [HttpPost("SubCommentLike")]
        [Produces("application/json")]
        public async Task<IActionResult> sub_comment_like([FromBody]SubCommentLike subCommentLike)
        {
            if (ModelState.IsValid)
            {
                Owner user_Session = um.GetUser_Cookie(Request);
                string decoded_post_id = _helper.DecodeFrom64(subCommentLike.post_id);
                if (subCommentLike.sub_post_id != null)
                {
                    decoded_post_id = _helper.DecodeFrom64(subCommentLike.sub_post_id);
                }
                string decoded_comment_id = _helper.DecodeFrom64(subCommentLike.comment_id);
                Like_Record like_Record = new Like_Record
                {
                    emoji = subCommentLike.emo,
                    user_name = user_Session.user_name,
                    user_picture = user_Session.user_picture,
                    _id = user_Session._id
                };
                string decoded_sub_comment_id = _helper.DecodeFrom64(subCommentLike.sub_comment_id);
                if (clm.has_like(decoded_sub_comment_id, like_Record))
                {
                    if (clm.get_like(decoded_sub_comment_id, user_Session._id) == subCommentLike.emo)
                    {
                        await clm.remove_like_record(decoded_sub_comment_id, like_Record);
                        await clm.update_sub_comment_likeAsync(decoded_post_id, decoded_comment_id, decoded_sub_comment_id, subCommentLike.emo, -1);
                    }
                    else
                    {
                        await clm.update_sub_comment_likeAsync(decoded_post_id, decoded_comment_id, decoded_sub_comment_id, clm.get_like(decoded_sub_comment_id, user_Session._id), -1);
                        await clm.update_sub_comment_likeAsync(decoded_post_id, decoded_comment_id, decoded_sub_comment_id, subCommentLike.emo, 1);
                        await clm.update_like_record(decoded_sub_comment_id, like_Record);
                    }
                }
                else
                {
                    await clm.insertLikeRecord(decoded_sub_comment_id, like_Record);
                    await clm.update_sub_comment_likeAsync(decoded_post_id, decoded_comment_id, decoded_sub_comment_id, subCommentLike.emo, 1);
                }

                return Ok();
            }
            else
            {
                return BadRequest(new { Message = "Missing fields" });
            }
        }
        public class PostLike
        {
            public string post_id { get; set; }

            public string sub_post_id { get; set; }
            public string emo { get; set; }
        }
        public class CommentLike
        {
            public string post_id { get; set; }
            public string sub_post_id { get; set; }
            public string comment_id { get; set; }
            public string emo { get; set; }
        }
        public class SubCommentLike
        {
            public string post_id { get; set; }
            public string sub_post_id { get; set; }
            public string comment_id { get; set; }
            public string sub_comment_id { get; set; }
            public string emo { get; set; }
        }
    }
}