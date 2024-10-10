using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Moq;
using SwipetorApp.Models.DbEntities;

namespace SwipetorAppTest.TestLibs;

public class DbCxMocker
{
    private readonly Mock<DbCx> _mockContext = new();

    public DbCxMocker AddEntities<TE>(Expression<Func<DbCx, DbSet<TE>>> entityFn, List<TE> entities) where TE : class
    {
        var mockSet = CreateMockDbSet(entities);
        _mockContext.Setup(entityFn).Returns(mockSet.Object);
        return this;
    }

    public Mock<DbCx> GetMockContext()
    {
        return _mockContext;
    }

    private Mock<DbSet<T>> CreateMockDbSet<T>(List<T> elements) where T : class
    {
        var elementsAsQueryable = elements.AsQueryable();
        var mockSet = new Mock<DbSet<T>>();

        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(elementsAsQueryable.Provider);
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(elementsAsQueryable.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(elementsAsQueryable.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => elementsAsQueryable.GetEnumerator());
        mockSet.Setup(m => m.AsQueryable()).Returns(elementsAsQueryable);

        return mockSet;
    }
}