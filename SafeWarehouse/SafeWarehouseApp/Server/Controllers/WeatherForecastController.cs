using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SafeWarehouseApp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SafeWarehouseApp.Shared.Models;

namespace SafeWarehouseApp.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Report> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new Report()
            {
            })
            .ToArray();
        }
    }
}
