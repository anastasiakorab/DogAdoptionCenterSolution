#pragma warning disable NUnit103

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DopAdoption.Domain.DomainModels;
using DopAdoption.Domain.DTOs;
using DopAdoption.Service.Interface;
using DopAdoption.Web.Controllers;
using DogAdoption.UnitTests.TestHelpers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace DogAdoption.UnitTests.Controllers;

[TestFixture]
public class DogsControllerTests
{
    private Mock<IDogsService> _dogs = null!;
    private Mock<IBreedsService> _breeds = null!;
    private Mock<IExternalDogService> _ext = null!;
    private DogsController _c = null!;

    [SetUp]
    public void SetUp()
    {
        _dogs = new Mock<IDogsService>();
        _breeds = new Mock<IBreedsService>();
        _ext = new Mock<IExternalDogService>();

        _c = new DogsController(_dogs.Object, _breeds.Object, _ext.Object);
        ControllerTestHelper.AttachTempData(_c);
    }

    [TearDown]
    public void TearDown()
    {
        if (_c is IDisposable d)
            d.Dispose();
    }

    [Test]
    public void Index_ReturnsView()
    {
        _dogs.Setup(s => s.GetAll()).Returns(new List<Dog>());
        Assert.That(_c.Index(), Is.TypeOf<ViewResult>());
    }

    [Test]
    public void Details_NullId_NotFound() // nema id vrakjam 404 
        => Assert.That(_c.Details(null), Is.TypeOf<NotFoundResult>());

