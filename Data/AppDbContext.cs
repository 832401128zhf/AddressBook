using AddressBook.Models;
using Microsoft.EntityFrameworkCore;

namespace AddressBook.Data;

/// <summary>数据库上下文（Entity Framework Core + SQLite）</summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<ContactMethod> ContactMethods => Set<ContactMethod>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 一对一（多）关系：一个联系人拥有多个联系方式；删除联系人时级联删除其联系方式
        modelBuilder.Entity<ContactMethod>()
            .HasOne(m => m.Contact)
            .WithMany(c => c.ContactMethods)
            .HasForeignKey(m => m.ContactId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
