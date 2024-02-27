using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace webapi.Models
{
    [PrimaryKey(nameof(DeviceId))]
    public class ProvisionedDevice
    {
        public int DeviceId { get; set; }

        [Required]
        public DeviceType DeviceType { get; set; }
        //Added DeviceGrade
        [Required]
        public string DeviceGrade { get; set; }
        
        [Required]
        public int DeviceCount { get; set; }
        //Removed campaign and adjusted from donation to provision
        //Campaign removed because we can easily use joins/nav properties to find the associated campaign
        //Required because every group of provisioned devices belongs to some provision and we cannot leave the foreign key blank
        [Required]
        public virtual Provision Provision { get; set; } = null!;
    }
}
