using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapi.Models
{
    [PrimaryKey(nameof(DonationId))]
    public class Donation
    {
        public int DonationId { get; set; }

        public string? DonationType { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime DonationDate { get; set; }

        [Required]
        public string DonorName { get; set; }

        [Required]
        public string DonorLocation { get; set; }

        public virtual List<DonatedDevice>? DonatedDevices { get; set; }

        [Required]
        public int TotalDeviceCount { get; set; }

        public virtual DonationReview DonationReview { get; set; } = null!;

        public Campaign? Campaign { get; set; }
    }
}