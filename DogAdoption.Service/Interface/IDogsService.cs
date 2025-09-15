using DopAdoption.Domain.DomainModels;
using System;
using System.Collections.Generic;

namespace DopAdoption.Service.Interface
{
    public interface IDogsService
    {
        List<Dog> GetAll();
        Dog? GetById(Guid id);
        Dog Add(Dog dog);
        Dog Update(Dog dog);
        Dog DeleteById(Guid id);
        ICollection<Dog> InsertMany(ICollection<Dog> dogs);

        Dog MarkAsAdopted(Guid id);
        
    }
}