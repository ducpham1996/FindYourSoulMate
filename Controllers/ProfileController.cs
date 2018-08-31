using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FindYourSoulMate.Models.Entities;
using FindYourSoulMate.Models.Manager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FindYourSoulMateAngular.Controllers
{
    [Produces("application/json")]
    [Route("api/Profile")]
    public class ProfileController : Controller
    {

        private UserManagement um;
        private Helper helper;

        public ProfileController()
        {
            um = new UserManagement();
            helper = new Helper();
        }

        [HttpGet("getProfile")]
        [Produces("application/json")]
        public User profile(string _id)
        {
            try
            {
                Debug.WriteLine(_id);
                string decoded_id = helper.DecodeFrom64(_id);
                Debug.WriteLine(decoded_id);
                return um.GetUser_Detail(decoded_id);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return new User();
            }
        }
    }
}