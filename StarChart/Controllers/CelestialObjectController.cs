using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

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

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] CelestialObject celestialObject)
        {
            var existingObject = _context.CelestialObjects.FirstOrDefault(co => co.Id == id);

            if (existingObject == null)
                return NotFound();

            existingObject.Name = celestialObject.Name;
            existingObject.OrbitalPeriod = celestialObject.OrbitalPeriod;
            existingObject.OrbitedObjectId = celestialObject.OrbitedObjectId;

            _context.CelestialObjects.Update(existingObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var existingObject = _context.CelestialObjects.FirstOrDefault(co => co.Id == id);

            if (existingObject == null)
                return NotFound();

            existingObject.Name = name;

            _context.CelestialObjects.Update(existingObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existingObjects = _context.CelestialObjects
                .Where(co => co.Id == id || co.OrbitedObjectId == id)
                .ToList();

            if (!existingObjects.Any())
                return NotFound();

            _context.CelestialObjects.RemoveRange(existingObjects);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
