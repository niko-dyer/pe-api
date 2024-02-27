using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapi.Models
{
    [PrimaryKey(nameof(ProvisionId))]
    public class Provision
    {
        public int ProvisionId { get; set; }

        public string? ProvisionType { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime ProvisionDate { get; set; }

        [Required]
        public string RecipientName { get; set; }

        [Required]
        public string ZipCode { get; set; }

        public virtual List<ProvisionedDevice>? ProvisionedDevices { get; set; }
        
        [Required]
        public int TotalDeviceCount { get; set; }
        //Campaign navigation property was missing
        //Required because provisions only exist to serve campaigns
        [Required]
        public Campaign Campaign { get; set; } = null!;
    }
}