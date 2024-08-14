namespace Aps.Configuration.Infrastructure.Repositories;

public class AccountsRepository<T> : IRepository<T>
{
    private readonly IMongoCollection<T> _collection;
    private readonly ILogger<AccountsRepository<T>> _logger;
    private readonly IPolicyProvider _policyProvider;

    public AccountsRepository(IMongoDatabase database, string collectionName,
                              ILogger<AccountsRepository<T>> logger, IPolicyProvider policyProvider)
    {
        _collection = database.GetCollection<T>(collectionName);
        _logger = logger;
        _policyProvider = policyProvider;
    }

    public async Task<T> GetByIdAsync(string id)
    {
        var retryPolicy = _policyProvider.GetRetryPolicy();

        return await retryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve document with ID: {Id}", id);

                var filter = Builders<T>.Filter.Eq("_id", id);
                var result = await _collection.Find(filter).FirstOrDefaultAsync();

                if (result is null)
                    _logger.LogWarning("No document found with ID: {Id}", id);

                return result;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Failed to get document by ID: {Id}", id);
                throw new DataAccessException("Failed to get document by ID.", ex);
            }
        });
    }

    public async Task<IEnumerable<T>> GetAllAsync(int limit = 1000)
    {
        var retryPolicy = _policyProvider.GetRetryPolicy();

        return await retryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve all documents with limit: {Limit}", limit);

                var result = await _collection.Find(Builders<T>.Filter.Empty)
                                              .Limit(limit)
                                              .ToListAsync();
                _logger.LogInformation("Retrieved {Count} documents", result.Count);
                return result;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Failed to retrieve documents with limit: {Limit}", limit);
                throw new DataAccessException("Failed to retrieve documents.", ex);
            }
        });
    }

    public async Task<T> AddAsync(T entity)
    {
        var retryPolicy = _policyProvider.GetRetryPolicy();

        return await retryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                _logger.LogInformation("Attempting to insert a new document.");

                await _collection.InsertOneAsync(entity);
                _logger.LogInformation("Document inserted successfully.");
                return entity;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Failed to insert document.");
                throw new DataAccessException("Failed to insert document.", ex);
            }
        });
    }

    public async Task<T> UpdateAsync(string id, T entity)
    {
        var retryPolicy = _policyProvider.GetRetryPolicy();

        return await retryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                _logger.LogInformation("Attempting to update document with ID: {Id}", id);

                var filter = Builders<T>.Filter.Eq("_id", id);
                var result = await _collection.ReplaceOneAsync(filter, entity);
                if (result.MatchedCount == 0)
                {
                    _logger.LogWarning("No document matched the ID: {Id} for update.", id);
                }
                else
                {
                    _logger.LogInformation("Document with ID: {Id} updated successfully.", id);
                }
                return result.IsAcknowledged ? entity : default;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Failed to update document with ID: {Id}", id);
                throw new DataAccessException("Failed to update document.", ex);
            }
        });
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var retryPolicy = _policyProvider.GetRetryPolicy();

        return await retryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                _logger.LogInformation("Attempting to delete document with ID: {Id}", id);

                var filter = Builders<T>.Filter.Eq("_id", id);
                var result = await _collection.DeleteOneAsync(filter);
                bool success = result.DeletedCount > 0;
                if (success)
                {
                    _logger.LogInformation("Document with ID: {Id} deleted successfully.", id);
                }
                else
                {
                    _logger.LogWarning("No document found with ID: {Id} to delete.", id);
                }
                return success;
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Failed to delete document with ID: {Id}", id);
                throw new DataAccessException("Failed to delete document.", ex);
            }
        });
    }

    public async Task<(IEnumerable<T> Documents, int TotalCount)> FindAsync(FilterDefinition<T> filter,
                                                                            int pageNumber = 1, int pageSize = 10)
    {
        var retryPolicy = _policyProvider.GetRetryPolicy();

        return await retryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                _logger.LogInformation("Attempting to find documents with filter: {@Filter}, " +
                                        "pageNumber: {PageNumber}, pageSize: {PageSize}", filter, pageNumber, pageSize);

                var totalCount = await _collection.CountDocumentsAsync(filter);

                var documents = await _collection.Find(filter)
                                                 .Skip((pageNumber - 1) * pageSize)
                                                 .Limit(pageSize)
                                                 .ToListAsync();

                _logger.LogInformation("Found {Count} documents with the provided filter. Total count: {TotalCount}", documents.Count, totalCount);
                return (Documents: documents, TotalCount: (int)totalCount);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Failed to find documents with filter: {@Filter}", filter);
                throw new DataAccessException("Failed to find documents with the specified filter.", ex);
            }
        });
    }
}