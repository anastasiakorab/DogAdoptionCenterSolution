using System;
using Microsoft.AspNetCore.Mvc;
using DopAdoption.Domain.DomainModels;
using DopAdoption.Service.Interface;

namespace DopAdoption.Web.Controllers
{
    public class BreedsController : Controller
    {
        private readonly IBreedsService _breedsService;

        public BreedsController(IBreedsService BreedsService)
        {
            _breedsService = BreedsService;
        }

        // GET: Breeds
        public IActionResult Index()
        {
            return View(_breedsService.GetAll());
        }

        // GET: Breeds/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var breed = _breedsService.GetById(id.Value);
            if (breed == null)
            {
                return NotFound();
            }

            return View(breed);
        }

        // GET: Breeds/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Breeds/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Name,Id")] Breed Breed)
        {
            
            Console.WriteLine("POST /Breeds/Create hit. Name=" + (Breed?.Name ?? "<null>"));

            if (!ModelState.IsValid)
            {
                return View(Breed);
            }

            if (Breed.Id == Guid.Empty)
                Breed.Id = Guid.NewGuid();

            _breedsService.Add(Breed);
            TempData["Success"] = $"Breed '{Breed.Name}' created.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Breeds/Edit/5
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var breed = _breedsService.GetById(id.Value);
            if (breed == null)
            {
                return NotFound();
            }
            return View(breed);
        }

        // POST: Breeds/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("Name,Id")] Breed Breed)
        {
            if (id != Breed.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(Breed);
            }

            _breedsService.Update(Breed);
            TempData["Success"] = $"Breed '{Breed.Name}' updated.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Breeds/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var breed = _breedsService.GetById(id.Value);
            if (breed == null)
            {
                return NotFound();
            }
            return View(breed);
        }

        // POST: Breeds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var breed = _breedsService.GetById(id);
            if (breed != null)
            {
                _breedsService.DeleteById(id);
                TempData["Success"] = $"Breed '{breed.Name}' deleted.";
            }
            return RedirectToAction(nameof(Index));
        }

        
    }
}
