﻿using Microsoft.EntityFrameworkCore;
using Model.DataModels;

namespace Services
{
    public interface ISignalRService
    {
        Task ClearClients();
    }
    public class SignalRService : ISignalRService
    {
        private readonly ConduitContext _context;

        public SignalRService(ConduitContext context)
        {
            _context = context;
        }

        public async Task ClearClients()
        {
            var clients = await _context.SignalRClients.ToListAsync();
            _context.SignalRClients.RemoveRange(clients);

            await _context.SaveChangesAsync();
        }
    }
}
