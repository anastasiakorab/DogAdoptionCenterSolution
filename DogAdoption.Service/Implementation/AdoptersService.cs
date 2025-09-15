using Microsoft.EntityFrameworkCore;
using DopAdoption.Domain.DomainModels;
using DopAdoption.Repository;
using DopAdoption.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DopAdoption.Service.Implementation
{
    public class AdoptersService : IAdoptersService
    {
        private readonly IRepository<Adopter> _adopterRepository;

        public AdoptersService(IRepository<Adopter> AdopterRepository)
        {
            _adopterRepository = AdopterRepository;
        }

        public Adopter Add(Adopter Adopter)
        {
            return _adopterRepository.Insert(Adopter);
        }

        public Adopter DeleteById(Guid Id)
        {
            var Adopter = _adopterRepository.Get(selector: x => x, predicate: y => y.Id == Id);
            return _adopterRepository.Delete(Adopter);
        }

        public List<Adopter> GetAll()
        {
            return _adopterRepository.GetAll(selector: x => x, include: x => x.Include(y => y.Applications)).ToList();
        }

        public Adopter? GetById(Guid Id)
        {
            return _adopterRepository.Get(selector: x => x, predicate: y => y.Id == Id, include: x => x.Include(a => a.Applications));
        }

        public Adopter Update(Adopter Adopter)
        {
            return _adopterRepository.Update(Adopter);
        }

        public ICollection<Adopter> InsertMany(ICollection<Adopter> Adopters)
        {
            return _adopterRepository.InsertMany(Adopters);
        }
    }
}