using Microsoft.AspNetCore.Mvc;
using StockPriceMonitoringAndAlerts.DTOs.AlertRules;
using StockPriceMonitoringAndAlerts.Services;

namespace StockPriceMonitoringAndAlerts.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertRulesController : ControllerBase
    {

        private readonly IAlertRuleService _service;

        public AlertRulesController(IAlertRuleService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AlertRuleDto>>> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AlertRuleDto>> GetById(int id)
        {
            var rule = await _service.GetByIdAsync(id);
            if (rule == null) return NotFound();
            return Ok(rule);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<AlertRuleDto>>> GetActiveRules()
        {
            return Ok(await _service.GetActiveAlertRulesAsync());
        }

        [HttpPost]
        public async Task<ActionResult<AlertRuleDto>> Create(CreateAlertRuleDto dto)
        {
            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingRule = await _service.GetByIdAsync(id);
            if (existingRule == null) return NotFound();
            try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
