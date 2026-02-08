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
public class DogsServiceTests
{
    private Mock<IRepository<Dog>> _repo = null!;
    private DogsService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IRepository<Dog>>();
        _service = new DogsService(_repo.Object);
    }

    [Test]
    public void Add_CallsInsert_AndReturnsInserted()
    {
        var d = new Dog { Id = Guid.NewGuid(), Name = "Rex", Age = 2, Status = "Available", Sex = "M" };
        _repo.Setup(r => r.Insert(It.IsAny<Dog>())).Returns((Dog x) => x);

        var res = _service.Add(d);

        Assert.That(res, Is.SameAs(d));
        _repo.Verify(r => r.Insert(d), Times.Once);
    }

    [Test]
    public void Update_CallsUpdate_AndReturnsUpdated()
    {
        var d = new Dog { Id = Guid.NewGuid(), Name = "A", Age = 1, Status = "Reserved", Sex = "F" };
        _repo.Setup(r => r.Update(It.IsAny<Dog>())).Returns((Dog x) => x);

        var res = _service.Update(d);

        Assert.That(res, Is.SameAs(d));
        _repo.Verify(r => r.Update(d), Times.Once);
    }

    [Test]
    public void InsertMany_CallsInsertMany_AndReturnsSameCollection()
    {
        var list = new List<Dog>
        {
            new Dog { Id = Guid.NewGuid(), Name = "A", Age = 1, Status = "Available", Sex = "M" },
            new Dog { Id = Guid.NewGuid(), Name = "B", Age = 2, Status = "Available", Sex = "F" }
        };

        _repo.Setup(r => r.InsertMany(It.IsAny<ICollection<Dog>>())).Returns((ICollection<Dog> x) => x);

        var res = _service.InsertMany(list);

        Assert.That(res, Is.SameAs(list));
        _repo.Verify(r => r.InsertMany(list), Times.Once);
    }

    [Test]
    public void GetAll_CallsGetAll_AndReturnsToList()
    {
        var dogs = new List<Dog>
        {
            new Dog { Id = Guid.NewGuid(), Name = "A", Age = 1, Status = "Available", Sex = "M" },
            new Dog { Id = Guid.NewGuid(), Name = "B", Age = 2, Status = "Reserved", Sex = "F" }
        };

        // selector: x=>x, ignore include/predicate
        _repo.Setup(r => r.GetAll(
                It.IsAny<System.Linq.Expressions.Expression<Func<Dog, Dog>>>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Dog, bool>>>(),
                It.IsAny<Func<IQueryable<Dog>, IOrderedQueryable<Dog>>>(),
                It.IsAny<Func<IQueryable<Dog>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Dog, object>>>()
            ))
            .Returns(dogs);

        var res = _service.GetAll();

        Assert.That(res, Has.Count.EqualTo(2));
        Assert.That(res[0].Name, Is.EqualTo("A"));
    }

    [Test]
    public void GetById_CallsGet_WithPredicate_AndReturnsDog()
    {
        var id = Guid.NewGuid();
        var dog = new Dog { Id = id, Name = "Rex", Age = 2, Status = "Available", Sex = "M" };

        _repo.Setup(r => r.Get(
                It.IsAny<System.Linq.Expressions.Expression<Func<Dog, Dog>>>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Dog, bool>>>(),
                It.IsAny<Func<IQueryable<Dog>, IOrderedQueryable<Dog>>>(),
                It.IsAny<Func<IQueryable<Dog>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Dog, object>>>()
            ))
            .Returns(dog);

        var res = _service.GetById(id);

        Assert.That(res, Is.SameAs(dog));
        _repo.Verify(r => r.Get(
            It.IsAny<System.Linq.Expressions.Expression<Func<Dog, Dog>>>(),
            It.IsAny<System.Linq.Expressions.Expression<Func<Dog, bool>>>(),
            It.IsAny<Func<IQueryable<Dog>, IOrderedQueryable<Dog>>>(),
            It.IsAny<Func<IQueryable<Dog>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Dog, object>>>()),
            Times.Once);
    }

    [Test]
    public void DeleteById_GetsThenDeletes_AndReturnsDeleted()
    {
        var id = Guid.NewGuid();
        var dog = new Dog { Id = id, Name = "X", Age = 3, Status = "Available", Sex = "M" };

        _repo.Setup(r => r.Get(
                It.IsAny<System.Linq.Expressions.Expression<Func<Dog, Dog>>>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Dog, bool>>>(),
                It.IsAny<Func<IQueryable<Dog>, IOrderedQueryable<Dog>>>(),
                It.IsAny<Func<IQueryable<Dog>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Dog, object>>>()
            ))
            .Returns(dog);

        _repo.Setup(r => r.Delete(It.IsAny<Dog>())).Returns((Dog x) => x);

        var res = _service.DeleteById(id);

        Assert.That(res, Is.SameAs(dog));
        _repo.Verify(r => r.Delete(dog), Times.Once);
    }

    [Test]
    public void MarkAsAdopted_SetsStatus_AndUpdates()
    {
        var id = Guid.NewGuid();
        var dog = new Dog { Id = id, Name = "R", Age = 1, Status = "Available", Sex = "M" };

        _repo.Setup(r => r.Get(
                It.IsAny<System.Linq.Expressions.Expression<Func<Dog, Dog>>>(),
                It.IsAny<System.Linq.Expressions.Expression<Func<Dog, bool>>>(),
                It.IsAny<Func<IQueryable<Dog>, IOrderedQueryable<Dog>>>(),
                It.IsAny<Func<IQueryable<Dog>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Dog, object>>>()
            ))
            .Returns(dog);

        _repo.Setup(r => r.Update(It.IsAny<Dog>())).Returns((Dog x) => x);

        var res = _service.MarkAsAdopted(id);

        Assert.That(dog.Status, Is.EqualTo("Adopted"));
        Assert.That(res, Is.SameAs(dog));
        _repo.Verify(r => r.Update(dog), Times.Once);
    }
}
