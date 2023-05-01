using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exchange.Application.Common.Interfaces;
using Exchange.Application.Interfaces;
using Exchange.Domain.Events;
using Exchange.Domain.ValueObjects;
using Exchange.RabbitMQBus.Bus;
using Microsoft.EntityFrameworkCore;

namespace Exchange.Application.IntegrationEvents;
public class RegisterClientEventHandler : IEventHandler<RegisterClientEvent>, ISelfTransientLifetime
{
    private readonly IApplicationDbContext _context;
    public RegisterClientEventHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task Handle(RegisterClientEvent @event)
    {
        var user = _context.UserAccounts.FirstOrDefault(x => x.Email == @event.Email);
        if (user == null)
        {
            await _context.UserAccounts.AddAsync(new Domain.Entities.UserAccount()
            {
                Name = @event.Name,
                Email = @event.Email,
                UserAccountGuid = @event.UserAccountGuid,
                LogoFileName = string.Empty,
                PhoneNumber = @event.PhoneNumber ?? string.Empty,
                Address = new Address(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty),
            });
            await _context.SaveChangesAsync(CancellationToken.None);
        }
    }
}
