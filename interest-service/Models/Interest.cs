using System.ComponentModel.DataAnnotations;

namespace interest_service.Models
{
    public class Interest
    {
        public long Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }
    }
}
