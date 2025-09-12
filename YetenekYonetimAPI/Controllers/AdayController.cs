using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YetenekYonetimAPI.Models;
using YetenekYonetimAPI.Services;
using System.Security.Claims; 

namespace YetenekYonetimAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "IKDirector, IKUzman, SystemAdmin")] 

    public class AdayController : ControllerBase
    {
        private readonly AdayService _adayService;
        private readonly JobPostingService _jobPostingService;
        private readonly ApplicationService _applicationService;

        public AdayController(AdayService adayService, JobPostingService jobPostingService, ApplicationService applicationService)
        {
            _adayService = adayService;
            _jobPostingService = jobPostingService;
            _applicationService = applicationService;
        }

        [HttpGet]
        public async Task<ActionResult<List<AdayDetailsDto>>> Get()
        {
            var kullaniciRolu = User.FindFirst(ClaimTypes.Role)?.Value;
            List<Aday> adaylar; 

            if (kullaniciRolu == "SystemAdmin")
            {
                adaylar = await _adayService.GetAsync();
            }
            else
            {
                var sirketId = User.FindFirst("SirketId")?.Value;
                var kullaniciId = User.FindFirst("userId")?.Value;

                if (string.IsNullOrEmpty(sirketId) || string.IsNullOrEmpty(kullaniciId))
                {
                    return Unauthorized("Kullanıcı veya şirket bilgisi token'da eksik.");
                }
                
                if (kullaniciRolu == "IKDirector")
                {
                    adaylar = await _applicationService.GetAdaylarByCompanyIdAsync(sirketId);
                }
                else if (kullaniciRolu == "IKUzman")
                {
                    adaylar = await _applicationService.GetAdaylarByUserIdAsync(kullaniciId);
                }
                else
                {
                    return Forbid();
                }
            }
            if (adaylar == null || adaylar.Count == 0)
            {
                return NotFound("Kayıtlı Aday Yok");
            }
            
            var adaylarDto = adaylar.Select(a => new AdayDetailsDto
            {
                id = a.Id,
                ad = a.Ad,
                soyad = a.Soyad,
                email = a.Eposta
            }).ToList();
        
            return adaylarDto;
        }


        [HttpGet("{id:length(24)}")]
        [Authorize(Roles = "IKDirector,IKUzman,SystemAdmin")]
        public async Task<ActionResult<Aday>> Get(string id)
        {
            var aday = await _adayService.GetAsync(id);
            if (aday is null)
            {
                return NotFound();
            }
            
            var kullaniciRolu = User.FindFirst(ClaimTypes.Role)?.Value;
            var sirketId = User.FindFirst("SirketId")?.Value;

            if (kullaniciRolu == "SystemAdmin")
            {
                return aday;
            }

            var basvurular = await _applicationService.GetByAdayIdAsync(id);
            if (basvurular is null || basvurular.Count == 0)
            {
                return Forbid();
            }

            var ilanIdleri = basvurular.Select(b => b.JobPostingId).ToList();
            var ilanlar = await _jobPostingService.GetByIdsAsync(ilanIdleri);
            var basvuruYapilanSirketIdleri = ilanlar.Select(i => i.CompanyId).ToList();

            if (!basvuruYapilanSirketIdleri.Contains(sirketId))
            {
                return Forbid();
            }

            if (kullaniciRolu == "IKUzman")
            {
                var kullaniciId = User.FindFirst("userId")?.Value;
                
                bool yetkiVar = false;
                foreach(var ilan in ilanlar)
                {
                    if (ilan.AssignedToUserId == kullaniciId)
                    {
                        yetkiVar = true;
                        break;
                    }
                }
                
                if (!yetkiVar)
                {
                    return Forbid();
                }
            }
            
            return aday;
        }

       
        [HttpPost]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> Post(Aday yeniAday)
        {
            await _adayService.CreateAsync(yeniAday);
            return CreatedAtAction(nameof(Get), new { id = yeniAday.Id }, yeniAday);
        }

        [HttpPut("{id:length(24)}")]
        [Authorize(Roles = "IKDirector,IKUzman,SystemAdmin")]
        public async Task<IActionResult> Update(string id, Aday guncelAday)
        {
            var aday = await _adayService.GetAsync(id);
            if (aday is null)
            {
                return NotFound();
            }
            
            var kullaniciRolu = User.FindFirst(ClaimTypes.Role)?.Value;

            if (kullaniciRolu != "SystemAdmin")
            {
                var basvurular = await _applicationService.GetByAdayIdAsync(id);
                if (basvurular is null || basvurular.Count == 0)
                {
                    return Forbid();
                }

                var ilanIdleri = basvurular.Select(b => b.JobPostingId).ToList();
                var ilanlar = await _jobPostingService.GetByIdsAsync(ilanIdleri);
                
                
                
                
            }

            guncelAday.Id = aday.Id;
            await _adayService.UpdateAsync(id, guncelAday);
            return NoContent();
        }
       
        [HttpDelete("{id:length(24)}")]
        [Authorize(Roles = "IKDirector,IKUzman,SystemAdmin")]
        public async Task<IActionResult> Delete(string id)
        {
            var aday = await _adayService.GetAsync(id);
            if (aday is null)
            {
                return NotFound();
            }
            
            var kullaniciRolu = User.FindFirst(ClaimTypes.Role)?.Value;
            var sirketId = User.FindFirst("SirketId")?.Value;

            if (kullaniciRolu != "SystemAdmin")
            {
                var basvurular = await _applicationService.GetByAdayIdAsync(id);
                if (basvurular is null || basvurular.Count == 0)
                {
                    return Forbid();
                }

                var ilanIdleri = basvurular.Select(b => b.JobPostingId).ToList();
                var ilanlar = await _jobPostingService.GetByIdsAsync(ilanIdleri);
                
                if (!ilanlar.Any(ilan => ilan.CompanyId == sirketId))
                {
                    return Forbid();
                }
                
                if (kullaniciRolu == "IKUzman" && !ilanlar.Any(ilan => ilan.AssignedToUserId == User.FindFirst("userId")?.Value))
                {
                    return Forbid();
                }
            }

            await _adayService.RemoveAsync(id);
            return NoContent();
        }
    }
}