using Conduit.Domain.Entities;
using Conduit.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Conduit.Infrastructure.Data
{
    public class ConduitContext : DbContext
    {
        public ConduitContext(DbContextOptions options) : base(options) { }
        public DbSet<ChatGroup> ChatGroups { get; set; }
        public DbSet<ChatGroupMessage> ChatGroupMessages { get; set; }
        public DbSet<ChatGroupParticipant> ChatGroupParticipants { get; set; }
        public DbSet<ChatGroupRule> ChatGroupRules { get; set; }
        public DbSet<LongLivedToken> LongLivedTokens { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<SignalRClient> SignalRClients { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<UserActivityLog> UserActivityLogs { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ChatGroupParticipantConfiguration());
            modelBuilder.ApplyConfiguration(new ChatGroupRuleConfiguration());
            modelBuilder.ApplyConfiguration(new SignalRClientConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
        }
    }
}
