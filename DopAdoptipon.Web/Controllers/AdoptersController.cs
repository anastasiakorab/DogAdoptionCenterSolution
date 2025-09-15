using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DopAdoption.Domain.DomainModels;
using DopAdoption.Service.Interface;

namespace DopAdoption.Web.Controllers
{
    public class AdoptersController : Controller
    {
        private readonly IAdoptersService _adoptersService;

        public AdoptersController(IAdoptersService AdoptersService)
        {
            _adoptersService = AdoptersService;
        }

        // GET: Adopters
        public IActionResult Index()
        {
            return View(_adoptersService.GetAll());
        }

        // GET: Adopters/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null) return NotFound();

            var adopter = _adoptersService.GetById(id.Value);
            if (adopter == null) return NotFound();

            return View(adopter);
        }

        // GET: Adopters/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Adopters/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("FullName,Email,Phone,Id")] Adopter adopter)
        {
            if (ModelState.IsValid)
            {
                adopter.Id = Guid.NewGuid();
                _adoptersService.Add(adopter);
                return RedirectToAction(nameof(Index));
            }
            return View(adopter);
        }

        // GET: Adopters/Edit/5
        public IActionResult Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var adopter = _adoptersService.GetById(id.Value);
            if (adopter == null) return NotFound();

            return View(adopter);
        }

        // POST: Adopters/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("FullName,Email,Phone,Id")] Adopter adopter)
        {
            if (id != adopter.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _adoptersService.Update(adopter);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdopterExists(adopter.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(adopter);
        }

        // GET: Adopters/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null) return NotFound();

            var adopter = _adoptersService.GetById(id.Value);
            if (adopter == null) return NotFound();

            return View(adopter);
        }

        // POST: Adopters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var adopter = _adoptersService.GetById(id);
            if (adopter != null)
            {
                _adoptersService.DeleteById(id);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool AdopterExists(Guid id)
        {
            return _adoptersService.GetById(id) != null;
        }
    }
}