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

        // --- GET: api/Aday ---
        [HttpGet]
        public async Task<ActionResult<List<AdayDetailsDto>>> Get()
        {
            var kullaniciRolu = User.FindFirst(ClaimTypes.Role)?.Value;
            List<Aday> adaylar; // Adaylar listesini burada tanımlıyoruz

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
            
            // Adayları DTO'ya dönüştürme işlemini burada, sadece bir kez yapıyoruz.
            var adaylarDto = adaylar.Select(a => new AdayDetailsDto
            {
                id = a.Id,
                ad = a.Ad,
                soyad = a.Soyad,
                email = a.Eposta
            }).ToList();

            return adaylarDto;
        }


        // --- GET: api/Aday/{id} ---
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

            // Adayın, kullanıcının şirketine başvurduğunu kontrol et.
            var basvurular = await _applicationService.GetByAdayIdAsync(id);
            if (basvurular is null || basvurular.Count == 0)
            {
                return Forbid();
            }

            // Adayın başvurduğu ilanların şirket ID'lerini topla.
            var ilanIdleri = basvurular.Select(b => b.JobPostingId).ToList();
            var ilanlar = await _jobPostingService.GetByIdsAsync(ilanIdleri);
            var basvuruYapilanSirketIdleri = ilanlar.Select(i => i.CompanyId).ToList();

            // Aday, kullanıcının şirketine başvuru yapmamışsa erişimi engelle.
            if (!basvuruYapilanSirketIdleri.Contains(sirketId))
            {
                return Forbid();
            }

            // IK Uzmanı için ek kontrol: Sadece kendisine atanan ilana başvuran adayı görebilir.
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

        // --- POST: api/Aday ---
        // Aday oluşturma. Adaylar genellikle ön yüzdeki bir form üzerinden gelir,
        // bu yüzden herkesin erişimine açık olabilir veya özel bir endpoint olabilir.
        // Şimdilik Direktör yetkisini atayalım.
        [HttpPost]
        [Authorize(Roles = "IKDirector, SystemAdmin")]
        public async Task<IActionResult> Post(Aday yeniAday)
        {
            await _adayService.CreateAsync(yeniAday);
            return CreatedAtAction(nameof(Get), new { id = yeniAday.Id }, yeniAday);
        }

        // --- PUT: api/Aday/{id} ---
        // Aday düzenleme. Direktör veya o adayın başvurduğu ilanın sahibi olan uzman yetkili.
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

            guncelAday.Id = aday.Id;
            await _adayService.UpdateAsync(id, guncelAday);
            return NoContent();
        }
        // --- DELETE: api/Aday/{id} ---
        // Aday silme. Direktör veya o adayın başvurduğu ilanın sahibi olan uzman yetkili.
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