#pragma warning disable NUnit103

using System;
using System.Collections.Generic;
using DopAdoption.Domain.DomainModels;
using DopAdoption.Service.Interface;
using DopAdoption.Web.Controllers;
using DogAdoption.UnitTests.TestHelpers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace DogAdoption.UnitTests.Controllers;

[TestFixture]
public class BreedsControllerTests
{
    private Mock<IBreedsService> _breeds = null!;
    private BreedsController _c = null!;

    [SetUp]
    public void SetUp()
    {
        _breeds = new Mock<IBreedsService>();
        _c = new BreedsController(_breeds.Object);
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
        _breeds.Setup(s => s.GetAll()).Returns(new List<Breed>());
        Assert.That(_c.Index(), Is.TypeOf<ViewResult>());
    }

    [Test]
    public void Details_NullId_NotFound()
        => Assert.That(_c.Details(null), Is.TypeOf<NotFoundResult>());

    [Test]
    public void Details_NotFound_NotFound()
    {
        _breeds.Setup(s => s.GetById(It.IsAny<Guid>())).Returns((Breed?)null);
        Assert.That(_c.Details(Guid.NewGuid()), Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public void Details_Found_View()
    {
        var b = new Breed { Name = "Labrador" };
        _breeds.Setup(s => s.GetById(It.IsAny<Guid>())).Returns(b);

        var res = _c.Details(Guid.NewGuid()) as ViewResult;

        Assert.That(res, Is.Not.Null);
        Assert.That(res!.Model, Is.SameAs(b));
    }

    [Test]
    public void CreateGet_ReturnsView()
        => Assert.That(_c.Create(), Is.TypeOf<ViewResult>());

    [Test]
    public void CreatePost_InvalidModel_ReturnsView_AndDoesNotAdd()
    {
        _c.ModelState.AddModelError("Name", "err");

        var model = new Breed { Name = "" };
        var res = _c.Create(model);

        Assert.That(res, Is.TypeOf<ViewResult>());
        _breeds.Verify(s => s.Add(It.IsAny<Breed>()), Times.Never);
    }

    [Test]
    public void CreatePost_Valid_WithEmptyGuid_GeneratesId_AndRedirects()
    {
        var model = new Breed { Id = Guid.Empty, Name = "Beagle" };
        _breeds.Setup(s => s.Add(It.IsAny<Breed>())).Returns((Breed x) => x);

        var res = _c.Create(model);

        Assert.That(model.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(res, Is.TypeOf<RedirectToActionResult>());
        Assert.That(_c.TempData["Success"]?.ToString(), Does.Contain("created"));
        _breeds.Verify(s => s.Add(It.IsAny<Breed>()), Times.Once);
    }

    [Test]
    public void CreatePost_Valid_WithExistingGuid_Redirects()
    {
        var model = new Breed { Id = Guid.NewGuid(), Name = "Husky" };
        _breeds.Setup(s => s.Add(It.IsAny<Breed>())).Returns((Breed x) => x);

        var res = _c.Create(model);

        Assert.That(res, Is.TypeOf<RedirectToActionResult>());
        _breeds.Verify(s => s.Add(It.IsAny<Breed>()), Times.Once);
    }

    [Test]
    public void EditGet_NullId_NotFound()
        => Assert.That(_c.Edit(null), Is.TypeOf<NotFoundResult>());

    [Test]
    public void EditGet_NotFound_NotFound()
    {
        _breeds.Setup(s => s.GetById(It.IsAny<Guid>())).Returns((Breed?)null);
        Assert.That(_c.Edit(Guid.NewGuid()), Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public void EditGet_Found_View()
    {
        var b = new Breed { Id = Guid.NewGuid(), Name = "A" };
        _breeds.Setup(s => s.GetById(It.IsAny<Guid>())).Returns(b);

        var res = _c.Edit(Guid.NewGuid());

        Assert.That(res, Is.TypeOf<ViewResult>());
    }

    [Test]
    public void EditPost_IdMismatch_NotFound()
    {
        var id = Guid.NewGuid();
        var model = new Breed { Id = Guid.NewGuid(), Name = "X" };

        Assert.That(_c.Edit(id, model), Is.TypeOf<NotFoundResult>());
        _breeds.Verify(s => s.Update(It.IsAny<Breed>()), Times.Never);
    }

    [Test]
    public void EditPost_InvalidModel_ReturnsView_AndDoesNotUpdate()
    {
        var id = Guid.NewGuid();
        var model = new Breed { Id = id, Name = "" };
        _c.ModelState.AddModelError("Name", "err");

        var res = _c.Edit(id, model);

        Assert.That(res, Is.TypeOf<ViewResult>());
        _breeds.Verify(s => s.Update(It.IsAny<Breed>()), Times.Never);
    }

    [Test]
    public void EditPost_Valid_Updates_AndRedirects()
    {
        var id = Guid.NewGuid();
        var model = new Breed { Id = id, Name = "Updated" };
        _breeds.Setup(s => s.Update(It.IsAny<Breed>())).Returns((Breed x) => x);

        var res = _c.Edit(id, model);

        Assert.That(res, Is.TypeOf<RedirectToActionResult>());
        Assert.That(_c.TempData["Success"]?.ToString(), Does.Contain("updated"));
        _breeds.Verify(s => s.Update(It.IsAny<Breed>()), Times.Once);
    }

    [Test]
    public void DeleteGet_NullId_NotFound()
        => Assert.That(_c.Delete(null), Is.TypeOf<NotFoundResult>());

    [Test]
    public void DeleteGet_NotFound_NotFound()
    {
        _breeds.Setup(s => s.GetById(It.IsAny<Guid>())).Returns((Breed?)null);
        Assert.That(_c.Delete(Guid.NewGuid()), Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public void DeleteGet_Found_View()
    {
        var b = new Breed { Id = Guid.NewGuid(), Name = "ToDelete" };
        _breeds.Setup(s => s.GetById(It.IsAny<Guid>())).Returns(b);

        Assert.That(_c.Delete(Guid.NewGuid()), Is.TypeOf<ViewResult>());
    }

    [Test]
    public void DeleteConfirmed_BreedNull_Redirects_WithoutDelete()
    {
        _breeds.Setup(s => s.GetById(It.IsAny<Guid>())).Returns((Breed?)null);

        var res = _c.DeleteConfirmed(Guid.NewGuid());

        Assert.That(res, Is.TypeOf<RedirectToActionResult>());
        _breeds.Verify(s => s.DeleteById(It.IsAny<Guid>()), Times.Never);
    }

    [Test]
    public void DeleteConfirmed_Found_Deletes_SetsTempData_AndRedirects()
    {
        var id = Guid.NewGuid();
        var b = new Breed { Id = id, Name = "B" };
        _breeds.Setup(s => s.GetById(id)).Returns(b);
        _breeds.Setup(s => s.DeleteById(id)).Returns(b);

        var res = _c.DeleteConfirmed(id);

        Assert.That(res, Is.TypeOf<RedirectToActionResult>());
        Assert.That(_c.TempData["Success"]?.ToString(), Does.Contain("deleted"));
        _breeds.Verify(s => s.DeleteById(id), Times.Once);
    }
}

#pragma warning restore NUnit103
