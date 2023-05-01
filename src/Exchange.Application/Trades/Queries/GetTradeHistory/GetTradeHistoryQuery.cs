using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Exchange.Application.Common.Interfaces;
using Exchange.Application.Common.Mappings;
using Exchange.Application.Common.Models;
using Exchange.Application.Currencies.Queries.GetCurrencySymbols;
using Exchange.Application.Interfaces;
using Exchange.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Exchange.Application.Trades.Queries.GetTradeHistory;
public record GetTradeHistoryQuery : IRequest<PaginatedList<GetTradeHistoryResult>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
public class GetTradeHistoryQueryHandler : IRequestHandler<GetTradeHistoryQuery, PaginatedList<GetTradeHistoryResult>>
{
    private readonly ICurrentUserService _currencyUserService;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetTradeHistoryQueryHandler(IApplicationDbContext context, ICurrentUserService currencyUserService, IMapper mapper)
    {
        _context = context;
        _currencyUserService = currencyUserService;
        _mapper = mapper;
    }


    public async Task<PaginatedList<GetTradeHistoryResult>> Handle(GetTradeHistoryQuery request, CancellationToken cancellationToken)
    {

        return await _context.Transactions
        .Where(x => x.UserAccount.UserAccountGuid == _currencyUserService.AccountUserId())
        .OrderBy(x => x.Created)
        .ProjectTo<GetTradeHistoryResult>(_mapper.ConfigurationProvider)
        .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
