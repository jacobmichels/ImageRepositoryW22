using ImageRepositoryW22.Repository.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageRepositoryW22.Repository
{
    public class ApplicationDbContext: DbContext
    {
        public DbSet<ApplicationUser> Users {  get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {

        }
    }
}
