using MongoDB.Driver;
using Microsoft.Extensions.Options;
using YetenekYonetimAPI.Models;

namespace YetenekYonetimAPI.Services
{
    public class CompanyService
    {
        private readonly IMongoCollection<Company> _companyCollection;

        public CompanyService(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _companyCollection = mongoDatabase.GetCollection<Company>("companies");
        }

        public async Task<List<Company>> GetAsync() =>
            await _companyCollection.Find(_ => true).ToListAsync();

        public async Task<Company?> GetAsync(string id) =>
            await _companyCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Company newCompany) =>
            await _companyCollection.InsertOneAsync(newCompany);

        public async Task UpdateAsync(string id, Company updatedCompany) =>
            await _companyCollection.ReplaceOneAsync(x => x.Id == id, updatedCompany);

        public async Task RemoveAsync(string id) =>
            await _companyCollection.DeleteOneAsync(x => x.Id == id);
    }
}