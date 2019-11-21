using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EntityFrameworkCoreProblemWithQueryTags
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new MyContext())
            {
                context.Database.EnsureCreated();

                context.Jobs
                    .TagWith("This tag must show up!")
                    .ToArray();

                context.Users
                    .TagWith("This tag must be missing!")
                    .ToArray();
            }
        }
    }

    public class PowerUser : User
    {
        public int Level { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Job
    {
        public int Id { get; set; }
    }

    class MyContext : DbContext
    {
        private const string ConnectionString = "server=localhost;port=5432;database=querytagsdb;username=postgres;password=password";

        public DbSet<User> Users { get; set; }
        public DbSet<Job> Jobs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(ConnectionString)
                .UseLoggerFactory(LoggerFactory.Create(x => x.AddConsole()));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasDiscriminator<bool>("ispoweruser")
                .HasValue<User>(false)
                .HasValue<PowerUser>(true);
        }
    }
}
