using Data;
using IncomelBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace IncomelBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Employees")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }


        [HttpPost]
        [Route("Create")]
        [Authorize(Policy = "CEmployee")]
        public IActionResult CreateUser([FromBody] Employee model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var employee = _employeeService.Create(model);
            if (!employee.Item2)
            {
                return BadRequest(new { Message = employee.Item1 });
            }


            return Ok( new { Message = employee.Item1 });


        }

        [HttpPut]
        [Route("Update")]
        [Authorize(Policy = "UEmployee")]
        public IActionResult Update([FromBody] Employee model
            )
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = ModelState });
            }

            var employee = _employeeService.GetEmpleoye(x => x.Id == model.Id);
            if (!employee.Item3)
            {
                return BadRequest(new { Message = employee.Item2 });
            }

            var uuser = _employeeService.Update(employee.Item1);
            if (!uuser.Item2)
            {
                return BadRequest(new { Message = uuser.Item2 });
            }


            return Ok( new { Message =  uuser.Item1 });


        }

        [HttpDelete]
        [Route("Delete")]
        [Authorize(Policy = "DEmployee")]
        public IActionResult Delete(int id )
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = ModelState });
            }

            var user = _employeeService.Delete(id);

            if (!user.Item2)
            {
                return BadRequest(new { Message = user.Item1 });
            }

            return Ok(new { Message = user.Item1 });

        }

        [HttpGet]
        [Route("Employees")]
        [Authorize(Policy = "REmployee")]
        public IActionResult Employees()
        {

            var employees = _employeeService.GetEmpleoyes();

            return Ok(employees.Item1);

        }


        [HttpGet]
        [Route("getEmployee")]
        [Authorize(Policy = "REmployee")]
        public IActionResult getEmployee(int id)
        {

            var employees = _employeeService.GetEmpleoye(x => x.Id == id);

            if (!employees.Item3)
            {
                return BadRequest(new { Message = employees.Item2 });
            }

            return Ok(employees.Item1);

        }

    }
}
