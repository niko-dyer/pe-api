using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace webapi.Models
{
    [PrimaryKey(nameof(Id))]
    public class CurrentDeviceHistory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [InverseProperty("CurrentDeviceHistories")]
        public virtual User CurrentHistoryCreatedBy { get; set; } = null!;

        [Required]
        [Column(TypeName = "date")]
        public DateTime CreatedOn { get; set; }

/*        [NotMapped]
        [Column(TypeName = "date")]
        public DateTime? ApprovedOn { get; set; }
        [NotMapped]
        [InverseProperty("ApprovedDonationReviews")]
        public virtual User? ApprovedBy { get; set; }

        [NotMapped]
        [Required]
        public string? ReviewStatus { get; set; } = null!;*/
        [Required]
        public string Action { get; set; }

        [Required]
        public virtual DeviceType DeviceType { get; set; } = null!;

        public string Location { get; set; }

        public string Grade { get; set; } = null!;


    }
}
