using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DopAdoption.Domain.DomainModels
{
    public class AdoptionApplication : BaseEntity
    {
        public Guid DogId { get; set; }
        public Guid AdopterId { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "Pending"; 

        public string? Notes { get; set; }
        public virtual Dog? Dog { get; set; }
        public virtual Adopter? Adopter { get; set; }
    }
}
