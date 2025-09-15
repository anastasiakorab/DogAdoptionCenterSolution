using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DopAdoption.Domain.DomainModels
{
    public class Adopter : BaseEntity
    {
        public string FullName { get; set; } 
        public string Email { get; set; } 
        public string? Phone { get; set; }
        public virtual ICollection<AdoptionApplication> Applications { get; set; } = new List<AdoptionApplication>();
    }
}
