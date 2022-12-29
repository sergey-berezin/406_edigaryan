using Microsoft.EntityFrameworkCore;

namespace WPF2
{
    public class ImagesContext : DbContext
    {
        public ImagesContext()
        {
            Database.EnsureCreated();
        }

        public DbSet<ImageEntry> Images { get; set; }

        public DbSet<ImageData> Data { get; set; }

        public void Clear()
        {
            Images.RemoveRange(Images);
            Data.RemoveRange(Data);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder o)
        {
            o.UseSqlite("Data Source=images.db");
        }
    }
}
