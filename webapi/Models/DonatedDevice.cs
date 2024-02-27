using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace webapi.Models
{
    [PrimaryKey(nameof(DeviceId))]
    public class DonatedDevice
    {

        public int DeviceId { get; set; }
        [Required]
        public DeviceType DeviceType { get; set; }
        public string? DeviceGrade { get; set; }
        
        [Required]
        public int DeviceCount { get; set; }

        
        

        [Required]
        public virtual Donation Donation { get; set; } = null!;
    }
}
