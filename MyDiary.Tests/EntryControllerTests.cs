using AutoFixture;
using Moq;
using MyDiary.Controllers;
using MyDiary.DataObjects;
using MyDiary.Models;
using MyDiary.Tests.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Http.OData;
using System.Web.Http.Results;
using System.Web.Http.Routing;
using System.Web.Routing;
using Xunit;
using FluentAssertions;
using System.Threading.Tasks;

namespace MyDiary.Tests
{
    public class EntryControllerTests :ControllerTestBase<EntryController,Entry, MobileServiceContext>
    {
        Mock<MobileServiceContext> mockedContext;
        IList<Entry> entries;

        public EntryControllerTests()
        {
            mockedContext = new Mock<MobileServiceContext>();
            // Setup user
            this.Controller.User = new TestPrincipal(
                new Claim(ClaimTypes.NameIdentifier, "testuser"),
                new Claim("sub", "foo")
                );

            //setup sample entries
            var fixture = new Fixture();
            entries = new List<Entry>
                  {
                    fixture.Build<Entry>()
                    .With(e => e.UserId,"testuser")
                    .With(e => e.Id, "123")
                    .With(e => e.Deleted, false)
                    .Create<Entry>(),
                    fixture.Build<Entry>()
                    .With(e => e.Deleted, false)
                    .Create<Entry>(),
                    fixture.Build<Entry>()
                    .With(e => e.Deleted, false)
                    .Create<Entry>(),
                  };

            //setup dbcontext and domain manager
            mockedContext
                .Setup(x => x.Set<Entry>())
                .ReturnsDbSetAsync(entries);

            

            this.SetUpDomainManager(mockedContext.Object);
        }

        [Fact]
        public void GetUserId_With_Correct_Claims()
        {
            //arrange
            var controller = new EntryController();
            controller.User = new TestPrincipal(
                new Claim(ClaimTypes.NameIdentifier, "testuser"),
                new Claim("sub", "foo")
                );

            //act
            var result = controller.GetUserId();

            //assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result);
        }

        [Fact]
        public void GetUserId_With_InComplete_Claims()
        {
            //arrange
            var controller = new EntryController();
            controller.User = new TestPrincipal(
                new Claim("sub", "foo")
                );

            //act
            var result = controller.GetUserId();

            //assert
            Assert.Null(result);
        }

        [Fact]
        public void GetUserId_With_Null_Claims()
        {
            //arrange
            var controller = new EntryController();
            controller.User = null;
            //act
            var ex = Assert.Throws<HttpResponseException>(() =>
              {
                  var result = controller.GetUserId();
              }
            );

            //assert
            Assert.Equal(HttpStatusCode.Unauthorized, ex.Response.StatusCode);
        }

        [Fact]
        public void GetAllDiaryEntries()
        {
            // Arrange
          
            // Act
            var allEntries = this.Controller.GetAllDiaryEntries();

            // Assert
            Assert.Equal(1, allEntries.Count());
        }

        [Fact]
        public async void GetDiaryEntry()
        {
            // Arrange

            // Act
            var singleEntry = await this.Controller.GetDiaryEntry("123");

            // Assert
            Assert.Equal(entries[0].Id, singleEntry.Id.ToString());
        }

        [Fact]
        public async void UpdateDiaryEntry()
        {
            // Arrange
            var delta = new Delta<Entry>(typeof(Entry));
            delta.TrySetPropertyValue("Title", "new");

            // Act

            var singleEntry = await this.Controller.UpdateDiaryEntry("123", delta);

            // Assert
            Assert.Equal(entries[0].Title, "new");
        }

        [Fact]
        public  async void DeleteDiaryEntry()
        {
            // Arrange
            var delta = new Delta<Entry>(typeof(Entry));
            delta.TrySetPropertyValue("Deleted", true);

            // Act
            var singleEntry = await this.Controller.UpdateDiaryEntry("123", delta);


            // Assert
            Assert.Equal(entries.Where(e => e.Deleted==false).Count(), 2);
        }

        [Fact]
        public void DeleteDiaryEntry_IdNotExits_HttpResponseException_BadRequest()
        {
            // Arrange

            // Act

           Action ax= () => this.Controller.DeleteDiaryEntry("9999");

            // Assert
            var result = ax.Should().Throw<HttpResponseException>();
            result.And.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.And.Response.ReasonPhrase.Should().Be("Id does not exist!");
        }

        [Fact]
        public async void InsertDiaryEntry()
        {
            // Arrange
            var newEntry = new Entry
            {
                Title = "newEntryAdded",
                UserId="testuser"
            };
         

            // Act
            await this.Controller.InsertDiaryEntry(newEntry);

            // Assert
            Assert.Equal(entries.Count(), 3);
        }

        [Fact]
        public async void InsertDiaryEntry_NoUserId_UnauthorizedResponse()
        {
            // Arrange
            var newEntry = new Entry
            {
                Title = "newEntryAdded"
            };


            // Act
            IHttpActionResult response =  await this.Controller.InsertDiaryEntry(newEntry);

            // Assert
            Assert.IsType<System.Web.Http.Results.UnauthorizedResult>(response);
        }


    }
}
