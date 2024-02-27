using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.Data;
using webapi.Models;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonationsController : ControllerBase
    {
        private readonly webapiContext _context;

        public DonationsController(webapiContext context)
        {
            _context = context;
        }

        // GET: api/Donations
        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin,Reviewer,Employee")]
        public async Task<ActionResult<IEnumerable<Donation>>> GetDonation()
        {
          if (_context.Donations == null)
          {
              return NotFound("The donations table is currently empty.");
          }
            
            return await _context.Donations.ToListAsync();
        }

        // GET: api/Donations/5
        [HttpGet("GetById/{id}")]
        [Authorize(Roles = "Admin,Reviewer,Employee")]
        public async Task<ActionResult<Donation>> GetDonation(int id)
        {
          if (_context.Donations == null)
          {
              return NotFound("The donations table is currently empty.");
          }
            var donation = await _context.Donations.FindAsync(id);

            if (donation == null)
            {
                return NotFound("There is no donation by that Id.");
            }

            return donation;
        }

        // PUT: api/Donations/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("Edit/{id}")]
        [Authorize(Roles = "Admin,Reviewer")]
        public async Task<IActionResult> PutDonation(int id, Donation donation)
        {
            if (id != donation.DonationId)
            {
                return BadRequest("The Id provided does not match the Id of the new donation.");
            }

            _context.Entry(donation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DonationExists(id))
                {
                    return NotFound("An Id by that name does not exist.");
                }
                else
                {
                    throw;
                }
            }

            return Ok("Donation updated successfully.");
        }

        // POST: api/Donations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Create")]
        [Authorize(Roles = "Admin,Reviewer")]
        public async Task<ActionResult<Donation>> PostDonation(Donation donation)
        {
          if (_context.Donations == null)
          {
              return Problem("Entity set 'webapiContext.Donation'  is null.");
          }
            _context.Donations.Add(donation);
            await _context.SaveChangesAsync();

            return Ok("Donation created successfully.");
        }

        // DELETE: api/Donations/5
        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "Admin,Reviewer")]
        public async Task<IActionResult> DeleteDonation(int id)
        {
            if (_context.Donations == null)
            {
                return NotFound("The donations table is currently empty.");
            }
            var donation = await _context.Donations.FindAsync(id);
            if (donation == null)
            {
                return NotFound("There is no donation found by that Id.");
            }

            _context.Donations.Remove(donation);
            await _context.SaveChangesAsync();

            return Ok("Donation deleted successfully.");
        }

        private bool DonationExists(int id)
        {
            return (_context.Donations?.Any(e => e.DonationId == id)).GetValueOrDefault();
        }
    }
}
