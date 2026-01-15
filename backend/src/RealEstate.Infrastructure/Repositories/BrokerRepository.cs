using MongoDB.Driver;
using RealEstate.Application.Interfaces.Repositories;
using RealEstate.Application.Queries.Brokers;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums.Brokers;
using RealEstate.Domain.Enums.Common;
using RealEstate.Infrastructure.Mongo;

namespace RealEstate.Infrastructure.Repositories;

public sealed class BrokerRepository : IBrokerRepository
{
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    private readonly IMongoCollection<Broker> _collection;

    public BrokerRepository(IMongoDatabase db)
    {
        _collection = db.GetCollection<Broker>(MongoCollectionNames.Brokers);
    }

    public Task<Broker?> GetById(Guid id, CancellationToken ct) =>
        _collection.Find(x => x.Id == id).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<Broker>> GetAllAsync(CancellationToken ct) =>
        await _collection.Find(Builders<Broker>.Filter.Empty)
            .SortBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .ToListAsync(ct);

    public Task CreateAsync(Broker entity, CancellationToken ct) =>
        _collection.InsertOneAsync(entity, cancellationToken: ct);

    public async Task<bool> UpdateAsync(Broker entity, CancellationToken ct)
    {
        var res = await _collection.ReplaceOneAsync(
            x => x.Id == entity.Id,
            entity,
            cancellationToken: ct);

        // ModifiedCount can be 0 when replacing with identical document
        return res.IsAcknowledged && res.MatchedCount == 1;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var res = await _collection.DeleteOneAsync(x => x.Id == id, ct);
        return res.IsAcknowledged && res.DeletedCount == 1;
    }

    public async Task<(IReadOnlyList<Broker> Items, long TotalCount)> GetListAsync(
        BrokerListQuery query,
        CancellationToken ct)
    {
        var builder = Builders<Broker>.Filter;
        var filters = new List<FilterDefinition<Broker>>();

        // Equality filters

        if (query.BrokerId.HasValue)
            filters.Add(builder.Eq(x => x.Id, query.BrokerId.Value));

        if (query.AgencyId.HasValue)
            filters.Add(builder.Eq(x => x.AgencyId, query.AgencyId.Value));

        if (!string.IsNullOrWhiteSpace(query.FirstName))
            filters.Add(builder.Eq(x => x.FirstName, query.FirstName.Trim()));

        if (!string.IsNullOrWhiteSpace(query.LastName))
            filters.Add(builder.Eq(x => x.LastName, query.LastName.Trim()));

        if (!string.IsNullOrWhiteSpace(query.Email))
        {
            var email = NormalizeEmail(query.Email);
            filters.Add(builder.Eq(x => x.Email, email));
        }

        if (!string.IsNullOrWhiteSpace(query.PhoneNumber))
        {
            var phone = NormalizePhone(query.PhoneNumber);
            filters.Add(builder.Eq(x => x.PhoneNumber, phone));
        }

        // return an empty list if no results were found
        var filter = filters.Count == 0 ? builder.Empty : builder.And(filters);

        // Sorting (SortBy and SortDirection)

        var sortBy = query.SortBy ?? SortBy.CreatedAt;
        var direction = query.SortDirection ?? SortDirection.Desc;

        var sort = BuildSort(sortBy, direction);

        // Paging

        var page = query.Page < 1 ? 1 : query.Page;

        var pageSize = query.PageSize < 1 ? DefaultPageSize : query.PageSize;
        if (pageSize > MaxPageSize) pageSize = MaxPageSize;

        var skip = (page - 1) * pageSize;

        // Query

        var totalCount = await _collection.CountDocumentsAsync(filter, cancellationToken: ct);

        var items = await _collection.Find(filter)
            .Sort(sort)
            .Skip(skip)
            .Limit(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    private static SortDefinition<Broker> BuildSort(SortBy sortBy, SortDirection direction)
    {
        var sortBuilder = Builders<Broker>.Sort;
        
        SortDefinition<Broker> Apply<TField>(System.Linq.Expressions.Expression<Func<Broker, TField>> field) =>
            direction == SortDirection.Asc
                ? sortBuilder.Ascending(field)
                : sortBuilder.Descending(field);

        return sortBy switch
        {
            SortBy.FirstName   => Apply(x => x.FirstName),
            SortBy.LastName    => Apply(x => x.LastName),
            SortBy.AgencyId    => Apply(x => x.AgencyId),
            SortBy.Email       => Apply(x => x.Email),
            SortBy.PhoneNumber => Apply(x => x.PhoneNumber),
            SortBy.CreatedAt   => Apply(x => x.CreatedAt),
            _                  => Apply(x => x.CreatedAt)
        };
    }

    private static string NormalizeEmail(string email) =>
        email.Trim().ToLowerInvariant();

    private static string NormalizePhone(string phone)
    {
        phone = phone.Trim();

        var chars = phone
            .Where(c => char.IsDigit(c) || c == '+')
            .ToArray();

        return new string(chars);
    }
}
