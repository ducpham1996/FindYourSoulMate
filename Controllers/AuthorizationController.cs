using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FindYourSoulMate.Models;
using FindYourSoulMate.Models.Entities;
using FindYourSoulMate.Models.Manager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FindYourSoulMateAngular.Controllers
{
    [Produces("application/json")]
    [Route("api/Authorization")]
    public class AuthorizationController : Controller
    {
        [HttpPost("Authorization")]
        public IActionResult Authorization([FromBody]Login login)
        {
            CheckPassword cp = new CheckPassword();
            Helper helper = new Helper();
            try
            {
                User_Authorization authorized_user = cp.check_password(login.UserName, login.PassWord);
                Owner userCookie = new Owner
                {
                    _id = helper.EncodeTo64(authorized_user._id.ToString()),
                    user_name = authorized_user.user.first_name + " " + authorized_user.user.last_name,
                    email = authorized_user.user.email,
                    user_picture = authorized_user.user.profile_img
                };
                return Ok(Json(new { data = userCookie }));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }

}