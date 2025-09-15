using DopAdoption.Domain.DomainModels;
using DopAdoption.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using System;
using System.Linq;

namespace DopAdoption.Web.Controllers
{
    public class AdoptionApplicationsController : Controller
    {
        private readonly IAdoptionApplicationsService _applicationsService;
        private readonly IDogsService _dogsService;
        private readonly IAdoptersService _adoptersService;

        public AdoptionApplicationsController(
            IAdoptionApplicationsService ApplicationsService,
            IDogsService DogsService,
            IAdoptersService AdoptersService)
        {
            _applicationsService = ApplicationsService;
            _dogsService = DogsService;
            _adoptersService = AdoptersService;
        }

        // GET: AdoptionApplications
        public IActionResult Index()
        {
            return View(_applicationsService.GetAll());
        }

        // GET: AdoptionApplications/Details/5
        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var app = _applicationsService.GetById(id.Value);
            if (app == null)
            {
                return NotFound();
            }

            return View(app);
        }

        // GET: AdoptionApplications/Create
        public IActionResult Create()
        {
            ViewData["DogId"] = new SelectList(_dogsService.GetAll(), "Id", "Name");
            ViewData["AdopterId"] = new SelectList(_adoptersService.GetAll(), "Id", "FullName");
            return View();
        }

        // POST: AdoptionApplications/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("DogId,AdopterId,Status,Notes,Id")] AdoptionApplication Application)
        {
            if (ModelState.IsValid)
            {
                Application.Id = Guid.NewGuid();
               
                _applicationsService.Add(Application);
                return RedirectToAction(nameof(Index));
            }
            ViewData["DogId"] = new SelectList(_dogsService.GetAll(), "Id", "Name", Application.DogId);
            ViewData["AdopterId"] = new SelectList(_adoptersService.GetAll(), "Id", "FullName", Application.AdopterId);
            return View(Application);
        }

        // GET: AdoptionApplications/Edit/5
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var app = _applicationsService.GetById(id.Value);
            if (app == null)
            {
                return NotFound();
            }

            ViewData["DogId"] = new SelectList(_dogsService.GetAll(), "Id", "Name", app.DogId);
            ViewData["AdopterId"] = new SelectList(_adoptersService.GetAll(), "Id", "FullName", app.AdopterId);
            return View(app);
        }

        // POST: AdoptionApplications/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("DogId,AdopterId,Status,Notes,Id")] AdoptionApplication Application)
        {
            if (id != Application.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _applicationsService.Update(Application);
                }
                catch
                {
                    if (!ApplicationExists(Application.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["DogId"] = new SelectList(_dogsService.GetAll(), "Id", "Name", Application.DogId);
            ViewData["AdopterId"] = new SelectList(_adoptersService.GetAll(), "Id", "FullName", Application.AdopterId);
            return View(Application);
        }

        // GET: AdoptionApplications/Delete/5
        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var app = _applicationsService.GetById(id.Value);
            if (app == null)
            {
                return NotFound();
            }
            return View(app);
        }

        // POST: AdoptionApplications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var app = _applicationsService.GetById(id);
            if (app != null)
            {
                _applicationsService.DeleteById(id);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ApplicationExists(Guid id)
        {
            return _applicationsService.GetById(id) != null;
        }
    }
}

