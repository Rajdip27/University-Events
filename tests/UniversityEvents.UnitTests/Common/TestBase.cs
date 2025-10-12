using Microsoft.EntityFrameworkCore;
using Moq;
using UniversityEvents.Application.Caching;
using UniversityEvents.Application.Helpers;
using UniversityEvents.Infrastructure.Data;
using UniversityEvents.Infrastructure.Healper.Acls;

namespace UniversityEvents.UnitTests.Common
{
    public abstract class TestBase
    {
        protected readonly UniversityDbContext Context;
        protected readonly Mock<IRedisCacheHelper> CacheMock;
        protected readonly Mock<ISignInHelper> SignInMock;

        protected TestBase()
        {
            var options = new DbContextOptionsBuilder<UniversityDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            SignInMock = new Mock<ISignInHelper>();
            Context = new UniversityDbContext(options, SignInMock.Object);

            CacheMock = new Mock<IRedisCacheHelper>();
        }
    }
}
