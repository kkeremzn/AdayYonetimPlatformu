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

      
        [HttpGet]
        public async Task<ActionResult<List<JobPosting>>> Get()
        {
            var kullaniciRolu = User.FindFirst(ClaimTypes.Role)?.Value;

            if (kullaniciRolu == "SystemAdmin")
            {
                return await _jobPostingService.GetAsync();
            }

            var sirketId = User.FindFirst("SirketId")?.Value;
            var kullaniciId = User.FindFirst("userId")?.Value;

            if (string.IsNullOrEmpty(sirketId) || string.IsNullOrEmpty(kullaniciId))
            {
                return Unauthorized("Kullanıcı veya şirket bilgisi geçersiz veya bulunamadı!");
            }

            if (kullaniciRolu == "IKDirector")
            {
                return await _jobPostingService.GetByCompanyIdAsync(sirketId);
            }
            else if (kullaniciRolu == "IKUzman")
            {
                return await _jobPostingService.GetByUserIdAsync(kullaniciId);
            }

            return Forbid();
        }

       
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

            if (kullaniciRolu != "SystemAdmin" && jobPosting.CompanyId != sirketId)
            {
                return Forbid();
            }

            if (kullaniciRolu == "IKUzman" && User.FindFirst("userId")?.Value != jobPosting.AssignedToUserId)
            {
                return Forbid();
            }

            var applications = await _applicationService.GetApplicationsWithAdayDetailsAsync(id);
            return applications;
        }


        [HttpPost]
        [Authorize(Roles = "IKDirector, SystemAdmin")]
        public async Task<IActionResult> Post(JobPosting newJobPosting)
        {
            var sirketId = User.FindFirst("SirketId")?.Value;
            if (String.IsNullOrEmpty(sirketId))
            {
                return Unauthorized("Şirket Bilgisi Tokenda Bulunamadı!");
            }

            var userId = User.FindFirst("userId")?.Value;
            newJobPosting.AssignedToUserId = userId;

            await _jobPostingService.CreateAsync(newJobPosting);
            return CreatedAtAction(nameof(Get), new { id = newJobPosting.Id }, newJobPosting);
        }

        [HttpPut("{id:length(24)}")]
        [Authorize(Roles = "IKDirector, SystemAdmin")]
        public async Task<IActionResult> Update(string id, JobPosting updatedJobPosting)
        {
            var existingJobPosting = await _jobPostingService.GetAsync(id);
            if (existingJobPosting is null)
            {
                return NotFound("İlan Bulunamadı!");
            }

            var sirketId = User.FindFirst("SirketId")?.Value;
            var kullaniciRolu = User.FindFirst(ClaimTypes.Role)?.Value;

            if (kullaniciRolu != "SystemAdmin" && existingJobPosting.CompanyId != sirketId)
            {
                return Forbid();
            }

           

            updatedJobPosting.Id = existingJobPosting.Id;
            await _jobPostingService.UpdateAsync(id, updatedJobPosting);
            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        [Authorize(Roles = "IKDirector, SystemAdmin")]
        public async Task<IActionResult> Delete(string id)
        {
            var jobPosting = await _jobPostingService.GetAsync(id);
            if (jobPosting is null)
            {
                return NotFound("İlan Bulunamadı!");
            }

            var sirketId = User.FindFirst("SirketId")?.Value;
            var kullaniciRolu = User.FindFirst(ClaimTypes.Role)?.Value;

            if (kullaniciRolu != "SystemAdmin" && jobPosting.CompanyId != sirketId)
            {
                return Forbid();
            }

            

            await _jobPostingService.RemoveAsync(id);
            return Content("İlan Silindi");
        }
    }
}