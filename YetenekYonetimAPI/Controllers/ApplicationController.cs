using Microsoft.AspNetCore.Mvc;
using YetenekYonetimAPI.Models;
using YetenekYonetimAPI.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace YetenekYonetimAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationController : ControllerBase
    {
        private readonly ApplicationService _applicationService;
        private readonly AdayService _adayService;
        private readonly JobPostingService _jobPostingService;

        public ApplicationController(ApplicationService applicationService, AdayService adayService, JobPostingService jobPostingService)
        {
            _applicationService = applicationService;
            _adayService = adayService;
            _jobPostingService = jobPostingService;
        }

        [HttpGet]
        [Authorize(Roles = "IKDirector, IKUzman, SystemAdmin")]
        public async Task<ActionResult<List<Application>>> Get()
        {
            var kullaniciRolu = User.FindFirst(ClaimTypes.Role)?.Value;

            if (kullaniciRolu == "SystemAdmin")
            {
                return await _applicationService.GetAsync();
            }

            var sirketId = User.FindFirst("SirketId")?.Value;
            var kullaniciId = User.FindFirst("userId")?.Value;

            if (string.IsNullOrEmpty(sirketId) || string.IsNullOrEmpty(kullaniciId))
            {
                return Unauthorized("Kullanıcı veya şirket bilgisi token'da eksik.");
            }
            

            if (kullaniciRolu == "IKDirector")
            {
                
                var ilanlar = await _jobPostingService.GetByCompanyIdAsync(sirketId);
                var ilanIdleri = ilanlar.Select(i => i.Id).ToList();
                return await _applicationService.GetByJobPostingIdsAsync(ilanIdleri);
                
            }
            else if (kullaniciRolu == "IKUzman")
            {
                var uzmanaAitIlanlar = await _jobPostingService.GetByUserIdAsync(kullaniciId);
                if (uzmanaAitIlanlar == null || uzmanaAitIlanlar.Count == 0)
                {
                    return new List<Application>();
                }

                var ilanIdleri = uzmanaAitIlanlar.Select(ilan => ilan.Id).ToList();
                return await _applicationService.GetByJobPostingIdsAsync(ilanIdleri);
            }
            
            return Forbid();
        }

    
        [HttpGet("{id:length(24)}")]
        [Authorize(Roles = "IKDirector,IKUzman,SystemAdmin")]
        public async Task<ActionResult<Application>> Get(string id)
        {
            var application = await _applicationService.GetAsync(id);
            if (application is null)
            {
                return NotFound("İlan Bulunamadı!");
            }

            var ilan = await _jobPostingService.GetAsync(application.JobPostingId);
            if (ilan is null)
            {
                return NotFound("Başvuruya ait ilan bulunamadı.");
            }
            
            var kullaniciRolu = User.FindFirst(ClaimTypes.Role)?.Value;
            var sirketId = User.FindFirst("SirketId")?.Value;

            if (kullaniciRolu != "SystemAdmin" && ilan.CompanyId != sirketId)
            {
                return Forbid();
            }
            
            if (kullaniciRolu == "IKUzman" && ilan.AssignedToUserId != User.FindFirst("userId")?.Value)
            {
                return Forbid();
            }
            
            return application;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Application newApplication)
        {
            var aday = await _adayService.GetAsync(newApplication.AdayId);
            if (aday is null)
            {
                return BadRequest("Geçersiz AdayId.");
            }

            var jobPosting = await _jobPostingService.GetAsync(newApplication.JobPostingId);
            if (jobPosting is null)
            {
                return BadRequest("Geçersiz JobPostingId.");
            }

            await _applicationService.CreateAsync(newApplication);

            return CreatedAtAction(nameof(Get), new { id = newApplication.Id }, newApplication);
        }

        [HttpPut("{id:length(24)}")]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> Update(string id, Application updatedApplication)
        {
            var existingApplication = await _applicationService.GetAsync(id);
            if (existingApplication is null)
            {
                return NotFound();
            }
            
            var ilan = await _jobPostingService.GetAsync(existingApplication.JobPostingId);
            if (ilan is null)
            {
                return NotFound("Başvuruya ait ilan bulunamadı.");
            }
            
            var kullaniciRolu = User.FindFirst(ClaimTypes.Role)?.Value;
            if (kullaniciRolu != "SystemAdmin")
            {
                return Forbid();
            }

            updatedApplication.Id = existingApplication.Id;
            await _applicationService.UpdateAsync(id, updatedApplication);
            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> Delete(string id)
        {
            var application = await _applicationService.GetAsync(id);
            if (application is null)
            {
                return NotFound();
            }

            var ilan = await _jobPostingService.GetAsync(application.JobPostingId);
            if (ilan is null)
            {
                return NotFound("Başvuruya ait ilan bulunamadı.");
            }
            
            var kullaniciRolu = User.FindFirst(ClaimTypes.Role)?.Value;

            if (kullaniciRolu != "SystemAdmin")
            {
                return Forbid();
            }
            
            
            await _applicationService.RemoveAsync(id);
            return NoContent();
        }
    }
}