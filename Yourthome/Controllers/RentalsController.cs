﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yourthome.Data;
using Yourthome.Models;
using Yourthome.ViewModel;
using System.IO;
using Microsoft.VisualBasic;

namespace Yourthome.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly YourthomeContext _context;

        public RentalsController(YourthomeContext context)
        {
            _context = context;      
        }
        /// <summary>
        /// Find all rentals
        /// </summary>
        // GET: Rentals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rental>>> GetRental([FromQuery]Region? region, [FromQuery]int? rooms,
            [FromQuery]PropertyType? property, [FromQuery]RentTime? renttime,
            [FromQuery]int? CostrangeStart, [FromQuery] int? CostrangeEnd, [FromQuery]FacFilter facfilter,
            [FromQuery]InfraFilter infrafilter, [FromQuery]Sort? sort)
        {
            var rents = _context.Rental.Include(r => r.Facilities).Include(r => r.Infrastructure).Include(r => r.Photos).AsQueryable();
            if (region.HasValue)
            {
                rents = rents.Where(r => r.Region == region); //filter by Region
            }
            if (rooms.HasValue)
            {
                rents = rents.Where(r => r.Rooms == rooms); //filter by amount of room
            }
            if (property.HasValue)
            {
                rents = rents.Where(r => r.PropertyType == property); //filter by property type (house or apartment)
            }
            if (renttime.HasValue)
            {
                rents = rents.Where(r => r.RentTime == renttime); //filter by rent time 
            }
            if(CostrangeStart.HasValue && CostrangeEnd.HasValue)
            {
                rents = rents.Where(r => r.Cost>=CostrangeStart && r.Cost<=CostrangeEnd); // filter in cost range
            }
            if (facfilter.Internet != null && facfilter.AirConditioning != null)
            {
                rents = rents.Where(r => r.Facilities.AirConditioning == facfilter.AirConditioning && r.Facilities.Internet == facfilter.Internet
                && r.Facilities.Phone == facfilter.Phone && r.Facilities.Washer == facfilter.Washer
                && r.Facilities.Refrigerator == facfilter.Refrigerator && r.Facilities.Balcony == facfilter.Balcony
                && r.Facilities.Kitchen == facfilter.Kitchen && r.Facilities.TV == facfilter.TV
                && r.Facilities.Internet == facfilter.Internet);
            }
            if (infrafilter.Cafe != null && infrafilter.Hospital != null)
            {
                rents = rents.Where(r => r.Infrastructure.BusStop == infrafilter.BusStop && r.Infrastructure.Parking == infrafilter.Parking
                && r.Infrastructure.Kindergarten == infrafilter.Kindergarten && r.Infrastructure.Cafe == infrafilter.Cafe
                && r.Infrastructure.Supermarket == infrafilter.Supermarket && r.Infrastructure.Park == infrafilter.Park
                && r.Infrastructure.Hospital == infrafilter.Hospital);
            }                    
            if (sort.HasValue)
            {
                switch(sort)
                {
                    case Sort.ASC:
                        rents = rents.OrderBy(r => r.Cost); //sort by cost in ascending order
                        break;
                    case Sort.DESC:
                        rents = rents.OrderByDescending(r => r.Cost); //sort by cost in descending order
                        break;
                }
            }
            return await rents.ToListAsync();
        }

        /// <summary>
        /// Find specific rental by ID
        /// </summary>
        // GET: Rentals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Rental>> GetRental(int id)
        {
            var rental = await _context.Rental.Include(r => r.Facilities).Include(r => r.Infrastructure).Include(r => r.Photos).
                SingleOrDefaultAsync(r => r.RentalID == id);
            if (rental == null)
            {
                return NotFound();
            }
            return rental;
        }
        /// <summary>
        /// Edits specific rental by ID
        /// </summary>
        // PUT: Rentals/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRental(int id, Rental rental)
        {
            if (id != rental.RentalID)
            {
                return BadRequest();
            }

            _context.Entry(rental).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RentalExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        /// <summary>
        /// Create new rental
        /// </summary>
        // POST: Rentals
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD. 
        [HttpPost]
        public async Task<ActionResult<Rental>> PostRental(RentalViewModel rvm)
        {
            Rental rental = new Rental {
                Region = rvm.Region,
                Street = rvm.Street,
                Rooms = rvm.Rooms,
                Cost = rvm.Cost,
                PropertyType = rvm.PropertyType,
                RentTime = rvm.RentTime,
                Description = rvm.Description,
                Latitude = rvm.Latitude,
                Longitude = rvm.Longitude,
                Facilities = rvm.Facilities,
                Infrastructure = rvm.Infrastructure
            };
            if(rvm.Photos!=null)
            {
                int count = 0;
                foreach (var img in rvm.Photos)
                {
                    byte[] ImageData = null;
                    using (var binaryReader = new BinaryReader(img.OpenReadStream()))
                    {
                        ImageData = binaryReader.ReadBytes((int)img.Length);
                    }
                    // установка массива байтов
                    rental.Photos[count].Image = ImageData;
                    count++;
                }              
            }
            _context.Rental.Add(rental);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRental", new { id = rental.RentalID }, rental);
        }
        /// <summary>
        /// Delete specific rental by ID
        /// </summary>
        // DELETE: Rentals/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Rental>> DeleteRental(int id)
        {
            var rental = await _context.Rental.FindAsync(id);
            if (rental == null)
            {
                return NotFound();
            }

            _context.Rental.Remove(rental);
            await _context.SaveChangesAsync();
            return rental;
        }

        private bool RentalExists(int id)
        {
            return _context.Rental.Any(e => e.RentalID == id);
        }        
    }
}

