using DopAdoption.Domain.DTOs;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace DopAdoption.Service.Interface
{
    public interface IExternalDogService
    {
        Task<DogApiDTO> GetRandomAsync();
        Task<List<DogApiDTO>> GetRandomManyAsync(int count); 
    }
}

