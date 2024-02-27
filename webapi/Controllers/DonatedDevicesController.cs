using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.Data;
using webapi.Models;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonatedDevicesController : ControllerBase
    {
        private readonly webapiContext _context;

        public DonatedDevicesController(webapiContext context)
        {
            _context = context;
        }

        // GET: api/DonationDetails
        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin,Reviewer")]
        public async Task<ActionResult<IEnumerable<DonatedDevice>>> GetDonationDetails()
        {
          if (_context.DonatedDevices == null)
          {
              return NotFound("The donation details table is currently empty.");
          }
            return await _context.DonatedDevices.ToListAsync();
        }

        // GET: api/DonationDetails/5
        [HttpGet("GetById/{id}")]
        [Authorize(Roles = "Admin,Reviewer")]
        public async Task<ActionResult<DonatedDevice>> GetDonationDetails(int id)
        {
          if (_context.DonatedDevices == null)
          {
              return NotFound("The donations table is currently empty.");
          }
            var donationDetails = await _context.DonatedDevices.FindAsync(id);

            if (donationDetails == null)
            {
                return NotFound("There is no donation found with Id.");
            }

            return donationDetails;
        }

        // PUT: api/DonationDetails/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("Edit/{id}")]
        [Authorize(Roles = "Admin,Reviewer")]
        public async Task<IActionResult> PutDonationDetails(int id, DonatedDevice donatedDevices)
        {
            if (id != donatedDevices.DeviceId)
            {
                return BadRequest("The Id provided does not match the updated donated device Id.");
            }

            _context.Entry(donatedDevices).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DonationDetailsExists(id))
                {
                    return NotFound("Donation details does not contain anything with that Id.");
                }
                else
                {
                    throw;
                }
            }

            return Ok("Donation details updated succesfully.");
        }

        // POST: api/DonationDetails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Create")]
        [Authorize(Roles = "Admin,Reviewer")]
        public async Task<ActionResult<DonatedDevice>> PostDonationDetails(DonatedDevice donatedDevices)
        {
          if (_context.DonatedDevices == null)
          {
              return Problem("Entity set 'webapiContext.DonationDetails'  is null.");
          }
            _context.DonatedDevices.Add(donatedDevices);
            await _context.SaveChangesAsync();

            return Ok("Donation details created successfully.");
        }

        // DELETE: api/DonationDetails/5
        [HttpDelete("DeleteById/{id}")]
        [Authorize(Roles = "Admin,Reviewer")]
        public async Task<IActionResult> DeleteDonationDetails(int id)
        {
            if (_context.DonatedDevices == null)
            {
                return NotFound("The donations details table is currently empty.");
            }
            var donationDetails = await _context.DonatedDevices.FindAsync(id);
            if (donationDetails == null)
            {
                return NotFound("The donations details table does not contain anything with that Id.");
            }

            _context.DonatedDevices.Remove(donationDetails);
            await _context.SaveChangesAsync();

            return Ok("Donation details deleted successfully.");
        }

        private bool DonationDetailsExists(int id)
        {
            return (_context.DonatedDevices?.Any(e => e.DeviceId == id)).GetValueOrDefault();
        }
    }
}
