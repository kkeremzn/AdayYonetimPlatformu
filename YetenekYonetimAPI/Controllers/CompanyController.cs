using Microsoft.AspNetCore.Mvc;
using YetenekYonetimAPI.Models;
using YetenekYonetimAPI.Services;

namespace YetenekYonetimAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly CompanyService _companyService;

        public CompanyController(CompanyService companyService)
        {
            _companyService = companyService;
        }

        // GET: api/Company
        [HttpGet]
        public async Task<List<Company>> Get() =>
            await _companyService.GetAsync();

        // GET: api/Company/{id}
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Company>> Get(string id)
        {
            var company = await _companyService.GetAsync(id);

            if (company is null)
            {
                return NotFound();
            }

            return company;
        }

        // POST: api/Company
        [HttpPost]
        public async Task<IActionResult> Post(Company newCompany)
        {
            await _companyService.CreateAsync(newCompany);

            return CreatedAtAction(nameof(Get), new { id = newCompany.Id }, newCompany);
        }

        // PUT: api/Company/{id}
        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Company updatedCompany)
        {
            var company = await _companyService.GetAsync(id);

            if (company is null)
            {
                return NotFound();
            }

            updatedCompany.Id = company.Id;

            await _companyService.UpdateAsync(id, updatedCompany);

            return NoContent();
        }

        // DELETE: api/Company/{id}
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var company = await _companyService.GetAsync(id);

            if (company is null)
            {
                return NotFound();
            }

            await _companyService.RemoveAsync(id);

            return NoContent();
        }
    }
}