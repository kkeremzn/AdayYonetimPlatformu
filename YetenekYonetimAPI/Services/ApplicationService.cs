using MongoDB.Driver;
using Microsoft.Extensions.Options;
using YetenekYonetimAPI.Models;
using YetenekYonetimAPI.Services;
using MongoDB.Bson;

namespace YetenekYonetimAPI.Services
{
    public class ApplicationService
    {
        private readonly IMongoCollection<Application> _applicationCollection;
        // Yeni eklenen servisler
        private readonly JobPostingService _jobPostingService;
        private readonly AdayService _adayService;

        // Constructor'ı güncelliyoruz
        public ApplicationService(IOptions<MongoDbSettings> mongoDbSettings, JobPostingService jobPostingService, AdayService adayService)
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _applicationCollection = mongoDatabase.GetCollection<Application>("applications");
            // Servisleri atıyoruz
            _jobPostingService = jobPostingService;
            _adayService = adayService;
        }

        public async Task<List<Application>> GetByJobPostingIdsAsync(List<string> jobPostingIds) =>
            await _applicationCollection.Find(basvuru => jobPostingIds.Contains(basvuru.JobPostingId)).ToListAsync();

        public async Task<List<ApplicationDetailsDto>> GetApplicationsWithAdayDetailsAsync(string jobPostingId)
        {
            if (!ObjectId.TryParse(jobPostingId, out ObjectId objectId))
            {
                return new List<ApplicationDetailsDto>();
            }

            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$match", new BsonDocument("jobPostingId", objectId)),
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "Adaylar" },
                    { "localField", "adayId" },
                    { "foreignField", "_id" },
                    { "as", "adayDetails" }
                }),
                new BsonDocument("$unwind", new BsonDocument
                {
                    { "path", "$adayDetails" },
                    { "preserveNullAndEmptyArrays", true }
                }),
                new BsonDocument("$project", new BsonDocument
                {
                    // _id'yi KAPATMA; [BsonId] ile eşleşmesi için kalsın
                    { "adayId", "$adayId" },
                    { "jobPostingId", "$jobPostingId" },
                    { "appliedAt", "$appliedAt" },
                    { "adayDetails", new BsonDocument
                    {
                        { "_id", "$adayDetails._id" },     // nested [BsonId] ile eşleşmesi için _id bırak
                        { "ad", "$adayDetails.Ad" },       // DB: Ad → DTO: ad
                        { "soyad", "$adayDetails.Soyad" }, // DB: Soyad → DTO: soyad
                        { "email", "$adayDetails.Eposta" } // DB: Eposta → DTO: email
                    }}
                })
            };

            return await _applicationCollection.Aggregate<ApplicationDetailsDto>(pipeline).ToListAsync();
        }

        public async Task<List<Application>> GetAsync() =>
            await _applicationCollection.Find(_ => true).ToListAsync();

        public async Task<Application?> GetAsync(string id) =>
            await _applicationCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Application newApplication) =>
            await _applicationCollection.InsertOneAsync(newApplication);

        public async Task UpdateAsync(string id, Application updatedApplication) =>
            await _applicationCollection.ReplaceOneAsync(x => x.Id == id, updatedApplication);

        public async Task RemoveAsync(string id) =>
            await _applicationCollection.DeleteOneAsync(x => x.Id == id);

        // Yeni sorgular için metotlar buraya eklenecek
        public async Task<List<Application>> GetByAdayIdAsync(string adayId) =>
            await _applicationCollection.Find(x => x.AdayId == adayId).ToListAsync();
        
        // Yeni metot: Şirket ID'sine göre başvurulara ait adayları getirme
        public async Task<List<Aday>> GetAdaylarByCompanyIdAsync(string companyId)
        {
            // AdayService ve JobPostingService'e erişim için servisleri constructor'a eklemelisin.
            // Şimdilik varsayalım ki erişim var.
            // Önce ilgili şirketin ilanlarını bul.
            var ilanlar = await _jobPostingService.GetByCompanyIdAsync(companyId);
            var ilanIdleri = ilanlar.Select(i => i.Id).ToList();

            // Bu ilanlara yapılan başvuruları bul.
            var basvurular = await _applicationCollection.Find(a => ilanIdleri.Contains(a.JobPostingId)).ToListAsync();
            var adayIdleri = basvurular.Select(b => b.AdayId).Distinct().ToList();

            // AdayService'ten bu adayları getir. (AdayService burada constructor'dan geçmeli)
            return await _adayService.GetByIdsAsync(adayIdleri);
        }

        // Yeni metot: Kullanıcı ID'sine (IK Uzmanı) göre başvurulara ait adayları getirme
        public async Task<List<Aday>> GetAdaylarByUserIdAsync(string userId)
        {
            // AdayService ve JobPostingService'e erişim için servisleri constructor'a eklemelisin.
            // Şimdilik varsayalım ki erişim var.
            // Önce ilgili kullanıcının (IK Uzmanı) ilanlarını bul.
            var ilanlar = await _jobPostingService.GetByUserIdAsync(userId);
            var ilanIdleri = ilanlar.Select(i => i.Id).ToList();

            // Bu ilanlara yapılan başvuruları bul.
            var basvurular = await _applicationCollection.Find(a => ilanIdleri.Contains(a.JobPostingId)).ToListAsync();
            var adayIdleri = basvurular.Select(b => b.AdayId).Distinct().ToList();

            // AdayService'ten bu adayları getir.
            return await _adayService.GetByIdsAsync(adayIdleri);
        }
            
        
    }
}