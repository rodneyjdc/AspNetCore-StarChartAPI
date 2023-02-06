using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
            this._context = context;
        }

        [HttpGet("{id:int}")]
        [ActionName("GetById")]
        public IActionResult GetById(int id)
        {
            try
            {

                var celestialObject = _context.CelestialObjects.FindAsync(id).Result;

                if (celestialObject != null)
                {
                    var satellites = _context.CelestialObjects.Where(obj => obj.OrbitedObjectId == id).ToList();
                    celestialObject.Satellites = satellites;

                    return Ok(celestialObject);
                }
                else
                {
                    return NotFound("There is no celestial object with the given ID.");
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            try
            {
                var celestialObject = _context.CelestialObjects.Where(obj => obj.Name == name)
                    .FirstOrDefault();

                if (celestialObject != default(CelestialObject))
                {
                    var satellites = _context.CelestialObjects.Where(obj => 
                        obj.OrbitedObjectId == celestialObject.Id).ToList();
                    celestialObject.Satellites = satellites;

                    return Ok(celestialObject);
                }
                else
                {
                    return NotFound("There is no celestial object with the given Name.");
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var celestialObjects = _context.CelestialObjects.ToList();

                if (celestialObjects.Any())
                {
                    foreach (var obj in celestialObjects)
                    {
                        var satellites = celestialObjects.Where(el => el.OrbitedObjectId == obj.Id).ToList();
                        obj.Satellites = satellites;
                    }

                    return Ok(celestialObjects);
                }
                else
                {
                    return NotFound("No celestial objects were found.");
                }
                
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }
    }
}
