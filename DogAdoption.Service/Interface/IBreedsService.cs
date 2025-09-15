using DopAdoption.Domain.DomainModels;
using System;
using System.Collections.Generic;

namespace DopAdoption.Service.Interface
{
    public interface IBreedsService
    {
        List<Breed> GetAll();
        Breed? GetById(Guid id);
        Breed Add(Breed breed);
        Breed Update(Breed breed);
        Breed DeleteById(Guid id);
        ICollection<Breed> InsertMany(ICollection<Breed> breeds);
    }
}