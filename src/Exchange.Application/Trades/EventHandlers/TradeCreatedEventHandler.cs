using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Exchange.Application.Common.Interfaces;
using Exchange.Application.Common.Models;
using Exchange.Caching;
using Exchange.Domain.Events;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Exchange.Application.Trades.EventHandlers;
public class TradeCreatedEventHandler : INotificationHandler<TradeCreatedEvent>
{
    private readonly ICacheManager _cache;
    private readonly int _expiry;
    public TradeCreatedEventHandler(ICacheManager cache, IOptions<CasheSettings> options)
    {
        _cache = cache;
        _expiry = options.Value.TradeExpiryMinutes;
    }

    public async Task Handle(TradeCreatedEvent notification, CancellationToken cancellationToken)
    {
        var tradeHistory = new TradeHistoryCommand()
        {
            Count = 0,
            TimeTrade = notification.Item.Created,
            StartTimeTrade = notification.Item.Created,
            UserAccount = notification.Item.UserAccountId
        };

        var history = _cache.Get($"{CacheKeys.EntryTrade}_{notification.Item.UserAccountId}", () => { return tradeHistory; }, _expiry);
        history.Count++;
        var expiry = (DateTime.Now - history.StartTimeTrade).Minutes;
        expiry = expiry < _expiry ? expiry : _expiry;
        await _cache.SetAsync($"{CacheKeys.EntryTrade}_{notification.Item.UserAccountId}", history, expiry);

    }
}
