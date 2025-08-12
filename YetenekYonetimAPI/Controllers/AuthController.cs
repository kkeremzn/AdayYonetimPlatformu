using Microsoft.AspNetCore.Mvc;
using YetenekYonetimAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using YetenekYonetimAPI.Services;
using BCrypt.Net;

namespace YetenekYonetimAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly KullaniciService _kullaniciService;

        public AuthController(JwtSettings jwtSettings, KullaniciService kullaniciService)
        {
            _jwtSettings = jwtSettings;
            _kullaniciService = kullaniciService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // 1. Veritabanından kullanıcıyı kullanıcı adına göre bul
            var kullanici = await _kullaniciService.GetByKullaniciAdiAsync(loginDto.Username);

            // 2. Kullanıcı bulunamadıysa veya şifre eşleşmiyorsa hata dön
            if (kullanici == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, kullanici.Sifre))
            {
                return Unauthorized("Geçersiz kullanıcı adı veya şifre.");
            }

            // 3. Kullanıcı geçerliyse JWT Token oluşturma
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, kullanici.KullaniciAdi),
                new Claim(ClaimTypes.Role, kullanici.Rol), // <-- Kullanıcının rolünü token'a ekle
                new Claim("SirketId", kullanici.SirketId), // <-- Şirket ID'sini token'a ekle
                new Claim("userId", kullanici.Id)
            };

            var token = new JwtSecurityToken(
                _jwtSettings.Issuer,
                _jwtSettings.Audience,
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            // 4. Token'ı döndür
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiresAt = token.ValidTo,
                role = kullanici.Rol,
                companyId = kullanici.SirketId,
                userId = kullanici.Id
            });

        }

    }
}