using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YetenekYonetimAPI.Models;
using YetenekYonetimAPI.Services;

namespace YetenekYonetimAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SystemAdmin,IKDirector")]
    public class KullaniciController : ControllerBase
    {
        private readonly KullaniciService _kullaniciService;

        public KullaniciController(KullaniciService kullaniciService)
        {
            _kullaniciService = kullaniciService;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Kullanici newKullanici)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var companyId = User.FindFirst("CompanyId")?.Value;

            if (role == "IKDirector")
            {
                
                newKullanici.SirketId = companyId!;
            }

            await _kullaniciService.CreateAsync(newKullanici);
            newKullanici.Sifre = null!;

            return CreatedAtAction(nameof(GetById), new { id = newKullanici.Id }, newKullanici);
        }

        [HttpGet]
        public async Task<ActionResult<List<Kullanici>>> GetAsync()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var companyId = User.FindFirst("CompanyId")?.Value;

            var tumKullanici = await _kullaniciService.GetAsync();

            if (tumKullanici == null || tumKullanici.Count == 0)
            {
                return NotFound("Kullanıcı bulunamadı");
            }

            if (role == "IKDirector")
            {
                tumKullanici = tumKullanici.Where(k => k.SirketId == companyId).ToList();
            }

            tumKullanici.ForEach(k => k.Sifre = null!);
            return Ok(tumKullanici);
        }

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Kullanici>> GetById(string id)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var companyId = User.FindFirst("CompanyId")?.Value;

            var kullanici = await _kullaniciService.GetAsync(id);

            if (kullanici == null)
            {
                return NotFound();
            }

            if (role == "IKDirector" && kullanici.SirketId != companyId)
            {
                return Forbid("Sadece kendi şirketinizdeki kullanıcıları görüntüleyebilirsiniz.");
            }

            kullanici.Sifre = null!;
            return kullanici;
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var companyId = User.FindFirst("CompanyId")?.Value;

            var kullanici = await _kullaniciService.GetAsync(id);

            if (kullanici == null)
            {
                return NotFound("Kullanıcı bulunamadı");
            }

            if (role == "IKDirector" && kullanici.SirketId != companyId)
            {
                return Forbid("Sadece kendi şirketinizdeki kullanıcıları silebilirsiniz.");
            }

            await _kullaniciService.RemoveAsync(id);

            return NoContent();
        }
    }
}
