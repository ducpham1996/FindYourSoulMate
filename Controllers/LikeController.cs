using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FindYourSoulMate.Models.Entities;
using FindYourSoulMate.Models.Manager;
using Microsoft.AspNetCore.Mvc;

namespace FindYourSoulMate.Controllers
{
    public class LikeController : Controller
    {
        private UserManagement um;
        private Helper _helper;
        private CommentLikeManagement lm;
        public LikeController()
        {
            um = new UserManagement();
            _helper = new Helper();
            lm = new CommentLikeManagement();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> comment_like(string post_id, string comment_id, string sub_comment_id, string emo)
        {
            if (ModelState.IsValid)
            {
                Owner user_Session = um.GetUser_Session(Request);
                string decoded_post_id = _helper.DecodeFrom64(post_id);
                string decoded_comment_id = _helper.DecodeFrom64(comment_id);
                Like_Record like_Record = new Like_Record
                {
                    emoji = emo,
                    user_name = user_Session.user_name,
                    user_picture = user_Session.user_picture,
                    _id = user_Session._id
                };
                if (sub_comment_id != null)
                {
                    string decoded_sub_comment_id = _helper.DecodeFrom64(sub_comment_id);
                    if (lm.has_like(decoded_sub_comment_id, like_Record))
                    {
                        if (lm.get_like(decoded_sub_comment_id, user_Session._id) == emo)
                        {
                            await lm.remove_like_record(decoded_sub_comment_id, like_Record);
                            await lm.update_sub_comment_likeAsync(decoded_post_id, decoded_comment_id, decoded_sub_comment_id, emo, -1);
                        }
                        else
                        {
                            await lm.update_sub_comment_likeAsync(decoded_post_id, decoded_comment_id, decoded_sub_comment_id, lm.get_like(decoded_sub_comment_id, user_Session._id), -1);
                            await lm.update_sub_comment_likeAsync(decoded_post_id, decoded_comment_id, decoded_sub_comment_id, emo, 1);
                            await lm.update_like_record(decoded_sub_comment_id, like_Record);
                        }
                    }
                    else
                    {
                        await lm.insertLikeRecord(decoded_sub_comment_id, like_Record);
                        await lm.update_sub_comment_likeAsync(decoded_post_id, decoded_comment_id, decoded_sub_comment_id, emo, 1);
                    }
                }
                else
                {

                    if (lm.has_like(decoded_comment_id, like_Record))
                    {
                        if (lm.get_like(decoded_comment_id, user_Session._id) == emo)
                        {
                            await lm.remove_like_record(decoded_comment_id, like_Record);
                            await lm.update_comment_likeAsync(decoded_post_id, decoded_comment_id, emo, -1);
                        }
                        else
                        {
                            await lm.update_comment_likeAsync(decoded_post_id, decoded_comment_id, lm.get_like(decoded_comment_id, user_Session._id), -1);
                            await lm.update_comment_likeAsync(decoded_post_id, decoded_comment_id, emo, 1);
                            await lm.update_like_record(decoded_comment_id, like_Record);
                        }
                    }
                    else
                    {
                        await lm.insertLikeRecord(decoded_comment_id, like_Record);
                        await lm.update_comment_likeAsync(decoded_post_id, decoded_comment_id, emo, 1);
                    }
                }
                return Ok();
            }
            else
            {
                return BadRequest(new { Message = "Missing fields" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> post_like(string post_id,string sub_post_id, string emo)
        {
            if (ModelState.IsValid)
            {
                Owner user_Session = um.GetUser_Session(Request);
                string decoded_post_id = _helper.DecodeFrom64(post_id);
                Like_Record like_Record = new Like_Record
                {
                    emoji = emo,
                    user_name = user_Session.user_name,
                    user_picture = user_Session.user_picture,
                    _id = user_Session._id
                };
                PostLikeManagement plm = new PostLikeManagement();
                Debug.WriteLine(decoded_post_id);
                Debug.WriteLine(plm.has_like(decoded_post_id, like_Record));
                if (plm.has_like(decoded_post_id, like_Record))
                {
                    if (plm.get_like(decoded_post_id, user_Session._id) == emo)
                    {
                        await plm.remove_post_like_record(decoded_post_id, like_Record);
                        await plm.update_post_like_count(decoded_post_id, emo, -1);
                    }
                    else
                    {
                        await plm.update_post_like_count(decoded_post_id, plm.get_like(decoded_post_id, user_Session._id), -1);
                        await plm.update_post_like_count(decoded_post_id, emo, 1);
                        await plm.update_post_like_record(decoded_post_id, like_Record);
                    }
                }
                else
                {
                    await plm.insertLikeRecord(decoded_post_id, like_Record);
                    await plm.update_post_like_count(decoded_post_id, emo, 1);
                }

                return Ok();
            }
            else
            {
                return BadRequest(new { Message = "Missing fields" });
            }


        }
    }
}