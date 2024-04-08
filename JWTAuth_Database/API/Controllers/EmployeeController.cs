using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly JwtauthenticationContext dbContext;

        public EmployeeController(JwtauthenticationContext DbContext)
        {
            dbContext = DbContext;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllEmployee()
        {
            var employee = await dbContext.Employees.ToListAsync();
            return Ok(employee);
        }
    }
}
