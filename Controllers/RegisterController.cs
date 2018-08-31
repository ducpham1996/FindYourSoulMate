using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using FindYourSoulMate.Models;
using FindYourSoulMate.Models.Entities;
using FindYourSoulMateAngular.Models.Manager;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FindYourSoulMateAngular.Controllers
{

    [Route("api/Register")]

    public class RegisterController : Controller
    {
        private IHostingEnvironment _env;
        public RegisterController(IHostingEnvironment env)
        {
            _env = env;
        }
        [Produces("application/json")]
        [HttpPost("CheckEmail")]
        public IActionResult checkEmailExisten([FromBody]Email email)
        {
            RegistraionManagement rm = new RegistraionManagement();
            if (rm.checkEmailIsExist(email.email))
            {
                return BadRequest("Email is existed");
            }
            return Ok();
        }

        [HttpPost("Registration")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> registrationAsync([FromForm] Register register)
        {
            try
            {
                RegistraionManagement rm = new RegistraionManagement();
                using (var deriveBytes = new Rfc2898DeriveBytes(register.password, 20))
                {
                    byte[] salt = deriveBytes.Salt;
                    byte[] key = deriveBytes.GetBytes(20);  // derive a 20-byte key
                    Authorization autho = new Authorization { username = register.email, salt = salt, key = key };
                    User user = new User
                    {
                        created_date = DateTime.Now,
                        last_login = DateTime.Now,
                        last_name = register.last_name,
                        first_name = register.first_name,
                        birth_date = register.birth_date,
                        gender = register.gender,
                        email = register.email,
                        phone_number = register.phone_number
                    };
                    if (register.profile_img != null)
                    {
                        var path = _env.WebRootPath;
                        var uploads = Path.Combine(path, "files");
                        string extension = System.IO.Path.GetExtension(register.profile_img.FileName);
                        string file_name = DateTime.Now.Ticks + extension;
                        await Save_fileAsync(file_name, uploads, register.profile_img);
                        user.profile_img = "/files/" + file_name;
                    }
                    else
                    {
                        user.profile_img = "/files/unknown.png";
                    }
                    await rm.registrationAsync(new User_Authorization { authorization = autho, user = user });
                    // save salt and key to database
                }
                return Ok("Registration successfully");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return BadRequest("Error from server");
            }
        }

        private async Task Save_fileAsync(string file_name, string save_folder, IFormFile profile_img)
        {

            var filePath = Path.Combine(save_folder, file_name);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await profile_img.CopyToAsync(stream);
            }
        }
    }
    public class Email
    {
        public string email { get; set; }
    }
    public class Register
    {
        public string first_name { get; set; }

        public string last_name { get; set; }

        public DateTime birth_date { get; set; }

        public IFormFile profile_img { get; set; }

        public int gender { get; set; }

        public string phone_number { get; set; }

        public string email { get; set; }

        public string password { get; set; }
    }
}