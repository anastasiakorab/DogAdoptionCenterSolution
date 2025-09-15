using DopAdoption.Domain.DomainModels;
using System;
using System.Collections.Generic;

namespace DopAdoption.Service.Interface
{
    public interface IAdoptionApplicationsService
    {
        List<AdoptionApplication> GetAll();
        AdoptionApplication? GetById(Guid id);
        AdoptionApplication Add(AdoptionApplication application);
        AdoptionApplication Update(AdoptionApplication application);
        AdoptionApplication DeleteById(Guid id);
        ICollection<AdoptionApplication> InsertMany(ICollection<AdoptionApplication> apps);

        
    }
}