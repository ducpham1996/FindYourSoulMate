using FindYourSoulMate.Models.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using MongoDB.Bson;

namespace FindYourSoulMate.Models.Manager
{
    public class UserManagement
    {
        DataContext dataContext;
        public UserManagement()
        {
            dataContext = new DataContext();
        }

        public Owner GetUser_Session(HttpRequest request)
        {
            Owner user_Session = new Owner();
            foreach (var claim in request.HttpContext.User.Claims)
            {
                if (claim.Type == ClaimTypes.PrimarySid)
                {
                    user_Session._id = claim.Value;
                }
                if (claim.Type == ClaimTypes.Email)
                {
                    user_Session.email = claim.Value;
                }
                if (claim.Type == ClaimTypes.Name)
                {
                    user_Session.user_name = claim.Value;
                }
                if (claim.Type == ClaimTypes.Actor)
                {
                    user_Session.user_picture = claim.Value;
                }
            }
            return user_Session;
        }
        public User GetUser_Detail(string _id)
        {
            var user_collection = dataContext.getConnection().GetCollection<User_Authorization>("User");
            User user = user_collection.AsQueryable().Where(x => x._id == ObjectId.Parse(_id)).Select(x => x.user).First();
            return user;
        }

    }
}
