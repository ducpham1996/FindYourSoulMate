using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FindYourSoulMate.Models;
using FindYourSoulMate.Models.Entities;
using FindYourSoulMate.Models.Manager;
using System.Diagnostics;
using System.Net.Http;
using System.Net;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Security.Cryptography;
using FindYourSoulMate.Models.Exception;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using MongoDB.Driver;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using System.Threading;

namespace FindYourSoulMate.Controllers
{
    public class UsersController : Controller
    {
        private readonly FindYourSoulMateContext _context;
        private UserManagement um;
        private DataContext data;
        private CheckPassword checkPassword;
        private IHostingEnvironment _env;

        public UsersController(IHostingEnvironment env)
        {
            um = new UserManagement();
            data = new DataContext();
            checkPassword = new CheckPassword();
            _env = env;

        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.User.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .SingleOrDefaultAsync(m => m.id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("User");
            return RedirectToAction("Login");

        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("email,password,remember")] Login login)
        {
            Debug.WriteLine(login.remember);
            if (ModelState.IsValid)
            {
                try
                {
                    User_Authorization u = checkPassword.check_password(login.email, login.password);
                    var claims = new List<Claim>
                             {
                                new Claim(ClaimTypes.PrimarySid,u._id.ToString() ),
                                new Claim(ClaimTypes.Email,u.user.email),
                                 new Claim(ClaimTypes.Name,u.user.first_name + " "+u.user.last_name),
                                 new Claim(ClaimTypes.Actor,u.user.profile_img),
                                 new Claim(ClaimTypes.Role, "User")
                            };

                    var userIdentity = new ClaimsIdentity(claims, "User");
                    ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);

                    if (login.remember)
                    {
                        //User_Session user_Session = new User_Session { user_name = u.user.first_name + " " + u.user.last_name, _id = u._id, profile_pic = u.user.profile_img };
                        //ISession session = HttpContext.Session;
                        //session.SetString("user", JsonConvert.SerializeObject(user_Session));
                        //var value = session.GetString("user");
                        //User_Session user = JsonConvert.DeserializeObject<User_Session>(value);
                        //Debug.WriteLine(user.user_name);
                        await HttpContext.SignInAsync(
                            scheme: "User",
                            principal: principal,
                            properties: new AuthenticationProperties
                            {
                                IsPersistent = true,
                                ExpiresUtc = DateTime.UtcNow.AddDays(30)
                            });
                    }
                    else
                    {
                        await HttpContext.SignInAsync(scheme: "User", principal: principal);
                    }
                    var claimsPrincipal = new ClaimsPrincipal(userIdentity);
                    // Set current principal
                    Thread.CurrentPrincipal = claimsPrincipal;
                    return Ok("/Posts");
                }
                catch (AccountIsNotExistException accountnotexist)
                {
                    return BadRequest(new { Message = accountnotexist.Message });
                }
                catch (InvalidUserNameAndPasswordException invalidUserName)
                {
                    return BadRequest(new { Message = invalidUserName.Message });
                }
            }
            else
            {
                return BadRequest(new { Message = "Please fill all fields" });
            }
        }


        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,first_name,last_name,birth_date,profile_img,profile_background,gender,phone_number,email,password,reenter_password")] User user, IFormFile profile_img)
        {
            if (ModelState.IsValid)
            {
                if (user.password.Equals(user.reenter_password))
                {
                    var users = data.getConnection().GetCollection<User_Authorization>("User");
                    var check_user = users.Find(x => x.authorization.username == user.email).SingleAsync();
                    try
                    {
                        User_Authorization au = check_user.Result;
                        return BadRequest(new { Message = "Email is already existed" });
                    }
                    catch (AggregateException ae)
                    {
                    }
                    // specify that we want to randomly generate a 20-byte salt
                    using (var deriveBytes = new Rfc2898DeriveBytes(user.password, 20))
                    {
                        byte[] salt = deriveBytes.Salt;
                        byte[] key = deriveBytes.GetBytes(20);  // derive a 20-byte key
                        Models.Authorization autho = new Models.Authorization { username = user.email, salt = salt, key = key };
                        user.created_date = DateTime.Now;
                        user.last_login = DateTime.Now;
                        if (profile_img != null)
                        {
                            var path = _env.WebRootPath;
                            var uploads = Path.Combine(path, "img");
                            user.profile_img = "/img/" + save_fileAsync(uploads, profile_img);
                        }
                        else
                        {
                            user.profile_img = "/img/unknown.jpg";
                        }
                        await users.InsertOneAsync(new User_Authorization { authorization = autho, user = user });
                        // save salt and key to database
                    }
                    return Ok("Insert successfully");
                }
                else
                {
                    return BadRequest(new { Message = "Password and Re-enter password must match" });
                }
            }
            else
            {
                return BadRequest(new { Message = "Please fill all fields" });
            }
        }


        private async Task<string> save_fileAsync(string save_folder, IFormFile profile_img)
        {
            string file_name = DateTime.Now.Ticks + ".jpg";
            var filePath = Path.Combine(save_folder, file_name);
            Debug.WriteLine(filePath);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await profile_img.CopyToAsync(stream);
            }
            return file_name;
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.SingleOrDefaultAsync(m => m.id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("id,first_name,last_name,birth_date,profile_img,profile_background,gender,phone_number,email,password,reenter_password")] User user)
        {
            if (id != user.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.id))
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
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .SingleOrDefaultAsync(m => m.id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.User.SingleOrDefaultAsync(m => m.id == id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(string id)
        {
            return _context.User.Any(e => e.id == id);
        }
    }
}
