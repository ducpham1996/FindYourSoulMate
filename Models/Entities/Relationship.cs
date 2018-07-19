using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FindYourSoulMate.Models.Entities
{
    public class Relationship
    {
        public string user_one_id { get; set; }

        public string user_two_id { get; set; }
        //0 Pending
        //1	Accepted
        //2	Declined
        //3	Blocked
        public int status { get; set; }
    }
}
