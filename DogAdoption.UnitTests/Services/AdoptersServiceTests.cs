using System;
using System.Collections.Generic;
using System.Linq;
using DopAdoption.Domain.DomainModels;
using DopAdoption.Repository;
using DopAdoption.Service.Implementation;
using Moq;
using NUnit.Framework;

namespace DogAdoption.UnitTests.Services;

[TestFixture]
public class AdoptersServiceTests
{
    private Mock<IRepository<Adopter>> _repo = null!;
    private AdoptersService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IRepository<Adopter>>();
        _service = new AdoptersService(_repo.Object);
    }

    [Test]
    public void Add_CallsInsert_AndReturnsInserted()
    {
        var a = new Adopter { Id = Guid.NewGuid(), FullName = "Ana A", Email = "a@a.com", Phone = "070" };
        _repo.Setup(r => r.Insert(It.IsAny<Adopter>())).Returns((Adopter x) => x);

        var res = _service.Add(a);

        Assert.That(res, Is.SameAs(a));
        _repo.Verify(r => r.Insert(a), Times.Once);
    }

    [Test]
    public void Update_CallsUpdate_AndReturnsUpdated()
    {
        var a = new Adopter { Id = Guid.NewGuid(), FullName = "Ana B", Email = "b@b.com" };
        _repo.Setup(r => r.Update(It.IsAny<Adopter>())).Returns((Adopter x) => x);

        var res = _service.Update(a);

        Assert.That(res, Is.SameAs(a));
        _repo.Verify(r => r.Update(a), Times.Once);
    }

    [Test]
    public void InsertMany_CallsInsertMany_AndReturnsSameCollection()
    {
        var list = new List<Adopter>
        {
            new Adopter { Id = Guid.NewGuid(), FullName = "A", Email = "a@a.com" },
            new Adopter { Id = Guid.NewGuid(), FullName = "B", Email = "b@b.com" }
        };

        _repo.Setup(r => r.InsertMany(It.IsAny<ICollection<Adopter>>()))
            .Returns((ICollection<Adopter> x) => x);

        var res = _service.InsertMany(list);

        Assert.That(res, Is.SameAs(list));
        _repo.Verify(r => r.InsertMany(list), Times.Once);
    }

    [Test]
    public void GetAll_CallsGetAll_AndReturnsToList()
    {
        var adopters = new List<Adopter>
        {
            new Adopter { Id = Guid.NewGuid(), FullName = "A", Email = "a@a.com" },
            new Adopter { Id = Guid.NewGuid(), FullName = "B", Email = "b@b.com" }
        };

        _repo.Setup(r => r.GetAll(
                It.IsAny<System.Linq.Expressions.Expression<Func<Adopter, Adopter>>>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Adopter, bool>>>(),
                It.IsAny<Func<IQueryable<Adopter>, IOrderedQueryable<Adopter>>>(),
                It.IsAny<Func<IQueryable<Adopter>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Adopter, object>>>()
            ))
            .Returns(adopters);

        var res = _service.GetAll();

        Assert.That(res, Has.Count.EqualTo(2));
        Assert.That(res[0].FullName, Is.EqualTo("A"));
    }

    [Test]
    public void GetById_CallsGet_AndReturnsAdopter()
    {
        var id = Guid.NewGuid();
        var adopter = new Adopter { Id = id, FullName = "Ana C", Email = "c@c.com" };

        _repo.Setup(r => r.Get(
                It.IsAny<System.Linq.Expressions.Expression<Func<Adopter, Adopter>>>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Adopter, bool>>>(),
                It.IsAny<Func<IQueryable<Adopter>, IOrderedQueryable<Adopter>>>(),
                It.IsAny<Func<IQueryable<Adopter>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Adopter, object>>>()
            ))
            .Returns(adopter);

        var res = _service.GetById(id);

        Assert.That(res, Is.SameAs(adopter));
        _repo.Verify(r => r.Get(
            It.IsAny<System.Linq.Expressions.Expression<Func<Adopter, Adopter>>>(),
            It.IsAny<System.Linq.Expressions.Expression<Func<Adopter, bool>>>(),
            It.IsAny<Func<IQueryable<Adopter>, IOrderedQueryable<Adopter>>>(),
            It.IsAny<Func<IQueryable<Adopter>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Adopter, object>>>()),
            Times.Once);
    }

    [Test]
    public void DeleteById_GetsThenDeletes_AndReturnsDeleted()
    {
        var id = Guid.NewGuid();
        var adopter = new Adopter { Id = id, FullName = "X", Email = "x@x.com" };

        _repo.Setup(r => r.Get(
                It.IsAny<System.Linq.Expressions.Expression<Func<Adopter, Adopter>>>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Adopter, bool>>>(),
                It.IsAny<Func<IQueryable<Adopter>, IOrderedQueryable<Adopter>>>(),
                It.IsAny<Func<IQueryable<Adopter>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Adopter, object>>>()
            ))
            .Returns(adopter);

        _repo.Setup(r => r.Delete(It.IsAny<Adopter>())).Returns((Adopter x) => x);

        var res = _service.DeleteById(id);

        Assert.That(res, Is.SameAs(adopter));
        _repo.Verify(r => r.Delete(adopter), Times.Once);
    }
}
