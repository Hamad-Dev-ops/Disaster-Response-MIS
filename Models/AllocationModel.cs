using System.ComponentModel.DataAnnotations;

namespace DisasterMIS_Frontend.Models
{
    public class AllocationModel
    {
        [Required]
        public int RequestID { get; set; }

        [Required]
        public int ResourceID { get; set; }

        [Required]
        public int QuantityAllocated { get; set; }

        [Required]
        public int WarehouseID { get; set; }
    }
}