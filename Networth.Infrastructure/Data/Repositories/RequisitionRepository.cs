using Microsoft.EntityFrameworkCore;
using Networth.Domain.Entities;
using Networth.Domain.Enums;
using Networth.Domain.Repositories;
using Networth.Infrastructure.Data.Context;

namespace Networth.Infrastructure.Data.Repositories;

/// <summary>
///     Repository implementation for Requisition entities.
/// </summary>
public class RequisitionRepository : BaseRepository<Requisition, string>, IRequisitionRepository
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="RequisitionRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public RequisitionRepository(NetworthDbContext context)
        : base(context)
    {
    }
}
