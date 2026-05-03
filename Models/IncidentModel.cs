using System.ComponentModel.DataAnnotations;

namespace DisasterMIS_Frontend.Models
{
    public class IncidentModel
    {
        [Required]
        public string Location { get; set; }

        [Required]
        public decimal Latitude { get; set; }

        [Required]
        public decimal Longitude { get; set; }

        [Required]
        public string DisasterType { get; set; }

        [Required]
        public string SeverityLevel { get; set; }

        public int PriorityLevel { get; set; } = 3;

        public string? ReporterContact { get; set; }
    }
}