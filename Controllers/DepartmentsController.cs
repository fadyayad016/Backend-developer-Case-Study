using EmployeeManagement.Services.DTOs;
using EmployeeManagement.Services.Services.DepartmentManagement;
using Microsoft.AspNetCore.Mvc;

namespace Backend_developer_Case_Study.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsController : Controller
    {

        private readonly IDepartmentService _departmentService;


        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DepartmentDto>), 200)] // Success response type
        public async Task<IActionResult> GetDepartments()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            return Ok(departments);
        }


        [HttpGet("{id}")] 
        [ProducesResponseType(typeof(DepartmentDto), 200)]
        [ProducesResponseType(404)] 
        public async Task<IActionResult> GetDepartmentById(int id)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            if (department == null)
            {
                return NotFound($"Department with ID {id} not found.");
            }
            return Ok(department);
        }


        [HttpPost] 
        [ProducesResponseType(typeof(DepartmentDto), 201)] // Created
        [ProducesResponseType(400)]
        [ProducesResponseType(409)] // Conflict for duplicate name
        public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentDto createDepartmentDto)
        {
            // Model validation (from Data Annotations in DTO)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdDepartment = await _departmentService.AddDepartmentAsync(createDepartmentDto);

            if (createdDepartment == null)
            {
                return Conflict($"Failed to create department. A department with name '{createDepartmentDto.Name}' might already exist.");
            }

            return CreatedAtAction(nameof(GetDepartmentById), new { id = createdDepartment.Id }, createdDepartment);
        }


    }
}
