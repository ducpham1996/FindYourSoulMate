using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindYourSoulMateAngular.Models.Entities
{
    public class UserStory
    {
        public string _id;
        public List<Story> user_stories;
    }
    public class Story
    {
        public string _id;
        public int status;
    }
}
