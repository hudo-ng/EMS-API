using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using EMS.Api.Models;
using EMS.Api.Data;

namespace EMS.Api.Controllers
{
    [ApiController]
    [Route("api/employees")]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public EmployeesController(AppDbContext db)
        {
            _db = db;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IEnumerable<Employee>> GetAll()
        {
            return await _db.Employees.ToListAsync();
        }

        [Authorize(Roles = "Admin,Manager,Staff")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var employee = await _db.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound("Employee not found");
            }
            if (User.IsInRole("Staff"))
            {
                if (employee.UserId != int.Parse(User.Claims.First(c => c.Type == "UserId").Value))
                {
                    return Forbid("You can only access your own employee record");
                }
            }
            return Ok(employee);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, Employee updatedEmployee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var employee = await _db.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound("Employee not found");
            }
            employee.FirstName = updatedEmployee.FirstName;
            employee.LastName = updatedEmployee.LastName;
            employee.Position = updatedEmployee.Position;
            employee.Wage = updatedEmployee.Wage;
            employee.Email = updatedEmployee.Email;
            await _db.SaveChangesAsync();
            return Ok(updatedEmployee);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateEmployee(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingEmployee = await _db.Employees.FirstOrDefaultAsync(e => e.Email == employee.Email);
            if (existingEmployee != null)
            {
                return BadRequest("Employee with this email already exists");
            }
            _db.Employees.Add(employee);
            await _db.SaveChangesAsync();
            return Ok(employee);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _db.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound("Employee not found");
            }
            _db.Employees.Remove(employee);
            await _db.SaveChangesAsync();
            return Ok("Employee deleted successfully");
        }
    }
}