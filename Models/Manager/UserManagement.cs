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
using FindYourSoulMateAngular.Controllers;
using Newtonsoft.Json;

namespace FindYourSoulMate.Models.Manager
{
    public class UserManagement
    {
        DataContext dataContext;
        Helper _helper;
        public UserManagement()
        {
            dataContext = new DataContext();
            _helper = new Helper();
        }

        public Owner GetUser_Cookie(HttpRequest request)
        {
            Cookie cookie = JsonConvert.DeserializeObject<Cookie>(request.Cookies["User"]);
            Owner author = cookie.value.data;
            author._id = _helper.DecodeFrom64(author._id);
            return author;
        }
        public User GetUser_Detail(string _id)
        {
            var user_collection = dataContext.getConnection().GetCollection<User_Authorization>("User");
            User user = user_collection.AsQueryable().Where(x => x._id == ObjectId.Parse(_id)).Select(x => x.user).First();
            return user;
        }
        public List<Post> getUserPosts(string user_id)
        {
            var post_collection = dataContext.getConnection().GetCollection<Post>("Post");
            var filter = Builders<Post>.Filter.Where(x => x.owner._id == user_id);
            var posts = post_collection.Find(filter).ToEnumerable().ToList();
            return posts;
        }

    }
}
