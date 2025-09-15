using Microsoft.EntityFrameworkCore;
using DopAdoption.Domain.DomainModels;
using DopAdoption.Repository;
using DopAdoption.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DopAdoption.Service.Implementation
{
    public class BreedsService : IBreedsService
    {
        private readonly IRepository<Breed> _breedRepository;

        public BreedsService(IRepository<Breed> BreedRepository)
        {
            _breedRepository = BreedRepository;
        }

        public Breed Add(Breed Breed)
        {
            return _breedRepository.Insert(Breed);
        }

        public Breed DeleteById(Guid Id)
        {
            var Breed = _breedRepository.Get(selector: x => x, predicate: y => y.Id == Id);
            return _breedRepository.Delete(Breed);
        }

        public List<Breed> GetAll()
        {
            return _breedRepository.GetAll(selector: x => x, include: x => x.Include(y => y.Dogs)).ToList();
        }

        public Breed? GetById(Guid Id)
        {
            return _breedRepository.Get(selector: x => x, predicate: y => y.Id == Id, include: x => x.Include(b => b.Dogs));
        }

        public Breed Update(Breed Breed)
        {
            return _breedRepository.Update(Breed);
        }

        public ICollection<Breed> InsertMany(ICollection<Breed> Breeds)
        {
            return _breedRepository.InsertMany(Breeds);
        }
    }
}