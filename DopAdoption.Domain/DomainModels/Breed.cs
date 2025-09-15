using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DopAdoption.Domain.DomainModels
{
    public class Breed : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public virtual ICollection<Dog> Dogs { get; set; } = new List<Dog>();
    }
}
