using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace webapi.Models
{
    [PrimaryKey(nameof(CampaignId))]
    public class Campaign
    {
        public int CampaignId { get; set; }
        
        [Required]
        public string CampaignName { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime StartDate { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime EndDate { get; set; }
        
        //Added missing navigation property
        //List because one campaign can potentially have multiple provisions
        //Null because provisions might not have been decided upon yet
        public virtual List<Provision>? Provisions { get; set; } 
        
    }
}
