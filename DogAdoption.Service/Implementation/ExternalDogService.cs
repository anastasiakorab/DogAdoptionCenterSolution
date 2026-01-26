using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using DopAdoption.Domain.DTOs;
using DopAdoption.Service.Interface;

namespace DopAdoption.Service.Implementation
{
    public class ExternalDogService : IExternalDogService
    {
        private readonly HttpClient _http;
        public ExternalDogService(HttpClient httpClient) { _http = httpClient; }

        private class DogCeoResponse { public string? message { get; set; } public string? status { get; set; } }
        private class DogCeoManyResponse { public string[]? message { get; set; } public string? status { get; set; } }

        public async Task<DogApiDTO> GetRandomAsync()
        {
            string json = await _http.GetStringAsync("https://dog.ceo/api/breeds/image/random");
            var api = JsonSerializer.Deserialize<DogCeoResponse>(json);
            string url = api?.message ?? string.Empty;
            return ParseDto(url);
        }

        public async Task<List<DogApiDTO>> GetRandomManyAsync(int count)
        {
            if (count < 1) count = 1;
            string json = await _http.GetStringAsync($"https://dog.ceo/api/breeds/image/random/{count}");
            var api = JsonSerializer.Deserialize<DogCeoManyResponse>(json);
            var urls = api?.message ?? Array.Empty<string>();

            var list = new List<DogApiDTO>();
            foreach (var url in urls)
            {
                list.Add(ParseDto(url));
            }
            return list;
        }

        private DogApiDTO ParseDto(string url)
        {
            var dto = new DogApiDTO { PhotoLink = url ?? string.Empty, Source = "dog.ceo" };

            try
            {
                if (!string.IsNullOrWhiteSpace(url))
                {
                    var u = new Uri(url);
                    var parts = u.Segments.Select(s => s.Trim('/')).ToArray();
                    int idx = Array.FindIndex(parts, p => p.Equals("breeds", StringComparison.OrdinalIgnoreCase));
                    if (idx >= 0 && idx + 1 < parts.Length)
                    {
                        var raw = parts[idx + 1];
                        var b = raw.Split('-', StringSplitOptions.RemoveEmptyEntries);
                        if (b.Length > 0)
                        {
                            dto.TypeOfDog = dto.TypeOfDog = char.ToUpper(b[0][0]) + b[0].Substring(1);
                            if (b.Length > 1)
                            {
                                var dv = string.Join(" ", b, 1, b.Length - 1);
                                dto.DogVariety = char.ToUpper(dv[0]) + dv.Substring(1);
                            }
                        }
                    }
                }
            }
            catch { }

            return dto;
        }
    }
}