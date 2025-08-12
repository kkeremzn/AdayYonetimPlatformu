using YetenekYonetimAPI.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

namespace YetenekYonetimAPI.Services
{
    public class AdayService
    {
        private readonly IMongoCollection<Aday> _adayCollection;

        public AdayService(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var mongoClient = new MongoClient(
                mongoDbSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                mongoDbSettings.Value.DatabaseName);

            _adayCollection = mongoDatabase.GetCollection<Aday>(
                "Adaylar");
        }

        public async Task<List<Aday>> GetAsync() =>
            await _adayCollection.Find(_ => true).ToListAsync();

        public async Task<Aday?> GetAsync(string id) =>
            await _adayCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Aday yeniAday) =>
            await _adayCollection.InsertOneAsync(yeniAday);

        public async Task UpdateAsync(string id, Aday guncelAday) =>
            await _adayCollection.ReplaceOneAsync(x => x.Id == id, guncelAday);

        public async Task RemoveAsync(string id) =>
            await _adayCollection.DeleteOneAsync(x => x.Id == id);
        public async Task<List<Aday>> GetByIdsAsync(List<string> ids) =>
            await _adayCollection.Find(aday => ids.Contains(aday.Id)).ToListAsync();
    }
}