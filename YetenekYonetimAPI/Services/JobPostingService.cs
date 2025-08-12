using MongoDB.Driver;
using Microsoft.Extensions.Options;
using YetenekYonetimAPI.Models;

namespace YetenekYonetimAPI.Services
{
    public class JobPostingService
    {
        private readonly IMongoCollection<JobPosting> _jobPostingCollection;

        public JobPostingService(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _jobPostingCollection = mongoDatabase.GetCollection<JobPosting>("jobPostings");
        }

        public async Task<List<JobPosting>> GetAsync() =>
            await _jobPostingCollection.Find(_ => true).ToListAsync();

        public async Task<JobPosting?> GetAsync(string id) =>
            await _jobPostingCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<List<JobPosting>> GetByCompanyIdAsync(string companyId) =>
            await _jobPostingCollection.Find(x => x.CompanyId == companyId).ToListAsync();


        public async Task CreateAsync(JobPosting newJobPosting) =>
            await _jobPostingCollection.InsertOneAsync(newJobPosting);

        public async Task UpdateAsync(string id, JobPosting updatedJobPosting) =>
            await _jobPostingCollection.ReplaceOneAsync(x => x.Id == id, updatedJobPosting);

        public async Task RemoveAsync(string id) =>
            await _jobPostingCollection.DeleteOneAsync(x => x.Id == id);

        public async Task<List<JobPosting>> GetByUserIdAsync(string assignedToUserId) =>
            await _jobPostingCollection.Find(x => x.AssignedToUserId == assignedToUserId).ToListAsync();
        public async Task<List<JobPosting>> GetByIdsAsync(List<string> ids) =>
            await _jobPostingCollection.Find(ilan => ids.Contains(ilan.Id)).ToListAsync();
    }
}