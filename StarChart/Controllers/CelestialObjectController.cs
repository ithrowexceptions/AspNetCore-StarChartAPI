using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.FirstOrDefault(co => co.Id == id);

            if (celestialObject == null)
                return NotFound();

            celestialObject.Satellites = _context.CelestialObjects
               .Where(co => co.OrbitedObjectId == id)
               .ToList();

            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects
                .Where(co => co.Name == name)
                .ToList();

            if (!celestialObjects.Any())
                return NotFound();

            celestialObjects.ForEach(x =>
                x.Satellites = _context.CelestialObjects
                    .Where(y => y.OrbitedObjectId == x.Id)
                    .ToList());

            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects
                .ToList();

            celestialObjects.ForEach(x =>
                x.Satellites = _context.CelestialObjects
                    .Where(y => y.OrbitedObjectId == x.Id)
                    .ToList());

            return Ok(celestialObjects);
        }
    }
}
