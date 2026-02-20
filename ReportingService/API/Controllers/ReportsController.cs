using Microsoft.AspNetCore.Mvc;
using ReportingService.Application.DTOs;
using ReportingService.Application.Services;
using ReportingService.Application.Interfaces;
using ReportingService.Domain.Entities; // <--- CETTE LIGNE MANQUAIT

namespace ReportingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly ReportGenerationService _generationService;
        private readonly IReportRepository _repository;

        public ReportsController(
            ReportGenerationService generationService,
            IReportRepository repository)
        {
            _generationService = generationService;
            _repository = repository;
        }

        [HttpPost("generate")]
public async Task<IActionResult> Generate([FromBody] GenerateReportRequest request)
{
    // Correction ici : On remplace _reportService par _generationService
    var result = await _generationService.GenerateAsync(request); 
    return Ok(result);
}


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var reports = await _repository.GetAllAsync();
            return Ok(reports);
        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> Download(Guid id)
        {
            var report = await _repository.GetByIdAsync(id);

            if (report == null)
                return NotFound();

            if (!System.IO.File.Exists(report.FilePath))
                return NotFound("Le fichier physique n'existe pas sur le serveur.");

            var fileBytes = await System.IO.File.ReadAllBytesAsync(report.FilePath);
            return File(fileBytes, "application/pdf", Path.GetFileName(report.FilePath));
        }

        [HttpGet("filter")]
        public async Task<IActionResult> Filter(DateTime start, DateTime end)
        {
            // On utilise le repository ici
            var reports = await _repository.GetByDateAsync(start, end);
            return Ok(reports);
        }
    }
}