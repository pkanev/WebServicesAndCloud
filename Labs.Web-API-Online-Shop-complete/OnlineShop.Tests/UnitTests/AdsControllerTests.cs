namespace OnlineShop.Tests.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Web.Http;
    using Data.Interfaces;
    using Data.Repositories;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Models;
    using Moq;
    using Services.Controllers;
    using Services.Infrastructure;
    using Services.Models.BindingModels;
    using Services.Models.ViewModels;

    [TestClass]
    public class AdsControllerTests
    {
        private MockContainer mocks;

        [TestInitialize]
        public void InitTest()
        {
            this.mocks = new MockContainer();
            this.mocks.PrepareMocks();
        }

        [TestMethod]
        public void GetAllAds_ShouldReturn_TotalAdsSortedByIndex()
        {
            // Arrange
            var fakeAds = this.mocks.AdRepositoryMock.Object.All();

            var fakeUser = this.mocks.UserRepositoryMock.Object.All().FirstOrDefault();
            if (fakeUser == null)
            {
                Assert.Fail("Cannot perform test - no users available");
            }


            var mockContext = new Mock<IOnlineShopData>();
            mockContext.Setup(mc => mc.Ads.All())
                .Returns(fakeAds);
            var mockIdProvider = new Mock<IUserIdProvider>();
            mockIdProvider.Setup(ip => ip.GetUserId())
                .Returns(fakeUser.Id);

            var adsController = new AdsController(mockContext.Object, mockIdProvider.Object);
            this.SetupController(adsController);

            // Act
            var response = adsController.GetAds()
                .ExecuteAsync(CancellationToken.None).Result;

            var adsResponse = response.Content
                .ReadAsAsync<IEnumerable<AdViewModel>>()
                .Result.Select(a => a.Id)
                .ToList();

            var orderedFakeAds = fakeAds
                .OrderByDescending(a => a.Type.Index)
                .ThenBy(a => a.PostedOn)
                .Select(a => a.Id)
                .ToList();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            CollectionAssert.AreEqual(orderedFakeAds, adsResponse);

        }

        [TestMethod]
        public void CreateAdd_ShouldSuccessfullyAddToRepository()
        {
            // Arrange
            var ads = new List<Ad>();

            var fakeUser = this.mocks.UserRepositoryMock.Object.All().FirstOrDefault();
            if (fakeUser == null)
            {
                Assert.Fail("Cannot perform test - no users available");
            }

            this.mocks.AdRepositoryMock.Setup(r => r.Add(It.IsAny<Ad>()))
                .Callback((Ad ad) =>
                {
                    ad.Owner = fakeUser;
                    ads.Add(ad);
                });

            var mockContext = new Mock<IOnlineShopData>();
            mockContext.Setup(c => c.Ads)
                .Returns(this.mocks.AdRepositoryMock.Object);
            mockContext.Setup(c => c.Categories)
                .Returns(this.mocks.CategoryRepositoryMock.Object);
            mockContext.Setup(c => c.Users)
                .Returns(this.mocks.UserRepositoryMock.Object);
            mockContext.Setup(c => c.AdTypes)
                .Returns(this.mocks.AdTypeRepositoryMock.Object);

            var mockIdProvider = new Mock<IUserIdProvider>();
            mockIdProvider.Setup(ip => ip.GetUserId())
                .Returns(fakeUser.Id);

            var adsController = new AdsController(mockContext.Object, mockIdProvider.Object);
            this.SetupController(adsController);

            // Act
            var randomName = Guid.NewGuid().ToString();
            var newAd = new CreateAdBindingModel()
            {
                Name = randomName,
                Price = 555,
                TypeId = 1,
                Description = "Just a simple descrption",
                Categories = new[] { 1, 2, 3}
            };
            var response = adsController.CreateAd(newAd)
                .ExecuteAsync(CancellationToken.None).Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            mockContext.Verify(c=>c.SaveChanges(), Times.Once);
            Assert.AreEqual(1, ads.Count);
            Assert.AreEqual(newAd.Name, ads[0].Name);
        }

        [TestMethod]
        public void ClosingAddAsOwner_ShouldReturn200Ok()
        {
            var fakeAds = mocks.AdRepositoryMock.Object.All();
            var openAd = fakeAds.FirstOrDefault(a => a.Status == AdStatus.Open);
            if (openAd == null)
            {
                Assert.Fail("Cannot perform test - no open ads are available");
            }

            var id = openAd.Id;

            var mockContext = new Mock<IOnlineShopData>();
            mockContext.Setup(c => c.Ads)
                .Returns(this.mocks.AdRepositoryMock.Object);

            var userIdProvider = new Mock<IUserIdProvider>();
            userIdProvider.Setup(ui => ui.GetUserId())
                .Returns(openAd.OwnerId);

            var adsController = new AdsController(mockContext.Object, userIdProvider.Object);
            this.SetupController(adsController);

            // Act
            var response = adsController.CloseAd(id)
                .ExecuteAsync(CancellationToken.None).Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            mockContext.Verify(c=>c.SaveChanges(), Times.Once);
            Assert.IsNotNull(openAd.ClosedOn);
            Assert.AreEqual(AdStatus.Closed, openAd.Status);
        }

        [TestMethod]
        public void ClosingAddAsNonOwner_ShouldReturn_Unauthorised401()
        {
            var openAd = mocks.AdRepositoryMock.Object
                .All()
                .FirstOrDefault(a => a.Status == AdStatus.Open);
            if (openAd == null)
            {
                Assert.Fail("There are no open ads in the repository!");
            }
            
            var foreignUser = mocks.UserRepositoryMock.Object
                .All()
                .FirstOrDefault(u => u.Id != openAd.OwnerId);
            if (foreignUser == null)
            {
                Assert.Fail("There is no user who does not own this add");
            }

            var userIdProvider = new Mock<IUserIdProvider>();
            userIdProvider.Setup(ui => ui.GetUserId())
                .Returns(foreignUser.Id);

            var mockContext = new Mock<IOnlineShopData>();
            mockContext.Setup(c => c.Ads)
                .Returns(mocks.AdRepositoryMock.Object);

            var adsController = new AdsController(mockContext.Object, userIdProvider.Object);
            this.SetupController(adsController);

            // Act
            var response = adsController.CloseAd(openAd.Id)
                .ExecuteAsync(CancellationToken.None).Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            mockContext.Verify(c=>c.SaveChanges(), Times.Never);
            Assert.AreEqual(AdStatus.Open, openAd.Status);

        }

        private void SetupController(AdsController adsController)
        {
            adsController.Request = new HttpRequestMessage();
            adsController.Configuration = new HttpConfiguration();
        }
    }
}
