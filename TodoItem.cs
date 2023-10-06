using Microsoft.EntityFrameworkCore;

namespace TODOApp.Models
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string? Title { get; set; }
    }

    class TodoDb: DbContext
    {
        public TodoDb(DbContextOptions options) : base(options) { }
        public DbSet<TodoItem> Todos { get; set; } = null!;
    }
}
