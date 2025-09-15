using Microsoft.EntityFrameworkCore;
using DopAdoption.Domain.DomainModels;
using DopAdoption.Repository;
using DopAdoption.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DopAdoption.Service.Implementation
{
    public class AdoptionApplicationsService : IAdoptionApplicationsService
    {
        private readonly IRepository<AdoptionApplication> _appRepository;

        public AdoptionApplicationsService(IRepository<AdoptionApplication> AppRepository)
        {
            _appRepository = AppRepository;
        }

        public AdoptionApplication Add(AdoptionApplication Application)
        {
            return _appRepository.Insert(Application);
        }

        public AdoptionApplication DeleteById(Guid Id)
        {
            var Application = _appRepository.Get(selector: x => x, predicate: y => y.Id == Id);
            return _appRepository.Delete(Application);
        }

        public List<AdoptionApplication> GetAll()
        {
            return _appRepository.GetAll(selector: x => x,
                include: x => x.Include(y => y.Adopter)
                               .Include(y => y.Dog))
                .ToList();
        }

        public AdoptionApplication? GetById(Guid Id)
        {
            return _appRepository.Get(selector: x => x,
                predicate: y => y.Id == Id,
                include: x => x.Include(y => y.Adopter)
                               .Include(y => y.Dog));
        }

        public AdoptionApplication Update(AdoptionApplication Application)
        {
            return _appRepository.Update(Application);
        }

        public ICollection<AdoptionApplication> InsertMany(ICollection<AdoptionApplication> Applications)
        {
            return _appRepository.InsertMany(Applications);
        }
    }
}