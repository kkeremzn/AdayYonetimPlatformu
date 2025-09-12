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
        private readonly JobPostingService _jobPostingService;
        private readonly AdayService _adayService;

        public ApplicationService(IOptions<MongoDbSettings> mongoDbSettings, JobPostingService jobPostingService, AdayService adayService)
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _applicationCollection = mongoDatabase.GetCollection<Application>("applications");
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
                    { "adayId", "$adayId" },
                    { "jobPostingId", "$jobPostingId" },
                    { "appliedAt", "$appliedAt" },
                    { "adayDetails", new BsonDocument
                    {
                        { "_id", "$adayDetails._id" },     
                        { "ad", "$adayDetails.Ad" },       
                        { "soyad", "$adayDetails.Soyad" }, 
                        { "email", "$adayDetails.Eposta" } 
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

        public async Task<List<Application>> GetByAdayIdAsync(string adayId) =>
            await _applicationCollection.Find(x => x.AdayId == adayId).ToListAsync();
        
        public async Task<List<Aday>> GetAdaylarByCompanyIdAsync(string companyId)
        {
         
            var ilanlar = await _jobPostingService.GetByCompanyIdAsync(companyId);
            var ilanIdleri = ilanlar.Select(i => i.Id).ToList();

            var basvurular = await _applicationCollection.Find(a => ilanIdleri.Contains(a.JobPostingId)).ToListAsync();
            var adayIdleri = basvurular.Select(b => b.AdayId).Distinct().ToList();

            return await _adayService.GetByIdsAsync(adayIdleri);
        }

        public async Task<List<Aday>> GetAdaylarByUserIdAsync(string userId)
        {
          
            var ilanlar = await _jobPostingService.GetByUserIdAsync(userId);
            var ilanIdleri = ilanlar.Select(i => i.Id).ToList();

            var basvurular = await _applicationCollection.Find(a => ilanIdleri.Contains(a.JobPostingId)).ToListAsync();
            var adayIdleri = basvurular.Select(b => b.AdayId).Distinct().ToList();

            return await _adayService.GetByIdsAsync(adayIdleri);
        }
            
        
    }
}