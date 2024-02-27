using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using webapi.Data;
using webapi.Models;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrentDevicesController : ControllerBase
    {

        public class CurrentDeviceRequest
        {
            public int? DeviceTypeId { get; set; }

            public int? Count { get; set; }
            public string? Grade { get; set; }
            public string? Location { get; set; }
        }

        public class UpdatedDevices
        {
            public IEnumerable<CurrentDeviceRequest>? Additions { get; set; }
            public IEnumerable<CurrentDeviceRequest>? Deletions { get; set; }

        }

        private readonly webapiContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentDevicesController(webapiContext context, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }


        [HttpGet("GetCurrentDevices")]
        [Authorize]
        public ActionResult<object> GetCurrentDevices()
        {

            if (_context.CurrentDevices == null)
            {
                return NotFound("Current device table is currently empty.");
            }
            //Query joins CurrentDevices and DeviceTypes to get the sum the count of each unique device type
            var deviceQuery = from cD in _context.CurrentDevices
                              join dT in _context.DeviceTypes on cD.DeviceType.DeviceTypeId equals dT.DeviceTypeId
                              group cD by new { dT.Category, dT.Type, dT.Size, cD.Grade, cD.Location, dT.DeviceTypeId }
                              into grouped
                              select new
                              {
                                  Category = grouped.Key.Category,
                                  Type = grouped.Key.Type,
                                  Size = grouped.Key.Size,
                                  Grade = grouped.Key.Grade,
                                  Location = grouped.Key.Location,
                                  Count = grouped.Count(),
                                  TypeID = grouped.Key.DeviceTypeId
                              };

            return Ok(deviceQuery);

        }

        // GET: api/CurrentDevices/5
        [HttpGet("GetById")]
        [Authorize]
        public async Task<ActionResult<CurrentDevice>> GetCurrentDeviceById(int id)
        {
            if (_context.CurrentDevices == null)
            {
                return NotFound("Current device table is currently empty.");
            }
            var currentDevice = await _context.CurrentDevices.FindAsync(id);

            if (currentDevice == null)
            {
                return NotFound("There are no devices in inventory by that Id.");
            }

            return currentDevice;
        }
        /// <summary>
        /// Gets a device type by primary key attributes
        /// </summary>
        /// <param name="category">Device Category</param>
        /// <param name="type">Device Type</param>
        /// <param name="size">Device Size</param>
        /// <param name="location">Device Location</param>
        /// <returns>Ok if found</returns>
        [HttpGet("GetByPK")]
        [Authorize]
        public async Task<ActionResult<CurrentDevice>> GetCurrentDeviceByPK(string category, string type, string size, string location)
        {
            //
            if (_context.CurrentDevices == null)
            {
                return NotFound("Current device table is currently empty.");
            }

            var query = from c in _context.CurrentDevices
                        join t in _context.DeviceTypes on c.DeviceType equals t
                        where c.Location == location && t.CategoryNormalized == category.ToUpper()
                        && t.TypeNormalized == type.ToUpper() && t.SizeNormalized == size.ToUpper()
                        select c;
            if (query.Any())
            {
                return Ok(query.First());
            }
            else
            {
                return NotFound("Could not find any devices with these parameters.");
            }

        }


        /// <summary>
        /// Updates current device counts and adds new devices types as needed
        /// </summary>
        /// <param name="updates">Wrapper Type that contains a list of additions and deletions</param>
        /// <returns>
        /// 200 Ok if update sucessful
        /// 400 BadRequest if it fails at anypoint
        /// </returns>
        [HttpPut("UpdateCurrentDevices")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CurrentDevice>> UpdateCurrentDevices([FromBody] UpdatedDevices updates)
        {
            var additions = updates.Additions.ToList();
            var deletions = updates.Deletions.ToList();

            try
            {
                //TODO: Additions need general error handling
                //TODO: Additions need error handling for when too many devices are added to the inventory
                if (additions.Any())
                {
                    foreach (var addition in additions)
                    {
                        var DeviceTypeQuery = (from t in _context.DeviceTypes
                                               where t.DeviceTypeId == addition.DeviceTypeId
                                               select t).First();
                        if (DeviceTypeQuery != null)
                        {
                            for (int i = 0; i < addition.Count; i++)
                            {

                                CurrentDevice c = new CurrentDevice
                                {
                                    DeviceType = DeviceTypeQuery,
                                    Grade = addition.Grade,
                                    Location = addition.Location,
                                };
                                AddTracking(c, "ADDITION");
                                _context.CurrentDevices.Add(c);

                            }
                        }
                        else
                        {
                            return BadRequest("No device types found with associated ID");
                        }
                    }
                    await _context.SaveChangesAsync();
                }
                if (deletions.Any())
                {
                    foreach (var deletion in deletions)
                    {
                        //Joins two tables together to search for 
                        var MatchingDevices = (from c in _context.CurrentDevices.Include(x => x.DeviceType)
                                               join t in _context.DeviceTypes on c.DeviceType.DeviceTypeId equals t.DeviceTypeId
                                               where c.DeviceType.DeviceTypeId == deletion.DeviceTypeId && deletion.Grade == c.Grade
                                               select c).ToList();
                        if (MatchingDevices.Count() >= deletions.Count)
                        {

                            for (int i = 0; i < deletion.Count; i++)
                            {
                                _context.CurrentDevices.Remove(MatchingDevices[i]);
                                AddTracking(MatchingDevices[i], "DELETION");
                            }
                        }
                        else
                        {
                            return NotFound("Number of deletions exceed what is currently in the inventory");
                        }
                    }
                    await _context.SaveChangesAsync();
                }
                return Ok("Changes made successfully.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private bool CurrentDeviceExists(int id)
        {
            return (_context.CurrentDevices?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private void AddTracking(CurrentDevice device, string action)
        {

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User user = _userManager.FindByIdAsync(userId).Result;
            DeviceType type = (from t in _context.DeviceTypes
                               where t.DeviceTypeId == device.DeviceType.DeviceTypeId
                               select t).First();
            CurrentDeviceHistory tracker = new CurrentDeviceHistory
            {
                CurrentHistoryCreatedBy = user,
                Action = action,
                CreatedOn = DateTime.Now,
                DeviceType = type,
                Grade = device.Grade,
                Location = device.Location

            };
            _context.CurrentDevicesHistory.Add(tracker);
            _context.SaveChanges();
        }
    }
}
