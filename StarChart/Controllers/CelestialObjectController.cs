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
                    return NotFound();
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
                var celestialObjects = _context.CelestialObjects.Where(obj => obj.Name == name).ToList();

                if (celestialObjects.Any())
                {
                    foreach (var obj in celestialObjects)
                    {
                        var satellites = _context.CelestialObjects.Where(el => el.OrbitedObjectId == obj.Id).ToList();
                        obj.Satellites = satellites;
                    }

                    return Ok(celestialObjects);
                }
                else
                {
                    return NotFound();
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
                    return NotFound();
                }

            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject obj)
        {
            try
            {
                _context.CelestialObjects.Add(obj);

                if (_context.SaveChanges() > 0)
                {
                    return CreatedAtRoute("GetById", new { id = obj.Id }, obj);
                }
                else
                {
                    return BadRequest("New celestial object cannot be saved into the database.");
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject obj)
        {
            try
            {
                var existingObj = _context.CelestialObjects.Find(id);

                if (existingObj != null)
                {
                    existingObj.Name = obj.Name;
                    existingObj.OrbitalPeriod = obj.OrbitalPeriod;
                    existingObj.OrbitedObjectId = obj.OrbitedObjectId;

                    _context.CelestialObjects.Update(existingObj);
                    _context.SaveChanges();

                    return NoContent();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            try
            {
                var existingObj = _context.CelestialObjects.Find(id);

                if (existingObj != null)
                {
                    existingObj.Name = name;
                    _context.CelestialObjects.Update(existingObj);
                    _context.SaveChanges();

                    return NoContent();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var result = _context.CelestialObjects.Where(obj => obj.Id == id || obj.OrbitedObjectId == id).ToList();

                if (result.Any())
                {
                    _context.CelestialObjects.RemoveRange(result);
                    _context.SaveChanges();

                    return NoContent();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }
    }
}
