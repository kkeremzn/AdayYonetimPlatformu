using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using YetenekYonetimAPI.Models;
using YetenekYonetimAPI.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace YetenekYonetimAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "IkDirector, IkUzman, SystemAdmin")]
    public class JobPostingController : ControllerBase
    {
        private readonly JobPostingService _jobPostingService;
        private readonly CompanyService _companyService;
        private readonly ApplicationService _applicationService;

        public JobPostingController(JobPostingService jobPostingService, CompanyService companyService, ApplicationService applicationService)
        {
            _jobPostingService = jobPostingService;
            _companyService = companyService;
            _applicationService = applicationService;
        }

        // --- OKUMA (GET) METOTLARI ---

        // Tüm ilanları getiren metot. Rol bazlı erişim kontrolü içerir.
        [HttpGet]
        public async Task<ActionResult<List<JobPosting>>> Get()
        {
            var kullaniciRolu = User.FindFirst(ClaimTypes.Role)?.Value;

            // SystemAdmin tüm ilanları görebilir.
            if (kullaniciRolu == "SystemAdmin")
            {
                return await _jobPostingService.GetAsync();
            }

            var sirketId = User.FindFirst("SirketId")?.Value;
            var kullaniciId = User.FindFirst("userId")?.Value;

            // Diğer roller için şirket ve kullanıcı bilgisi kontrolü zorunludur.
            if (string.IsNullOrEmpty(sirketId) || string.IsNullOrEmpty(kullaniciId))
            {
                return Unauthorized("Kullanıcı veya şirket bilgisi geçersiz veya bulunamadı!");
            }

            if (kullaniciRolu == "IKDirector")
            {
                // IK Direktörü sadece kendi şirketinin ilanlarını görebilir.
                return await _jobPostingService.GetByCompanyIdAsync(sirketId);
            }
            else if (kullaniciRolu == "IKUzman")
            {
                // IK Uzmanı sadece kendisine atanan ilanları görebilir.
                return await _jobPostingService.GetByUserIdAsync(kullaniciId);
            }

            return Forbid();
        }

       
        // Bir ilana ait başvuruları detaylarıyla getirme.
        [HttpGet("{id:length(24)}/applications-with-details")]
        [Authorize(Roles = "IKDirector,IKUzman, SystemAdmin")]
        public async Task<ActionResult<List<ApplicationDetailsDto>>> GetApplicationsWithDetailsByJobPostingId(string id)
        {
            var jobPosting = await _jobPostingService.GetAsync(id);
            if (jobPosting is null)
            {
                return NotFound("İlan bulunamadı!");
            }

            var sirketId = User.FindFirst("SirketId")?.Value;
            var kullaniciRolu = User.FindFirst(ClaimTypes.Role)?.Value;

            // Çoklu kiracılık kontrolü: SystemAdmin hariç, ilanın şirket ID'si token'daki ile eşleşmeli.
            if (kullaniciRolu != "SystemAdmin" && jobPosting.CompanyId != sirketId)
            {
                return Forbid();
            }

            // IK Uzmanı için ek kontrol: Sadece kendisine atanan ilanın başvurularını görebilir.
            if (kullaniciRolu == "IKUzman" && User.FindFirst("userId")?.Value != jobPosting.AssignedToUserId)
            {
                return Forbid();
            }

            var applications = await _applicationService.GetApplicationsWithAdayDetailsAsync(id);
            return applications;
        }

        // --- YAZMA (CREATE, UPDATE, DELETE) METOTLARI ---

        // İlan Oluşturma: Sadece İK Direktörü yetkili
        [HttpPost]
        [Authorize(Roles = "IKDirector, SystemAdmin")]
        public async Task<IActionResult> Post(JobPosting newJobPosting)
        {
            var sirketId = User.FindFirst("SirketId")?.Value;
            if (String.IsNullOrEmpty(sirketId))
            {
                return Unauthorized("Şirket Bilgisi Tokenda Bulunamadı!");
            }

            // İlanı oluşturan Direktörün kendi ID'sini atama
            var userId = User.FindFirst("userId")?.Value;
            newJobPosting.AssignedToUserId = userId;

            await _jobPostingService.CreateAsync(newJobPosting);
            return CreatedAtAction(nameof(Get), new { id = newJobPosting.Id }, newJobPosting);
        }

        // İlan Güncelleme: Direktör, uzman veya SystemAdmin yetkili
        [HttpPut("{id:length(24)}")]
        [Authorize(Roles = "IKDirector,IKUzman, SystemAdmin")]
        public async Task<IActionResult> Update(string id, JobPosting updatedJobPosting)
        {
            var existingJobPosting = await _jobPostingService.GetAsync(id);
            if (existingJobPosting is null)
            {
                return NotFound("İlan Bulunamadı!");
            }

            var sirketId = User.FindFirst("SirketId")?.Value;
            var kullaniciRolu = User.FindFirst(ClaimTypes.Role)?.Value;

            // Çoklu kiracılık kontrolü: SystemAdmin hariç, ilanın şirket ID'si, token'daki ile eşleşmeli.
            if (kullaniciRolu != "SystemAdmin" && existingJobPosting.CompanyId != sirketId)
            {
                return Forbid();
            }

            // IK Uzmanı için ek kontrol: Sadece kendisine atanan ilanı güncelleyebilir.
            if (kullaniciRolu == "IKUzman" && User.FindFirst("userId")?.Value != existingJobPosting.AssignedToUserId)
            {
                return Forbid();
            }

            updatedJobPosting.Id = existingJobPosting.Id;
            await _jobPostingService.UpdateAsync(id, updatedJobPosting);
            return NoContent();
        }

        // İlan Silme: Direktör, uzman veya SystemAdmin yetkili
        [HttpDelete("{id:length(24)}")]
        [Authorize(Roles = "IKDirector,IKUzman, SystemAdmin")]
        public async Task<IActionResult> Delete(string id)
        {
            var jobPosting = await _jobPostingService.GetAsync(id);
            if (jobPosting is null)
            {
                return NotFound("İlan Bulunamadı!");
            }

            var sirketId = User.FindFirst("SirketId")?.Value;
            var kullaniciRolu = User.FindFirst(ClaimTypes.Role)?.Value;

            // Çoklu kiracılık kontrolü: SystemAdmin hariç, ilanın şirket ID'si, token'daki ile eşleşmeli.
            if (kullaniciRolu != "SystemAdmin" && jobPosting.CompanyId != sirketId)
            {
                return Forbid();
            }

            // IK Uzmanı için ek kontrol: Sadece kendisine atanan ilanı silebilir.
            if (kullaniciRolu == "IKUzman" && User.FindFirst("userId")?.Value != jobPosting.AssignedToUserId)
            {
                return Forbid();
            }

            await _jobPostingService.RemoveAsync(id);
            return Content("İlan Silindi");
        }
    }
}