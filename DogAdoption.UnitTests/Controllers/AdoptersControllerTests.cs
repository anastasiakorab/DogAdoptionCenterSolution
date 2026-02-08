#pragma warning disable NUnit103

using System;
using System.Collections.Generic;
using DopAdoption.Domain.DomainModels;
using DopAdoption.Service.Interface;
using DopAdoption.Web.Controllers;
using DogAdoption.UnitTests.TestHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace DogAdoption.UnitTests.Controllers;

[TestFixture]
public class AdoptersControllerTests
{
    private Mock<IAdoptersService> _adopters = null!;
    private AdoptersController _c = null!;

    [SetUp]
    public void SetUp()
    {
        _adopters = new Mock<IAdoptersService>();
        _c = new AdoptersController(_adopters.Object);
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
        _adopters.Setup(s => s.GetAll()).Returns(new List<Adopter>());
        Assert.That(_c.Index(), Is.TypeOf<ViewResult>());
    }

    [Test]
    public void Details_NullId_NotFound()
        => Assert.That(_c.Details(null), Is.TypeOf<NotFoundResult>());

    [Test]
    public void Details_NotFound_NotFound()
    {
        _adopters.Setup(s => s.GetById(It.IsAny<Guid>())).Returns((Adopter?)null);
        Assert.That(_c.Details(Guid.NewGuid()), Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public void Details_Found_View()
    {
        var a = new Adopter { FullName = "Ana", Email = "a@a.com" };
        _adopters.Setup(s => s.GetById(It.IsAny<Guid>())).Returns(a);

        var res = _c.Details(Guid.NewGuid()) as ViewResult;

        Assert.That(res, Is.Not.Null);
        Assert.That(res!.Model, Is.SameAs(a));
    }

    [Test]
    public void CreateGet_ReturnsView()
        => Assert.That(_c.Create(), Is.TypeOf<ViewResult>());

    [Test]
    public void CreatePost_InvalidModel_ReturnsView_AndDoesNotAdd()
    {
        _c.ModelState.AddModelError("FullName", "err");

        var model = new Adopter { FullName = "", Email = "x@x.com" };
        var res = _c.Create(model);

        Assert.That(res, Is.TypeOf<ViewResult>());
        _adopters.Verify(s => s.Add(It.IsAny<Adopter>()), Times.Never);
    }

    [Test]
    public void CreatePost_Valid_GeneratesId_Adds_AndRedirects()
    {
        var model = new Adopter { Id = Guid.Empty, FullName = "B", Email = "b@b.com" };
        _adopters.Setup(s => s.Add(It.IsAny<Adopter>())).Returns((Adopter x) => x);

        var res = _c.Create(model);

        Assert.That(model.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(res, Is.TypeOf<RedirectToActionResult>());
        _adopters.Verify(s => s.Add(It.IsAny<Adopter>()), Times.Once);
    }

    [Test]
    public void EditGet_NullId_NotFound()
        => Assert.That(_c.Edit(null), Is.TypeOf<NotFoundResult>());

    [Test]
    public void EditGet_NotFound_NotFound()
    {
        _adopters.Setup(s => s.GetById(It.IsAny<Guid>())).Returns((Adopter?)null);
        Assert.That(_c.Edit(Guid.NewGuid()), Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public void EditGet_Found_View()
    {
        var a = new Adopter { Id = Guid.NewGuid(), FullName = "A", Email = "a@a.com" };
        _adopters.Setup(s => s.GetById(It.IsAny<Guid>())).Returns(a);

        Assert.That(_c.Edit(Guid.NewGuid()), Is.TypeOf<ViewResult>());
    }

    [Test]
    public void EditPost_IdMismatch_NotFound()
    {
        var id = Guid.NewGuid();
        var model = new Adopter { Id = Guid.NewGuid(), FullName = "X", Email = "x@x.com" };

        Assert.That(_c.Edit(id, model), Is.TypeOf<NotFoundResult>());
        _adopters.Verify(s => s.Update(It.IsAny<Adopter>()), Times.Never);
    }

    [Test]
    public void EditPost_InvalidModel_ReturnsView_AndDoesNotUpdate()
    {
        var id = Guid.NewGuid();
        var model = new Adopter { Id = id, FullName = "", Email = "x@x.com" };
        _c.ModelState.AddModelError("FullName", "err");

        var res = _c.Edit(id, model);

        Assert.That(res, Is.TypeOf<ViewResult>());
        _adopters.Verify(s => s.Update(It.IsAny<Adopter>()), Times.Never);
    }

    [Test]
    public void EditPost_Valid_Updates_AndRedirects()
    {
        var id = Guid.NewGuid();
        var model = new Adopter { Id = id, FullName = "Upd", Email = "u@u.com" };
        _adopters.Setup(s => s.Update(It.IsAny<Adopter>())).Returns((Adopter x) => x);

        var res = _c.Edit(id, model);

        Assert.That(res, Is.TypeOf<RedirectToActionResult>());
        _adopters.Verify(s => s.Update(It.IsAny<Adopter>()), Times.Once);
    }

    [Test]
    public void EditPost_ConcurrencyException_AndAdopterMissing_ReturnsNotFound()
    {
        var id = Guid.NewGuid();
        var model = new Adopter { Id = id, FullName = "A", Email = "a@a.com" };

        _adopters.Setup(s => s.Update(It.IsAny<Adopter>()))
                 .Throws(new DbUpdateConcurrencyException());

        
        _adopters.Setup(s => s.GetById(id)).Returns((Adopter?)null);

        var res = _c.Edit(id, model);

        Assert.That(res, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public void EditPost_ConcurrencyException_AndAdopterExists_Throws()
    {
        var id = Guid.NewGuid();
        var model = new Adopter { Id = id, FullName = "A", Email = "a@a.com" };

        _adopters.Setup(s => s.Update(It.IsAny<Adopter>()))
                 .Throws(new DbUpdateConcurrencyException());

        
        _adopters.Setup(s => s.GetById(id)).Returns(new Adopter { Id = id, FullName = "X", Email = "x@x.com" });

        Assert.Throws<DbUpdateConcurrencyException>(() => _c.Edit(id, model));
    }

    [Test]
    public void DeleteGet_NullId_NotFound()
        => Assert.That(_c.Delete(null), Is.TypeOf<NotFoundResult>());

    [Test]
    public void DeleteGet_NotFound_NotFound()
    {
        _adopters.Setup(s => s.GetById(It.IsAny<Guid>())).Returns((Adopter?)null);
        Assert.That(_c.Delete(Guid.NewGuid()), Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public void DeleteGet_Found_View()
    {
        var a = new Adopter { Id = Guid.NewGuid(), FullName = "Del", Email = "d@d.com" };
        _adopters.Setup(s => s.GetById(It.IsAny<Guid>())).Returns(a);

        Assert.That(_c.Delete(Guid.NewGuid()), Is.TypeOf<ViewResult>());
    }

    [Test]
    public void DeleteConfirmed_AdopterNull_Redirects_WithoutDelete()
    {
        _adopters.Setup(s => s.GetById(It.IsAny<Guid>())).Returns((Adopter?)null);

        var res = _c.DeleteConfirmed(Guid.NewGuid());

        Assert.That(res, Is.TypeOf<RedirectToActionResult>());
        _adopters.Verify(s => s.DeleteById(It.IsAny<Guid>()), Times.Never);
    }

    [Test]
    public void DeleteConfirmed_Found_Deletes_AndRedirects()
    {
        var id = Guid.NewGuid();
        _adopters.Setup(s => s.GetById(id))
                 .Returns(new Adopter { Id = id, FullName = "A", Email = "a@a.com" });

        _adopters.Setup(s => s.DeleteById(id))
                 .Returns(new Adopter { Id = id });

        var res = _c.DeleteConfirmed(id);

        Assert.That(res, Is.TypeOf<RedirectToActionResult>());
        _adopters.Verify(s => s.DeleteById(id), Times.Once);
    }
}

#pragma warning restore NUnit103
