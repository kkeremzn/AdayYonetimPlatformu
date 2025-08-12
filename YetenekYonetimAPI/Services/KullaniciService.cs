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
            // Kullanıcının parolasını hash'le ve Sifre alanına ata
            newKullanici.Sifre = BCrypt.Net.BCrypt.HashPassword(newKullanici.Sifre);
            await _kullaniciCollection.InsertOneAsync(newKullanici);
        }
        public async Task<Kullanici?> GetAsync(string id) =>
        await _kullaniciCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<List<Kullanici>> GetAsync() =>
        await _kullaniciCollection.Find(_ => true).ToListAsync();

        // Diğer CRUD metotları (Create, Update, Delete) ileride eklenebilir.
    }
}