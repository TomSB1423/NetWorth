using Microsoft.EntityFrameworkCore;
using Networth.Backend.Domain.Entities;
using Networth.Backend.Domain.Enums;
using Networth.Backend.Domain.Repositories;
using Networth.Backend.Infrastructure.Data.Context;

namespace Networth.Backend.Infrastructure.Data.Repositories;

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
