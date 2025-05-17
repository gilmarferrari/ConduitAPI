using Conduit.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Conduit.Infrastructure.Data.Configurations
{
    public class SignalRClientConfiguration : IEntityTypeConfiguration<SignalRClient>
    {
        public void Configure(EntityTypeBuilder<SignalRClient> builder)
        {
            builder.HasKey(x => new { x.ConnectionID, x.UserID });
        }
    }
}
