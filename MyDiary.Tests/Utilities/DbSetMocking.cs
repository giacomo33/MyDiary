using Moq;
using Moq.Language;
using Moq.Language.Flow;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDiary.Tests.Utilities
{
    public static class DbSetMocking
    {
        private static Mock<DbSet<T>> CreateMockSet<T>(IQueryable<T> data)
                where T : class
        {
            var queryableData = data.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider)
                    .Returns(queryableData.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression)
                    .Returns(queryableData.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType)
                    .Returns(queryableData.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator())
                    .Returns(queryableData.GetEnumerator());
            return mockSet;
        }

        public static IReturnsResult<TContext> ReturnsDbSet<TEntity, TContext>(
                this IReturns<TContext, DbSet<TEntity>> setup,
                TEntity[] entities)
            where TEntity : class
            where TContext : DbContext
        {
            var mockSet = CreateMockSet(entities.AsQueryable());
            return setup.Returns(mockSet.Object);
        }

        public static IReturnsResult<TContext> ReturnsDbSet<TEntity, TContext>(
                this IReturns<TContext, DbSet<TEntity>> setup,
                IQueryable<TEntity> entities)
            where TEntity : class
            where TContext : DbContext
        {
            var mockSet = CreateMockSet(entities);
            return setup.Returns(mockSet.Object);
        }

        public static IReturnsResult<TContext> ReturnsDbSet<TEntity, TContext>(
                this IReturns<TContext, DbSet<TEntity>> setup,
                IEnumerable<TEntity> entities)
            where TEntity : class
            where TContext : DbContext
        {
            var mockSet = CreateMockSet(entities.AsQueryable());
            return setup.Returns(mockSet.Object);
        }

        public static IReturnsResult<TContext> ReturnsDbSetAsync<TEntity, TContext>(
               this IReturns<TContext, DbSet<TEntity>> setup,
               IEnumerable<TEntity> entities)
           where TEntity : class
           where TContext : DbContext
        {
            var mockSet = MockAsyncData<TEntity>.MockAsyncQueryResult(entities.AsQueryable());
            return setup.Returns(mockSet.Object);
        }
    }

    static class MockAsyncData<T> where T : class
    {
        public static Mock<DbSet<T>> MockAsyncQueryResult(IQueryable<T> data)
        {
            var mockSet = new Mock<DbSet<T>>();

            mockSet.As<IDbAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<T>(data.GetEnumerator()));

            mockSet.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<T>(data.Provider));

            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            return mockSet;
        }
    }
}
