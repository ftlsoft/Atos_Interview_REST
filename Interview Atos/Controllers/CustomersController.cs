using Interview_Atos.Models;
using Interview_Atos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Interview_Atos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous] //ToDo: obvious it should require authorization
    public class CustomersController : ControllerBase
    {
        private readonly ILogger<CustomersController> _logger;
        private readonly ICustomerService _customerService;

        public CustomersController(ILogger<CustomersController> logger, ICustomerService customerService)
        {
            _logger = logger;
            _customerService = customerService;
        }

        [HttpGet("{customerId}")]
        [ProducesResponseType(typeof(Customer), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ActionName(nameof(GetByIdAsync))]

        public async Task<ActionResult<Customer>> GetByIdAsync(int customerId)
        {
            if (customerId < 0)
                return BadRequest();
            var customer = await _customerService.FindAsync(customerId);
            return customer == null ? NotFound() : Ok(customer);
        }

        [HttpGet]
        [ProducesResponseType(typeof(Customer), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Customer>> GetAllAsync()
        {
            _logger.LogDebug($"{nameof(CustomersController)} {nameof(GetAllAsync)}");
            return Ok(_customerService.GetAllAsync());
        }

        [HttpPost("create")]
        [ProducesResponseType(typeof(Customer), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Customer>> CreateAsync([FromBody] Customer newCustomer)
        {
            _logger.LogDebug($"{nameof(CustomersController)} {nameof(CreateAsync)}");
            var result = await _customerService.CreateAsync(newCustomer);

            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetByIdAsync), new { customerId = result.Value.Id }, newCustomer);

            if (result.Error.Any())
                return BadRequest(result.Error);

            _logger.LogError($"{nameof(CustomersController)} {nameof(CreateAsync)} returned object == null and validationErrors.Any() == false, this should never occurred");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpDelete("{customerId}")]
        public async Task<ActionResult> DeleteAsync(int customerId)
        {
            _logger.LogDebug($"{nameof(CustomersController)} {nameof(DeleteAsync)} customerId: {customerId}");
            var res = await _customerService.DeleteAsync(customerId);
            return Ok(res);
        }
    }
}
