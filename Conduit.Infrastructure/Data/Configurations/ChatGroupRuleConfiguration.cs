using Conduit.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Conduit.Infrastructure.Data.Configurations
{
    public class ChatGroupRuleConfiguration : IEntityTypeConfiguration<ChatGroupRule>
    {
        public void Configure(EntityTypeBuilder<ChatGroupRule> builder)
        {
            builder.HasKey(x => new { x.ChatGroupID, x.Rule });
        }
    }
}
