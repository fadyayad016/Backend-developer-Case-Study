using EmployeeManagement.Services.DTOs;
using EmployeeManagement.Services.Services.LogHistoryManagement;
using Microsoft.AspNetCore.Mvc;

namespace Backend_developer_Case_Study.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogHistoryController : Controller
    {

        private readonly ILogHistoryService _logHistoryService;
        public LogHistoryController(ILogHistoryService logHistoryService)
        {
            _logHistoryService = logHistoryService;
        }

        [HttpGet] 
        [ProducesResponseType(typeof(IEnumerable<LogHistoryDto>), 200)]
        public async Task<IActionResult> GetLogHistory([FromQuery] int? employeeId = null)
        {
            var logEntries = await _logHistoryService.GetAllLogHistoryAsync(employeeId);
            return Ok(logEntries);
        }

    }
}
