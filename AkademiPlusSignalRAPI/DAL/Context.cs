using Microsoft.EntityFrameworkCore;

namespace AkademiPlusSignalRAPI.DAL
{
    public class Context:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DERYA; initial catalog=SignalRDb; integrated security=true");
        }
        public DbSet<User> User { get; set; }
        public DbSet<Room> Rooms { get; set; }
    }
}
