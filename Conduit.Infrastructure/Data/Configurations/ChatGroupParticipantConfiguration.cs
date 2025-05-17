using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Conduit.Domain.Entities;

namespace Conduit.Infrastructure.Data.Configurations
{
    public class ChatGroupParticipantConfiguration : IEntityTypeConfiguration<ChatGroupParticipant>
    {
        public void Configure(EntityTypeBuilder<ChatGroupParticipant> builder)
        {
            builder.HasKey(x => new { x.ChatGroupID, x.ParticipantID });
        }
    }
}
