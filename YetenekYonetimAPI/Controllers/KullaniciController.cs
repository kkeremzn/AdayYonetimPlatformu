using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YetenekYonetimAPI.Models;
using YetenekYonetimAPI.Services;

namespace YetenekYonetimAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SystemAdmin")] // Sadece SystemAdmin bu işlemleri yapabilir
    public class KullaniciController : ControllerBase
    {
        private readonly KullaniciService _kullaniciService;

        public KullaniciController(KullaniciService kullaniciService)
        {
            _kullaniciService = kullaniciService;
        }

        // Yeni bir kullanıcı oluşturma (İK Direktörü veya İK Uzmanı)
        [HttpPost]
        public async Task<IActionResult> Post(Kullanici newKullanici)
        {
            // Parola hash'leme işlemi KullaniciService içinde otomatik olarak gerçekleşecek
            await _kullaniciService.CreateAsync(newKullanici);

            return CreatedAtAction(nameof(GetById), new { id = newKullanici.Id }, newKullanici);
        }

        [HttpGet]
        public async Task<ActionResult<Kullanici>> GetAsync()
        {
            var tumKullanici = await _kullaniciService.GetAsync();
            if (tumKullanici == null || tumKullanici.Count == 0)
            {
                return NotFound("Kullanıcı bulunamadı");
            }
            tumKullanici.ForEach(k => k.Sifre = null!);
            return Ok(tumKullanici);
        }

        // Kullanıcıyı ID'sine göre getirme metodu
        // Bu metodu şimdilik oluşturup, yetkilendirme mantığını daha sonra sıkılaştıracağız.
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Kullanici>> GetById(string id)
        {
            var kullanici = await _kullaniciService.GetAsync(id);
            if (kullanici == null)
            {
                return NotFound();
            }
            return kullanici;
        }
    }
}