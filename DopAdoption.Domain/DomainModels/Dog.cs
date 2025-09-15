using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DopAdoption.Domain.DomainModels
{
    public class Dog : BaseEntity
    {
        public string Name { get; set; }

        //[Range(0, int.MaxValue, ErrorMessage = "Age cannot be negative.")]
        public int Age { get; set; }
        public string Sex { get; set; } = "M"; 

        public Guid BreedId { get; set; }

        
        public string Status { get; set; } = "Available"; 


        public virtual Breed? Breed { get; set; }
        public virtual ICollection<AdoptionApplication> Applications { get; set; } = new List<AdoptionApplication>();
    }
}

