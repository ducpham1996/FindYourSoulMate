using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FindYourSoulMate.Models.Entities;
using FindYourSoulMate.Models;

namespace FindYourSoulMate.Models
{
    public class FindYourSoulMateContext : DbContext
    {
        public FindYourSoulMateContext (DbContextOptions<FindYourSoulMateContext> options)
            : base(options)
        {
        }

        public DbSet<FindYourSoulMate.Models.Entities.User> User { get; set; }

        public DbSet<FindYourSoulMate.Models.Post> Post { get; set; }
    }
}
