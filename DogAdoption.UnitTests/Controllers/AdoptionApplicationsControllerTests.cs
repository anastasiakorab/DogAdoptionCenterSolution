#pragma warning disable NUnit103

using System;
using System.Collections.Generic;
using DopAdoption.Domain.DomainModels;
using DopAdoption.Service.Interface;
using DopAdoption.Web.Controllers;
using DogAdoption.UnitTests.TestHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using NUnit.Framework;

namespace DogAdoption.UnitTests.Controllers;

[TestFixture]
public class AdoptionApplicationsControllerTests
{
    private Mock<IAdoptionApplicationsService> _apps = null!;
    private Mock<IDogsService> _dogs = null!;
    private Mock<IAdoptersService> _adopters = null!;
    private AdoptionApplicationsController _c = null!;

    [SetUp]
    public void SetUp()
    {
        _apps = new Mock<IAdoptionApplicationsService>();
        _dogs = new Mock<IDogsService>();
        _adopters = new Mock<IAdoptersService>();

        _c = new AdoptionApplicationsController(_apps.Object, _dogs.Object, _adopters.Object);
        ControllerTestHelper.AttachTempData(_c);
    }

    [TearDown]
    public void TearDown()
    {
        if (_c is IDisposable d) d.Dispose();
    }

    [Test]
    public void Index_ReturnsView()
    {
        _apps.Setup(s => s.GetAll()).Returns(new List<AdoptionApplication>());
        Assert.That(_c.Index(), Is.TypeOf<ViewResult>());
    }

    [Test]
    public void Details_NullId_NotFound()
        => Assert.That(_c.Details(null), Is.TypeOf<NotFoundResult>());

