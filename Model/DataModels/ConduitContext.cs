using Microsoft.EntityFrameworkCore;

namespace Model.DataModels
{
    public class ConduitContext : DbContext
    {
        public ConduitContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<UserActivityLog> UserActivityLogs { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<SignalRClient> SignalRClients { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ChatGroup> ChatGroups { get; set; }
        public DbSet<ChatGroupMessage> ChatGroupMessages { get; set; }
    }
}
