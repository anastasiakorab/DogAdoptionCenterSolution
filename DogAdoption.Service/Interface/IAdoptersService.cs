using DopAdoption.Domain.DomainModels;
using System;
using System.Collections.Generic;

namespace DopAdoption.Service.Interface
{
    public interface IAdoptersService
    {
        List<Adopter> GetAll();
        Adopter? GetById(Guid id);
        Adopter Add(Adopter adopter);
        Adopter Update(Adopter adopter);
        Adopter DeleteById(Guid id);
        ICollection<Adopter> InsertMany(ICollection<Adopter> adopters);
    }
}