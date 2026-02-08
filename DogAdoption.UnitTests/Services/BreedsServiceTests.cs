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
public class BreedsServiceTests
{
    private Mock<IRepository<Breed>> _repo = null!;
    private BreedsService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IRepository<Breed>>();
        _service = new BreedsService(_repo.Object);
    }

    [Test]
    public void Add_CallsInsert_AndReturnsInserted()
    {
        var b = new Breed { Id = Guid.NewGuid(), Name = "Labrador" };
        _repo.Setup(r => r.Insert(It.IsAny<Breed>())).Returns((Breed x) => x);

        var res = _service.Add(b);

        Assert.That(res, Is.SameAs(b));
        _repo.Verify(r => r.Insert(b), Times.Once);
    }

    [Test]
    public void Update_CallsUpdate_AndReturnsUpdated()
    {
        var b = new Breed { Id = Guid.NewGuid(), Name = "Husky" };
        _repo.Setup(r => r.Update(It.IsAny<Breed>())).Returns((Breed x) => x);

        var res = _service.Update(b);

        Assert.That(res, Is.SameAs(b));
        _repo.Verify(r => r.Update(b), Times.Once);
    }

    [Test]
    public void InsertMany_CallsInsertMany_AndReturnsSameCollection()
    {
        var list = new List<Breed>
        {
            new Breed { Id = Guid.NewGuid(), Name = "A" },
            new Breed { Id = Guid.NewGuid(), Name = "B" }
        };

        _repo.Setup(r => r.InsertMany(It.IsAny<ICollection<Breed>>()))
            .Returns((ICollection<Breed> x) => x);

        var res = _service.InsertMany(list);

        Assert.That(res, Is.SameAs(list));
        _repo.Verify(r => r.InsertMany(list), Times.Once);
    }

    [Test]
    public void GetAll_CallsGetAll_AndReturnsToList()
    {
        var breeds = new List<Breed>
        {
            new Breed { Id = Guid.NewGuid(), Name = "A" },
            new Breed { Id = Guid.NewGuid(), Name = "B" }
        };

        _repo.Setup(r => r.GetAll(
                It.IsAny<System.Linq.Expressions.Expression<Func<Breed, Breed>>>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Breed, bool>>>(),
                It.IsAny<Func<IQueryable<Breed>, IOrderedQueryable<Breed>>>(),
                It.IsAny<Func<IQueryable<Breed>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Breed, object>>>()
            ))
            .Returns(breeds);

        var res = _service.GetAll();

        Assert.That(res, Has.Count.EqualTo(2));
        Assert.That(res[0].Name, Is.EqualTo("A"));
    }

    [Test]
    public void GetById_CallsGet_AndReturnsBreed()
    {
        var id = Guid.NewGuid();
        var breed = new Breed { Id = id, Name = "Retriever" };

        _repo.Setup(r => r.Get(
                It.IsAny<System.Linq.Expressions.Expression<Func<Breed, Breed>>>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Breed, bool>>>(),
                It.IsAny<Func<IQueryable<Breed>, IOrderedQueryable<Breed>>>(),
                It.IsAny<Func<IQueryable<Breed>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Breed, object>>>()
            ))
            .Returns(breed);

        var res = _service.GetById(id);

        Assert.That(res, Is.SameAs(breed));
        _repo.Verify(r => r.Get(
            It.IsAny<System.Linq.Expressions.Expression<Func<Breed, Breed>>>(),
            It.IsAny<System.Linq.Expressions.Expression<Func<Breed, bool>>>(),
            It.IsAny<Func<IQueryable<Breed>, IOrderedQueryable<Breed>>>(),
            It.IsAny<Func<IQueryable<Breed>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Breed, object>>>()),
            Times.Once);
    }

    [Test]
    public void DeleteById_GetsThenDeletes_AndReturnsDeleted()
    {
        var id = Guid.NewGuid();
        var breed = new Breed { Id = id, Name = "X" };

        _repo.Setup(r => r.Get(
                It.IsAny<System.Linq.Expressions.Expression<Func<Breed, Breed>>>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Breed, bool>>>(),
                It.IsAny<Func<IQueryable<Breed>, IOrderedQueryable<Breed>>>(),
                It.IsAny<Func<IQueryable<Breed>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Breed, object>>>()
            ))
            .Returns(breed);

        _repo.Setup(r => r.Delete(It.IsAny<Breed>())).Returns((Breed x) => x);

        var res = _service.DeleteById(id);

        Assert.That(res, Is.SameAs(breed));
        _repo.Verify(r => r.Delete(breed), Times.Once);
    }
}
