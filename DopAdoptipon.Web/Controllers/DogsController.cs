using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DopAdoption.Domain.DomainModels;
using DopAdoption.Service.Interface;

namespace DopAdoption.Web.Controllers
{
    public class DogsController : Controller
    {
        private readonly IDogsService _dogsService;
        private readonly IBreedsService _breedsService;
        private readonly IExternalDogService _externalDogService;

       
        private static readonly string[] _statusOptions = new[] { "Available", "Reserved", "Adopted" };
        private static readonly string[] _sexOptions = new[] { "M", "F" };

        public DogsController(IDogsService DogsService, IBreedsService BreedsService, IExternalDogService ExternalDogService)
        {
            _dogsService = DogsService;
            _breedsService = BreedsService;
            _externalDogService = ExternalDogService;
        }

        // GET: Dogs
        public IActionResult Index()
        {
            return View(_dogsService.GetAll());
        }

        // GET: Dogs
        [HttpGet("Dogs/ApiPing")]
        public async Task<IActionResult> ApiPing()
        {
            try
            {
                var dto = await _externalDogService.GetRandomAsync();
                return Ok(dto); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, "API error: " + ex.Message);
            }
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RandomPhoto()
        {
            try
            {
                var p = await _externalDogService.GetRandomAsync();
                TempData["RandomUrl"] = p.PhotoLink;
                TempData["RandomBreed"] = p.TypeOfDog;
                TempData["RandomSubBreed"] = p.DogVariety;
                TempData["RandomProvider"] = p.Source;
                TempData["Success"] = "Random dog photo loaded from API.";
            }
            catch
            {
                TempData["Error"] = "Could not load photo from API.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RandomPhotos(int count = 4)
        {
            try
            {
                var photos = await _externalDogService.GetRandomManyAsync(count);
                ViewBag.RandomPhotos = photos;
            }
            catch
            {
                TempData["Error"] = "Could not load photos from API.";
            }
            return View("Index", _dogsService.GetAll());
        }

        // GET: Dogs/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null) return NotFound();

            var dog = _dogsService.GetById(id.Value);
            if (dog == null) return NotFound();

            return View(dog);
        }

        // GET: Dogs/Create
        public IActionResult Create()
        {
            ViewData["BreedId"] = new SelectList(_breedsService.GetAll(), "Id", "Name");
            ViewBag.Statuses = new SelectList(_statusOptions, "Available");
            ViewBag.Sexes = new SelectList(_sexOptions, "M");
            return View();
        }

        // POST: Dogs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Name,Age,Sex,BreedId,Status,Id")] Dog dog)
        {
            if (ModelState.IsValid)
            {
                if (dog.Id == Guid.Empty) dog.Id = Guid.NewGuid();
                _dogsService.Add(dog);
                TempData["Success"] = $"Dog '{dog.Name}' created.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["BreedId"] = new SelectList(_breedsService.GetAll(), "Id", "Name", dog.BreedId);
            ViewBag.Statuses = new SelectList(_statusOptions, dog.Status);
            ViewBag.Sexes = new SelectList(_sexOptions, dog.Sex);
            return View(dog);
        }

        // GET: Dogs/Edit/5
        public IActionResult Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var dog = _dogsService.GetById(id.Value);
            if (dog == null) return NotFound();

            ViewData["BreedId"] = new SelectList(_breedsService.GetAll(), "Id", "Name", dog.BreedId);
            ViewBag.Statuses = new SelectList(_statusOptions, dog.Status);
            ViewBag.Sexes = new SelectList(_sexOptions, dog.Sex);
            return View(dog);
        }

        // POST: Dogs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("Name,Age,Sex,BreedId,Status,Id")] Dog dog)
        {
            if (id != dog.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _dogsService.Update(dog);
                    TempData["Success"] = $"Dog '{dog.Name}' updated.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DogExists(dog.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["BreedId"] = new SelectList(_breedsService.GetAll(), "Id", "Name", dog.BreedId);
            ViewBag.Statuses = new SelectList(_statusOptions, dog.Status);
            ViewBag.Sexes = new SelectList(_sexOptions, dog.Sex);
            return View(dog);
        }

        // GET: Dogs/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null) return NotFound();

            var dog = _dogsService.GetById(id.Value);
            if (dog == null) return NotFound();

            return View(dog);
        }

        // POST: Dogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var dog = _dogsService.GetById(id);
            if (dog != null)
            {
                try
                {
                    _dogsService.DeleteById(id);
                    TempData["Success"] = $"Dog '{dog.Name}' deleted.";
                }
                catch
                {
                    TempData["Error"] = "Cannot delete this dog.";
                }
            }
            return RedirectToAction(nameof(Index));
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarkAsAdopted(Guid id)
        {
            _dogsService.MarkAsAdopted(id);
            TempData["Success"] = "Dog marked as adopted.";
            return RedirectToAction(nameof(Index));
        }

        private bool DogExists(Guid id)
        {
            return _dogsService.GetById(id) != null;
        }
    }
}