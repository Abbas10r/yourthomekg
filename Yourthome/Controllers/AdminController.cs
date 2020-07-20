﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Yourthome.Data;
using Yourthome.Helpers;
using Yourthome.Models;
using Yourthome.Models.ViewModel;
using Yourthome.Services;

namespace Yourthome.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly YourthomeContext _context;
        private IUserService _userService;
        private IMapper _mapper;
        public AdminController(YourthomeContext context, IUserService userService,
            IMapper mapper)
        {
            _context = context;
            _userService = userService;
            _mapper = mapper;
        }
        /// <summary>
        /// Get all users
        /// </summary>
        [HttpGet("users")]
        [Authorize(Roles = Role.Admin)]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            var users = _userService.GetAll();
            var model = _mapper.Map<IList<UserModel>>(users);
            return Ok(model);
        }
        /// <summary>
        /// get specific user by id
        /// </summary>
        [HttpGet("users/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            var user = _userService.GetById(id);
            var model = _mapper.Map<UserModel>(user);
            return Ok(model);
        }
        /// <summary>
        /// edit user's info
        /// </summary>
        [HttpPut("users/{id}/update")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update(int id, [FromForm]UpdateModel model)
        {
            // map model to entity and set id
            var user = _mapper.Map<User>(model);
            user.Id = id;
            if (user.Avatar != null)
            {
                byte[] ImageData = null;
                using (var binaryReader = new BinaryReader(user.Avatar.OpenReadStream()))
                {
                    ImageData = binaryReader.ReadBytes((int)user.Avatar.Length);
                }
                // установка массива байтов
                user.AvatarStored = ImageData;
            }
            try
            {
                // update user 
                _userService.Update(user, model.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
        /// <summary>
        /// delete user
        /// </summary>
        [HttpDelete("users/{id}/delete")]
        [Authorize(Roles = Role.Admin)]
        public async Task<ActionResult> Delete(int id)
        {
            _userService.Delete(id);
            return Ok();
        }
        /// <summary>
        /// get rentals
        /// </summary>
        [HttpGet("rentals")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Rental>>> GetRental()
        {
            return await _context.Rental.ToListAsync();
        }
        /// <summary>
        /// get rental by id
        /// </summary>
        [HttpGet("rentals/{id}")]
        [Authorize(Roles = Role.Admin)]
        public async Task<ActionResult<Rental>> GetRental(int id)
        {
            var rental = await _context.Rental.Include(r => r.Facilities).Include(r => r.Infrastructure).Include(r => r.Photos).
                Include(r => r.Bookings)
                .SingleOrDefaultAsync(r => r.RentalID == id);
            if (rental == null)
            {
                return NotFound();
            }
            return rental;
        }
        /// <summary>
        /// edit rental
        /// </summary>
        [HttpPut("rentals/{id}/update")]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> PutRental(int id, Rental rental)
        {
            if (id != rental.RentalID)
            {
                return BadRequest();
            }
            _context.Entry(rental).State = EntityState.Modified;
            _context.Entry(rental.Facilities).State = EntityState.Modified;
            _context.Entry(rental.Infrastructure).State = EntityState.Modified;
            foreach (var i in rental.Photos)
            {
                _context.Entry(i).State = EntityState.Modified;
            }
            foreach (var i in rental.Bookings)
            {
                _context.Entry(i).State = EntityState.Modified;
            }
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
        /// delete rental
        /// </summary>
        [HttpDelete("rentals/{id}/delete")]
        [Authorize(Roles = Role.Admin)]
        public async Task<ActionResult<Rental>> DeleteRental(int ID)
        {
            var rental = await _context.Rental.FindAsync(ID);
            if (rental == null)
            {
                return NotFound();
            }

            _context.Rental.Remove(rental);
            await _context.SaveChangesAsync();
            return rental;
        }
        private bool RentalExists(int ID)
        {
            return _context.Rental.Any(e => e.RentalID == ID);
        }
    }
}