    [Test]
    public void Details_NotFound_NotFound() // vnesuva id ama kuceto ne postoi
    {
        _dogs.Setup(s => s.GetById(It.IsAny<Guid>())).Returns((Dog?)null);
        Assert.That(_c.Details(Guid.NewGuid()), Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public void Details_Found_View()  // kuceto sto e vneseno postoi i se vrakja 
    {
        var dog = new Dog();
        _dogs.Setup(s => s.GetById(It.IsAny<Guid>())).Returns(dog);

        var res = _c.Details(Guid.NewGuid()) as ViewResult; //kontrolerot vrakja dog

        Assert.That(res, Is.Not.Null);
        Assert.That(res!.Model, Is.SameAs(dog)); // uspeva
    }

    [Test]
    public void CreateGet_ReturnsView() // kontr vrakja lista na rasi iako e prazna
    {
        _breeds.Setup(s => s.GetAll()).Returns(new List<Breed>());
        Assert.That(_c.Create(), Is.TypeOf<ViewResult>());
    }

    [Test]
    public void CreatePost_InvalidModel_ReturnsView()
    {
        _breeds.Setup(s => s.GetAll()).Returns(new List<Breed>());
        _c.ModelState.AddModelError("Name", "err");

        var res = _c.Create(new Dog { Name = "" });

        Assert.That(res, Is.TypeOf<ViewResult>());
        _dogs.Verify(s => s.Add(It.IsAny<Dog>()), Times.Never);
    }

    [Test]
    public void CreatePost_Valid_Redirects()
    {
        _dogs.Setup(s => s.Add(It.IsAny<Dog>())).Returns((Dog d) => d); // se vrkajat kuceto so e pobarano

        var res = _c.Create(new Dog { Name = "Rex", Age = 1 });

        Assert.That(res, Is.TypeOf<RedirectToActionResult>());
        _dogs.Verify(s => s.Add(It.IsAny<Dog>()), Times.Once);
    }

    [Test]
    public void EditGet_NullId_NotFound() // ako ne vnesam id 
        => Assert.That(_c.Edit(null), Is.TypeOf<NotFoundResult>());

    [Test]
    public void EditGet_NotFound_NotFound() // ako kuce so takvo id ne postojt
    {
        _dogs.Setup(s => s.GetById(It.IsAny<Guid>())).Returns((Dog?)null);
        Assert.That(_c.Edit(Guid.NewGuid()), Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public void EditGet_Found_View() // ako postojt takvo kuce
    {
        _breeds.Setup(s => s.GetAll()).Returns(new List<Breed>());
        _dogs.Setup(s => s.GetById(It.IsAny<Guid>())).Returns(new Dog());

        Assert.That(_c.Edit(Guid.NewGuid()), Is.TypeOf<ViewResult>());
    }

    [Test]
    public void EditPost_IdMismatch_NotFound()
    {
        Assert.That(_c.Edit(Guid.NewGuid(), new Dog { Id = Guid.NewGuid() }),
            Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public void EditPost_InvalidModel_ReturnsView()
    {
        _breeds.Setup(s => s.GetAll()).Returns(new List<Breed>());
        var id = Guid.NewGuid();
        _c.ModelState.AddModelError("x", "y");

        Assert.That(_c.Edit(id, new Dog { Id = id }), Is.TypeOf<ViewResult>());
    }

    [Test]
    public void EditPost_Valid_Redirects()
    {
        var id = Guid.NewGuid();
        _dogs.Setup(s => s.Update(It.IsAny<Dog>())).Returns((Dog d) => d);

        var res = _c.Edit(id, new Dog { Id = id, Name = "A" });

        Assert.That(res, Is.TypeOf<RedirectToActionResult>());
        _dogs.Verify(s => s.Update(It.IsAny<Dog>()), Times.Once);
    }

    [Test]
    public void DeleteGet_NullId_NotFound()
        => Assert.That(_c.Delete(null), Is.TypeOf<NotFoundResult>());

    [Test]
    public void DeleteGet_NotFound_NotFound()
    {
        _dogs.Setup(s => s.GetById(It.IsAny<Guid>())).Returns((Dog?)null);
        Assert.That(_c.Delete(Guid.NewGuid()), Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public void DeleteGet_Found_View()
    {
        _dogs.Setup(s => s.GetById(It.IsAny<Guid>())).Returns(new Dog());
        Assert.That(_c.Delete(Guid.NewGuid()), Is.TypeOf<ViewResult>());
    }

    [Test]
    public void DeleteConfirmed_DogNull_Redirects()
    {
        _dogs.Setup(s => s.GetById(It.IsAny<Guid>())).Returns((Dog?)null);
        Assert.That(_c.DeleteConfirmed(Guid.NewGuid()),
            Is.TypeOf<RedirectToActionResult>());
    }

    [Test]
    public void DeleteConfirmed_Success_SetsTempData()
    {
        var id = Guid.NewGuid();
        _dogs.Setup(s => s.GetById(id)).Returns(new Dog { Id = id, Name = "R" });
        _dogs.Setup(s => s.DeleteById(id)).Returns(new Dog());

        _ = _c.DeleteConfirmed(id);

        Assert.That(_c.TempData["Success"]?.ToString(), Does.Contain("deleted"));
    }

    [Test]
    public void DeleteConfirmed_Failure_SetsError()
    {
        var id = Guid.NewGuid();
        _dogs.Setup(s => s.GetById(id)).Returns(new Dog { Id = id, Name = "R" });
        _dogs.Setup(s => s.DeleteById(id)).Throws(new Exception("fail"));

        _ = _c.DeleteConfirmed(id);

        Assert.That(_c.TempData["Error"]?.ToString(),
            Is.EqualTo("Cannot delete this dog."));
    }

    [Test]
    public void MarkAsAdopted_Redirects()
    {
        _dogs.Setup(s => s.MarkAsAdopted(It.IsAny<Guid>())).Returns(new Dog());
        Assert.That(_c.MarkAsAdopted(Guid.NewGuid()),
            Is.TypeOf<RedirectToActionResult>());
    }

    [Test]
    public async Task ApiPing_Success_Ok()
    {
        _ext.Setup(s => s.GetRandomAsync()).ReturnsAsync(new DogApiDTO());
        Assert.That(await _c.ApiPing(), Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task ApiPing_Failure_500() // ako padne api/snema internet status code = 500
    {
        _ext.Setup(s => s.GetRandomAsync()).ThrowsAsync(new Exception("x"));

        var res = await _c.ApiPing() as ObjectResult;

        Assert.That(res, Is.Not.Null);
        Assert.That(res!.StatusCode, Is.EqualTo(500));
    }

    [Test]
    public async Task RandomPhoto_Success_SetsSuccess()
    {
        _ext.Setup(s => s.GetRandomAsync()).ReturnsAsync(
            new DogApiDTO { PhotoLink = "u", TypeOfDog = "A", DogVariety = "B", Source = "dog.ceo" });

        var res = await _c.RandomPhoto();

        Assert.That(res, Is.TypeOf<RedirectToActionResult>());
        Assert.That(_c.TempData["Success"]?.ToString(),
            Is.EqualTo("Random dog photo loaded from API."));
    }

    [Test]
    public async Task RandomPhoto_Failure_SetsError()
    {
        _ext.Setup(s => s.GetRandomAsync()).ThrowsAsync(new Exception("x"));

        var res = await _c.RandomPhoto();

        Assert.That(res, Is.TypeOf<RedirectToActionResult>());
        Assert.That(_c.TempData["Error"]?.ToString(),
            Is.EqualTo("Could not load photo from API."));
    }

    [Test]
    public async Task RandomPhotos_Success_ReturnsIndexView()
    {
        _dogs.Setup(s => s.GetAll()).Returns(new List<Dog>());
        _ext.Setup(s => s.GetRandomManyAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<DogApiDTO> { new DogApiDTO() });

        var res = await _c.RandomPhotos(2) as ViewResult;

        Assert.That(res, Is.Not.Null);
        Assert.That(res!.ViewName, Is.EqualTo("Index"));
    }

    [Test]
    public async Task RandomPhotos_Failure_SetsError_ReturnsIndexView()
    {
        _dogs.Setup(s => s.GetAll()).Returns(new List<Dog>());
        _ext.Setup(s => s.GetRandomManyAsync(It.IsAny<int>()))
            .ThrowsAsync(new Exception("x"));

        var res = await _c.RandomPhotos(2);

        Assert.That(res, Is.TypeOf<ViewResult>());
        Assert.That(_c.TempData["Error"]?.ToString(),
            Is.EqualTo("Could not load photos from API."));
    }
}

#pragma warning restore NUnit103
