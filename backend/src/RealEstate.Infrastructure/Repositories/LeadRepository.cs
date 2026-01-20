using MongoDB.Driver;
using RealEstate.Application.Interfaces.Repositories;
using RealEstate.Application.Queries.Leads;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.Repositories;

public sealed class LeadRepository : ILeadRepository
{
    private const string CollectionName = "leads";

    private readonly IMongoCollection<Lead> _collection;

    public LeadRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Lead>(CollectionName);
    }

    public Task CreateAsync(Lead lead, CancellationToken ct) =>
        _collection.InsertOneAsync(lead, cancellationToken: ct);

    public Task<Lead?> GetByIdAsync(Guid id, CancellationToken ct) =>
        _collection.Find(x => x.Id == id).FirstOrDefaultAsync(ct);

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _collection.DeleteOneAsync(x => x.Id == id, ct);
        return result.DeletedCount == 1;
    }

    public async Task<bool> UpdateAsync(Lead lead, CancellationToken ct)
    {
        var result = await _collection.ReplaceOneAsync(
            filter: x => x.Id == lead.Id,
            replacement: lead,
            cancellationToken: ct);

        return result.ModifiedCount == 1;
    }

    public async Task<(IReadOnlyList<Lead> Items, long TotalItems)> GetListAsync(LeadListQuery query, CancellationToken ct)
    {
        var filter = BuildFilter(query);

        var totalItems = await _collection.CountDocumentsAsync(filter, cancellationToken: ct);

        var find = _collection.Find(filter);
        find = ApplySorting(find);
        find = ApplyPaging(find, query);

        var items = await find.ToListAsync(ct);

        return (items, totalItems);
    }

    private static FilterDefinition<Lead> BuildFilter(LeadListQuery query)
    {
        var builder = Builders<Lead>.Filter;
        var filter = builder.Empty;

        if (query.Id is not null)
        {
            filter &= builder.Eq(x => x.Id, query.Id.Value);
        }

        if (query.PropertyId is not null)
        {
            filter &= builder.Eq(x => x.PropertyId, query.PropertyId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Email))
        {
            filter &= builder.Eq(x => x.Email, query.Email.Trim().ToLowerInvariant());
        }

        if (!string.IsNullOrWhiteSpace(query.PhoneNumber))
        {
            var normalized = new string(query.PhoneNumber
                .Trim()
                .Where(c => char.IsDigit(c) || c == '+')
                .ToArray());

            filter &= builder.Eq(x => x.PhoneNumber, normalized);
        }

        return filter;
    }
	 
    private static IFindFluent<Lead, Lead> ApplySorting(IFindFluent<Lead, Lead> find)
        => find.Sort(Builders<Lead>.Sort.Descending(x => x.CreatedAt));

    private static IFindFluent<Lead, Lead> ApplyPaging(IFindFluent<Lead, Lead> find, LeadListQuery query)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : query.PageSize;

        var skip = (page - 1) * pageSize;

        return find.Skip(skip).Limit(pageSize);
    }
}
