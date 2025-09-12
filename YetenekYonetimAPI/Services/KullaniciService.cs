using YetenekYonetimAPI.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using BCrypt.Net;


namespace YetenekYonetimAPI.Services
{
    public class KullaniciService
    {
        private readonly IMongoCollection<Kullanici> _kullaniciCollection;

        public KullaniciService(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _kullaniciCollection = mongoDatabase.GetCollection<Kullanici>("Kullanicilar");
        }

        public async Task<Kullanici?> GetByKullaniciAdiAsync(string kullaniciAdi) =>
            await _kullaniciCollection.Find(x => x.KullaniciAdi == kullaniciAdi).FirstOrDefaultAsync();

        public async Task CreateAsync(Kullanici newKullanici)
        {
            newKullanici.Sifre = BCrypt.Net.BCrypt.HashPassword(newKullanici.Sifre);
            await _kullaniciCollection.InsertOneAsync(newKullanici);
        }
        public async Task<Kullanici?> GetAsync(string id) =>
        await _kullaniciCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<List<Kullanici>> GetAsync() =>
        await _kullaniciCollection.Find(_ => true).ToListAsync();
        public async Task RemoveAsync(string id) =>
            await _kullaniciCollection.DeleteOneAsync(x => x.Id == id);

        
    }
}