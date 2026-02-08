using System;
using System.Collections.Generic;
using System.Linq;
using DopAdoption.Domain.DomainModels;
using DopAdoption.Repository;
using DopAdoption.Service.Implementation;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using NUnit.Framework;

namespace DogAdoption.UnitTests.Services;

[TestFixture]
public class AdoptionApplicationsServiceTests
{
    private Mock<IRepository<AdoptionApplication>> _repo = null!;
    private AdoptionApplicationsService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IRepository<AdoptionApplication>>();
        _service = new AdoptionApplicationsService(_repo.Object);
    }

    [Test]
    public void Add_CallsInsert_AndReturnsInserted()
    {
        var app = new AdoptionApplication { Id = Guid.NewGuid() };
        _repo.Setup(r => r.Insert(It.IsAny<AdoptionApplication>()))
             .Returns((AdoptionApplication x) => x);

        var res = _service.Add(app);

        Assert.That(res, Is.SameAs(app));
        _repo.Verify(r => r.Insert(app), Times.Once);
    }

    [Test]
    public void Update_CallsUpdate_AndReturnsUpdated()
    {
        var app = new AdoptionApplication { Id = Guid.NewGuid() };
        _repo.Setup(r => r.Update(It.IsAny<AdoptionApplication>()))
             .Returns((AdoptionApplication x) => x);

        var res = _service.Update(app);

        Assert.That(res, Is.SameAs(app));
        _repo.Verify(r => r.Update(app), Times.Once);
    }

    [Test]
    public void InsertMany_CallsInsertMany_AndReturnsSameCollection()
    {
        var list = new List<AdoptionApplication>
        {
            new AdoptionApplication { Id = Guid.NewGuid() },
            new AdoptionApplication { Id = Guid.NewGuid() }
        };

        _repo.Setup(r => r.InsertMany(It.IsAny<ICollection<AdoptionApplication>>()))
             .Returns((ICollection<AdoptionApplication> x) => x);

        var res = _service.InsertMany(list);

        Assert.That(res, Is.SameAs(list));
        _repo.Verify(r => r.InsertMany(list), Times.Once);
    }

    [Test]
    public void GetAll_CallsGetAll_AndReturnsList()
    {
        var apps = new List<AdoptionApplication>
        {
            new AdoptionApplication { Id = Guid.NewGuid() },
            new AdoptionApplication { Id = Guid.NewGuid() }
        };

        _repo.Setup(r => r.GetAll(
                It.IsAny<System.Linq.Expressions.Expression<Func<AdoptionApplication, AdoptionApplication>>>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<AdoptionApplication, bool>>>(),
                It.IsAny<Func<IQueryable<AdoptionApplication>, IOrderedQueryable<AdoptionApplication>>>(),
                It.IsAny<Func<IQueryable<AdoptionApplication>, IIncludableQueryable<AdoptionApplication, object>>>()
            ))
            .Returns(apps);

        var res = _service.GetAll();

        Assert.That(res, Has.Count.EqualTo(2));
    }

    [Test]
    public void GetById_CallsGet_AndReturnsApplication()
    {
        var id = Guid.NewGuid();
        var app = new AdoptionApplication { Id = id };

        _repo.Setup(r => r.Get(
                It.IsAny<System.Linq.Expressions.Expression<Func<AdoptionApplication, AdoptionApplication>>>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<AdoptionApplication, bool>>>(),
                It.IsAny<Func<IQueryable<AdoptionApplication>, IOrderedQueryable<AdoptionApplication>>>(),
                It.IsAny<Func<IQueryable<AdoptionApplication>, IIncludableQueryable<AdoptionApplication, object>>>()
            ))
            .Returns(app);

        var res = _service.GetById(id);

        Assert.That(res, Is.SameAs(app));
        _repo.Verify(r => r.Get(
            It.IsAny<System.Linq.Expressions.Expression<Func<AdoptionApplication, AdoptionApplication>>>(),
            It.IsAny<System.Linq.Expressions.Expression<Func<AdoptionApplication, bool>>>(),
            It.IsAny<Func<IQueryable<AdoptionApplication>, IOrderedQueryable<AdoptionApplication>>>(),
            It.IsAny<Func<IQueryable<AdoptionApplication>, IIncludableQueryable<AdoptionApplication, object>>>()),
            Times.Once);
    }

    [Test]
    public void DeleteById_GetsThenDeletes_AndReturnsDeleted()
    {
        var id = Guid.NewGuid();
        var app = new AdoptionApplication { Id = id };

        _repo.Setup(r => r.Get(
                It.IsAny<System.Linq.Expressions.Expression<Func<AdoptionApplication, AdoptionApplication>>>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<AdoptionApplication, bool>>>(),
                It.IsAny<Func<IQueryable<AdoptionApplication>, IOrderedQueryable<AdoptionApplication>>>(),
                It.IsAny<Func<IQueryable<AdoptionApplication>, IIncludableQueryable<AdoptionApplication, object>>>()
            ))
            .Returns(app);

        _repo.Setup(r => r.Delete(It.IsAny<AdoptionApplication>()))
             .Returns((AdoptionApplication x) => x);

        var res = _service.DeleteById(id);

        Assert.That(res, Is.SameAs(app));
        _repo.Verify(r => r.Delete(app), Times.Once);
    }
}