    [Test]
    public void Details_NotFound_NotFound()
    {
        _apps.Setup(s => s.GetById(It.IsAny<Guid>())).Returns((AdoptionApplication?)null);
        Assert.That(_c.Details(Guid.NewGuid()), Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public void Details_Found_View()
    {
        var app = new AdoptionApplication { Status = "Pending" };
        _apps.Setup(s => s.GetById(It.IsAny<Guid>())).Returns(app);

        var res = _c.Details(Guid.NewGuid()) as ViewResult;

        Assert.That(res, Is.Not.Null);
        Assert.That(res!.Model, Is.SameAs(app));
    }

    [Test]
    public void CreateGet_ReturnsView_AndSetsSelectLists()
    {
        _dogs.Setup(s => s.GetAll()).Returns(new List<Dog> { new Dog { Id = Guid.NewGuid(), Name = "Rex" } });
        _adopters.Setup(s => s.GetAll()).Returns(new List<Adopter> { new Adopter { Id = Guid.NewGuid(), FullName = "Ana", Email = "a@a.com" } });

        var res = _c.Create();

        Assert.That(res, Is.TypeOf<ViewResult>());
        Assert.That(_c.ViewData["DogId"], Is.TypeOf<SelectList>());
        Assert.That(_c.ViewData["AdopterId"], Is.TypeOf<SelectList>());
    }

    [Test]
    public void CreatePost_InvalidModel_ReturnsView_AndSetsSelectLists()
    {
        _c.ModelState.AddModelError("x", "y");

        var dogId = Guid.NewGuid();
        var adopterId = Guid.NewGuid();

        _dogs.Setup(s => s.GetAll()).Returns(new List<Dog> { new Dog { Id = dogId, Name = "Rex" } });
        _adopters.Setup(s => s.GetAll()).Returns(new List<Adopter> { new Adopter { Id = adopterId, FullName = "Ana", Email = "a@a.com" } });

        var model = new AdoptionApplication { DogId = dogId, AdopterId = adopterId };

        var res = _c.Create(model);

        Assert.That(res, Is.TypeOf<ViewResult>());
        Assert.That(_c.ViewData["DogId"], Is.TypeOf<SelectList>());
        Assert.That(_c.ViewData["AdopterId"], Is.TypeOf<SelectList>());
        _apps.Verify(s => s.Add(It.IsAny<AdoptionApplication>()), Times.Never);
    }

    [Test]
    public void CreatePost_Valid_Adds_GeneratesId_AndRedirects()
    {
        var model = new AdoptionApplication
        {
            Id = Guid.Empty,
            DogId = Guid.NewGuid(),
            AdopterId = Guid.NewGuid(),
            Status = "Pending"
        };

        _apps.Setup(s => s.Add(It.IsAny<AdoptionApplication>())).Returns((AdoptionApplication x) => x);

        var res = _c.Create(model);

        Assert.That(model.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(res, Is.TypeOf<RedirectToActionResult>());
        _apps.Verify(s => s.Add(It.IsAny<AdoptionApplication>()), Times.Once);
    }

    [Test]
    public void EditGet_NullId_NotFound()
        => Assert.That(_c.Edit(null), Is.TypeOf<NotFoundResult>());

    [Test]
    public void EditGet_NotFound_NotFound()
    {
        _apps.Setup(s => s.GetById(It.IsAny<Guid>())).Returns((AdoptionApplication?)null);
        Assert.That(_c.Edit(Guid.NewGuid()), Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public void EditGet_Found_View_AndSetsSelectLists()
    {
        var dogId = Guid.NewGuid();
        var adopterId = Guid.NewGuid();

        var app = new AdoptionApplication { Id = Guid.NewGuid(), DogId = dogId, AdopterId = adopterId };

        _apps.Setup(s => s.GetById(It.IsAny<Guid>())).Returns(app);
        _dogs.Setup(s => s.GetAll()).Returns(new List<Dog> { new Dog { Id = dogId, Name = "Rex" } });
        _adopters.Setup(s => s.GetAll()).Returns(new List<Adopter> { new Adopter { Id = adopterId, FullName = "Ana", Email = "a@a.com" } });

        var res = _c.Edit(Guid.NewGuid());

        Assert.That(res, Is.TypeOf<ViewResult>());
        Assert.That(_c.ViewData["DogId"], Is.TypeOf<SelectList>());
        Assert.That(_c.ViewData["AdopterId"], Is.TypeOf<SelectList>());
    }

    [Test]
    public void EditPost_IdMismatch_NotFound()
    {
        var id = Guid.NewGuid();
        var model = new AdoptionApplication { Id = Guid.NewGuid() };

        Assert.That(_c.Edit(id, model), Is.TypeOf<NotFoundResult>());
        _apps.Verify(s => s.Update(It.IsAny<AdoptionApplication>()), Times.Never);
    }

    [Test]
    public void EditPost_InvalidModel_ReturnsView_AndSetsSelectLists()
    {
        _c.ModelState.AddModelError("x", "y");

        var id = Guid.NewGuid();
        var dogId = Guid.NewGuid();
        var adopterId = Guid.NewGuid();

        _dogs.Setup(s => s.GetAll()).Returns(new List<Dog> { new Dog { Id = dogId, Name = "Rex" } });
        _adopters.Setup(s => s.GetAll()).Returns(new List<Adopter> { new Adopter { Id = adopterId, FullName = "Ana", Email = "a@a.com" } });

        var model = new AdoptionApplication { Id = id, DogId = dogId, AdopterId = adopterId };

        var res = _c.Edit(id, model);

        Assert.That(res, Is.TypeOf<ViewResult>());
        Assert.That(_c.ViewData["DogId"], Is.TypeOf<SelectList>());
        Assert.That(_c.ViewData["AdopterId"], Is.TypeOf<SelectList>());
        _apps.Verify(s => s.Update(It.IsAny<AdoptionApplication>()), Times.Never);
    }

    [Test]
    public void EditPost_Valid_Updates_AndRedirects()
    {
        var id = Guid.NewGuid();
        var model = new AdoptionApplication { Id = id, DogId = Guid.NewGuid(), AdopterId = Guid.NewGuid(), Status = "Approved" };

        _apps.Setup(s => s.Update(It.IsAny<AdoptionApplication>())).Returns((AdoptionApplication x) => x);

        var res = _c.Edit(id, model);

        Assert.That(res, Is.TypeOf<RedirectToActionResult>());
        _apps.Verify(s => s.Update(It.IsAny<AdoptionApplication>()), Times.Once);
    }

    [Test]
    public void EditPost_UpdateThrows_AndApplicationMissing_ReturnsNotFound()
    {
        var id = Guid.NewGuid();
        var model = new AdoptionApplication { Id = id, DogId = Guid.NewGuid(), AdopterId = Guid.NewGuid() };

        _apps.Setup(s => s.Update(It.IsAny<AdoptionApplication>())).Throws(new Exception("fail"));

        // ApplicationExists -> GetById(id) == null
        _apps.Setup(s => s.GetById(id)).Returns((AdoptionApplication?)null);

        var res = _c.Edit(id, model);

        Assert.That(res, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public void EditPost_UpdateThrows_AndApplicationExists_Throws()
    {
        var id = Guid.NewGuid();
        var model = new AdoptionApplication { Id = id, DogId = Guid.NewGuid(), AdopterId = Guid.NewGuid() };

        _apps.Setup(s => s.Update(It.IsAny<AdoptionApplication>())).Throws(new Exception("fail"));

        // ApplicationExists -> GetById(id) != null
        _apps.Setup(s => s.GetById(id)).Returns(new AdoptionApplication { Id = id });

        Assert.Throws<Exception>(() => _c.Edit(id, model));
    }

    [Test]
    public void DeleteGet_NullId_NotFound()
        => Assert.That(_c.Delete(null), Is.TypeOf<NotFoundResult>());

    [Test]
    public void DeleteGet_NotFound_NotFound()
    {
        _apps.Setup(s => s.GetById(It.IsAny<Guid>())).Returns((AdoptionApplication?)null);
        Assert.That(_c.Delete(Guid.NewGuid()), Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public void DeleteGet_Found_View()
    {
        _apps.Setup(s => s.GetById(It.IsAny<Guid>())).Returns(new AdoptionApplication { Id = Guid.NewGuid() });
        Assert.That(_c.Delete(Guid.NewGuid()), Is.TypeOf<ViewResult>());
    }

    [Test]
    public void DeleteConfirmed_AppNull_Redirects_WithoutDelete()
    {
        _apps.Setup(s => s.GetById(It.IsAny<Guid>())).Returns((AdoptionApplication?)null);

        var res = _c.DeleteConfirmed(Guid.NewGuid());

        Assert.That(res, Is.TypeOf<RedirectToActionResult>());
        _apps.Verify(s => s.DeleteById(It.IsAny<Guid>()), Times.Never);
    }

    [Test]
    public void DeleteConfirmed_Found_Deletes_AndRedirects()
    {
        var id = Guid.NewGuid();
        _apps.Setup(s => s.GetById(id)).Returns(new AdoptionApplication { Id = id });
        _apps.Setup(s => s.DeleteById(id)).Returns(new AdoptionApplication { Id = id });

        var res = _c.DeleteConfirmed(id);

        Assert.That(res, Is.TypeOf<RedirectToActionResult>());
        _apps.Verify(s => s.DeleteById(id), Times.Once);
    }
}

#pragma warning restore NUnit103
