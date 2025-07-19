using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Services.DTOs;
using EmployeeManagement.Services.Services.EmployeeManagement;
using Microsoft.AspNetCore.Mvc;

namespace Backend_developer_Case_Study.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _employeeService;
        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }



        [HttpGet] 
        [ProducesResponseType(typeof(IEnumerable<EmployeeDto>), 200)] // Success response type
        [ProducesResponseType(400)] // Bad Request for invalid input
        public async Task<IActionResult> GetEmployees(
            [FromQuery] string name,
            [FromQuery] int? departmentId,
            [FromQuery] EmployeeStatus? status,
            [FromQuery] DateTime? hireDateStart,
            [FromQuery] DateTime? hireDateEnd,
            [FromQuery] string? sortBy,
            [FromQuery] string?sortOrder,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            // Basic validation for pageination
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("Page number and page size must be positive integers.");
            }

            var employees = await _employeeService.GetAllEmployeesAsync(
                name, departmentId, status, hireDateStart, hireDateEnd,
                sortBy, sortOrder, pageNumber, pageSize
            );

            if (employees == null)
            {
                return StatusCode(500, "An error occurred while retrieving employees.");
            }

            return Ok(employees);
        }


        [HttpGet("{id}")] // HTTP GET request with ID in route (e.g., /api/Employees/1)
        [ProducesResponseType(typeof(EmployeeDto), 200)]
        [ProducesResponseType(404)] // Not Found
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound($"Employee with ID {id} not found.");
            }
            return Ok(employee);
        }


        [HttpPost] // HTTP POST request
        [ProducesResponseType(typeof(EmployeeDto), 201)] // Created
        [ProducesResponseType(400)] // Bad Request for validation errors
        [ProducesResponseType(409)] // Conflict for duplicate email/department issues
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeDto createEmployeeDto)
        {
            // Model validation (from Data Annotations in DTO)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdEmployee = await _employeeService.AddEmployeeAsync(createEmployeeDto);

            if (createdEmployee == null)
            {
                return Conflict("Failed to create employee. Department might not exist or email is duplicated.");
            }

            // Return 201 Created status with the location of the new resource
            return CreatedAtAction(nameof(GetEmployeeById), new { id = createdEmployee.Id }, createdEmployee);
        }



        [HttpPut("{id}")] // HTTP PUT request with ID in route
        [ProducesResponseType(typeof(EmployeeDto), 200)] // OK
        [ProducesResponseType(400)] // Bad Request
        [ProducesResponseType(404)] // Not Found
        [ProducesResponseType(409)] // Conflict
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] UpdateEmployeeDto updateEmployeeDto)
        {
            // Model validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedEmployee = await _employeeService.UpdateEmployeeAsync(id, updateEmployeeDto);

            if (updatedEmployee == null)
            {
                // Similar to create, service returns null for various failures
                var existing = await _employeeService.GetEmployeeByIdAsync(id);
                if (existing == null)
                {
                    return NotFound($"Employee with ID {id} not found.");
                }
                return Conflict("Failed to update employee. Department might not exist or email is duplicated.");
            }

            return Ok(updatedEmployee);
        }




        [HttpDelete("{id}")] 
        [ProducesResponseType(204)] // No Content (successful deletion)
        [ProducesResponseType(404)] // Not Found
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var isDeleted = await _employeeService.DeleteEmployeeAsync(id);
            if (!isDeleted)
            {
                // If deletion failed, it's likely because the employee wasn't found
                return NotFound($"Employee with ID {id} not found or could not be deleted.");
            }
            return NoContent(); // 204 No Content for successful deletion
        }





    }
}
