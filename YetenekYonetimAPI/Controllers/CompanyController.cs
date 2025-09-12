using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YetenekYonetimAPI.Models;
using YetenekYonetimAPI.Services;

namespace YetenekYonetimAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize( Roles = "SystemAdmin")]
    public class CompanyController : ControllerBase
    {
        private readonly CompanyService _companyService;

        public CompanyController(CompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpGet]
        public async Task<List<Company>> Get() =>
            await _companyService.GetAsync();

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Company>> Get(string id)
        {
            var company = await _companyService.GetAsync(id);
            var kullaniciRolu = User.FindFirst(ClaimTypes.Role)?.Value;
            if (kullaniciRolu != "SystemAdmin")
            {
                return Forbid();
            }

            if (company is null)
            {
                return NotFound();
            }

            return company;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Company newCompany)
        {
            await _companyService.CreateAsync(newCompany);
            var kullaniciRolu = User.FindFirst(ClaimTypes.Role)?.Value;
            if (kullaniciRolu != "SystemAdmin")
            {
                return Forbid();
            }

            return CreatedAtAction(nameof(Get), new { id = newCompany.Id }, newCompany);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Company updatedCompany)
        {
            var company = await _companyService.GetAsync(id);
            var kullaniciRolu = User.FindFirst(ClaimTypes.Role)?.Value;
            if (kullaniciRolu != "SystemAdmin")
            {
                return Forbid();
            }

            if (company is null)
            {
                return NotFound();
            }

            updatedCompany.Id = company.Id;

            await _companyService.UpdateAsync(id, updatedCompany);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var company = await _companyService.GetAsync(id);
            var kullaniciRolu = User.FindFirst(ClaimTypes.Role)?.Value;
            if (kullaniciRolu != "SystemAdmin")
            {
                return Forbid();
            }

            if (company is null)
            {
                return NotFound();
            }

            await _companyService.RemoveAsync(id);

            return NoContent();
        }
    }
}