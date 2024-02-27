using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace webapi.Models
{
    [PrimaryKey(nameof(DonationReviewId))]
    public class DonationReview
    {
        
        public int DonationReviewId { get; set; }

        [Required]
        [InverseProperty("CreatedDonationReviews")]
        public virtual User DonationCreatedBy { get; set; } = null!;

        [Required]
        [Column(TypeName = "date")]
        public DateTime CreatedOn { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ApprovedOn { get; set; }

        [InverseProperty("ApprovedDonationReviews")]
        public virtual User? DonationApprovedBy { get; set; }

        [Required] 
        public string DonationStatus { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(Models.Donation.DonationId))]
        public virtual Donation Donation { get; set; } = null!;
        

    }
}
