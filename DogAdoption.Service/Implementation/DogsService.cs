using Microsoft.EntityFrameworkCore;
using DopAdoption.Domain.DomainModels;
using DopAdoption.Repository;
using DopAdoption.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DopAdoption.Service.Implementation
{
    public class DogsService : IDogsService
    {
        private readonly IRepository<Dog> _dogRepository;

        public DogsService(IRepository<Dog> DogRepository)
        {
            _dogRepository = DogRepository;
        }

        public Dog Add(Dog Dog)
        {
            return _dogRepository.Insert(Dog);
        }

        public Dog DeleteById(Guid Id)
        {
            var Dog = _dogRepository.Get(selector: x => x, predicate: y => y.Id == Id);
            return _dogRepository.Delete(Dog);
        }

        public List<Dog> GetAll()
        {
            return _dogRepository.GetAll(selector: x => x, include: x => x.Include(y => y.Breed)).ToList();
        }

        public Dog? GetById(Guid Id)
        {
            return _dogRepository.Get(selector: x => x, predicate: y => y.Id == Id, include: x => x.Include(d => d.Breed));
        }

        public Dog Update(Dog Dog)
        {
            return _dogRepository.Update(Dog);
        }

        public ICollection<Dog> InsertMany(ICollection<Dog> Dogs)
        {
            return _dogRepository.InsertMany(Dogs);
        }

        public Dog MarkAsAdopted(Guid Id)
        {
            var Dog = _dogRepository.Get(selector: x => x, predicate: y => y.Id == Id);
            Dog.Status = "Adopted";
            return _dogRepository.Update(Dog);
        }
    }
}