namespace OnlineShop.Tests.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Runtime.CompilerServices;
    using System.Web.Http;
    using Data;
    using EntityFramework.Extensions;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.Owin.Testing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Models;
    using Owin;
    using Services;
    using Services.Models.ViewModels;

    [TestClass]
    public class AdsIntegrationTests
    {
        private static TestServer httpTestServer;
        private static HttpClient httpClient;
        private const string TestUserName = "ApuFromSpringfield";
        private const string TestUserPassword = "!23Qweasdf123!EWQ";
        private string accessToken;

        private string AccessToken
        {
            get
            {
                if (this.accessToken == null)
                {
                    var loginResponse = this.Login();
                    if (!loginResponse.IsSuccessStatusCode)
                    {
                        Assert.Fail("Unable to login " + loginResponse.ReasonPhrase);
                    }
                    var loginData = loginResponse.Content
                            .ReadAsAsync<LoginDto>().Result;
                    this.accessToken = loginData.Access_Token;
                }
                return this.accessToken;
            }
        }

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext testContext)
        {
            httpTestServer = TestServer.Create(appBuilder =>
            {
                var config = new HttpConfiguration();
                WebApiConfig.Register(config);
                var startup = new Startup();

                startup.Configuration(appBuilder);
                appBuilder.UseWebApi(config);
            });
            httpClient = httpTestServer.HttpClient;

            SeedDatabase();
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            if (httpTestServer != null)
            {
                CleanDatabase();
                httpTestServer.Dispose();
            }
        }

        [TestMethod]
        public void Login_ShouldReturn200OkAndAccessToken()
        {
            var loginResponse = this.Login();
            var loginData = loginResponse.Content
                .ReadAsAsync<LoginDto>().Result;

            Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);
            Assert.IsNotNull(loginResponse.StatusCode);
        }

        [TestMethod]
        public void PostingAddWithInvalisdAdType_ShoudReturnBadRequest400()
        {
            //Arrange
            var context = new OnlineShopContext();
            var category = context.Categories.First();

            var data = new FormUrlEncodedContent(new []
            {
                new KeyValuePair<string, string>("name", "Opel Astra"), 
                new KeyValuePair<string, string>("description", "..."), 
                new KeyValuePair<string, string>("price", "2000"), 
                new KeyValuePair<string, string>("typeId", "-1"), //invalid
                new KeyValuePair<string, string>("categories[0]", category.Id.ToString()), 
            });

            //Act
            var response = this.PostNewAd(data);

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void PostingAdWithoutcategories_ShouldReturnBadRequest400()
        {
            // Arrange
            var context = new OnlineShopContext();
            var adType = context.AdTypes.First();
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("name", "Opel Astra"), 
                new KeyValuePair<string, string>("description", "..."), 
                new KeyValuePair<string, string>("price", "2000"), 
                new KeyValuePair<string, string>("typeId", adType.Id.ToString()),
                new KeyValuePair<string, string>("categories[0]", "-1"),  //invalid
            });
            // Act
            var response = this.PostNewAd(data);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void PostingAdsWithMoreThan3Categories_ShouldRetrurnBadRequest400()
        {
            // Arrange
            var context = new OnlineShopContext();
            var adType = context.AdTypes.First();
            var categories = context.Categories.Take(4).Select(c=> new {c.Id}).ToList();
            
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("name", "Opel Astra"), 
                new KeyValuePair<string, string>("description", "..."), 
                new KeyValuePair<string, string>("price", "2000"), 
                new KeyValuePair<string, string>("typeId", adType.Id.ToString()),
                new KeyValuePair<string, string>("categories[0]", categories[0].Id.ToString()),
                new KeyValuePair<string, string>("categories[1]", categories[1].Id.ToString()),
                new KeyValuePair<string, string>("categories[2]", categories[2].Id.ToString()),
                new KeyValuePair<string, string>("categories[3]", categories[3].Id.ToString()),  //invalid
            });

            // Act
            var response = this.PostNewAd(data);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void PostingAdWithoutAName_ShouldReturnBadRequest400()
        {
            // Arrange
            var context = new OnlineShopContext();
            var adType = context.AdTypes.First();
            var category = context.Categories.First();

            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("name", ""),
                new KeyValuePair<string, string>("description", "..."), 
                new KeyValuePair<string, string>("price", "2000"), 
                new KeyValuePair<string, string>("typeId", adType.Id.ToString()),
                new KeyValuePair<string, string>("categories[0]", category.Id.ToString()),
            });
            // Act
            var response = this.PostNewAd(data);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void PostingAd_ShouldReturnAdAndOkRequest200()
        {
            // Arrange
            var context = new OnlineShopContext();
            var adType = context.AdTypes.First();
            var category = context.Categories.First();

            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("name", "Another ad"),
                new KeyValuePair<string, string>("description", "More of the same"), 
                new KeyValuePair<string, string>("price", "2500"), 
                new KeyValuePair<string, string>("typeId", adType.Id.ToString()),
                new KeyValuePair<string, string>("categories[0]", category.Id.ToString()),
            });
            // Act
            var response = this.PostNewAd(data);
            var adResponse = response.Content.ReadAsAsync<AdViewModel>().Result;

            if (adResponse == null)
            {
                Assert.Fail("The action did not return a response");
            }

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("Another ad", adResponse.Name);
            Assert.AreEqual("More of the same", adResponse.Description);
            Assert.AreEqual(2500, adResponse.Price);
            Assert.AreEqual(adType.Name, adResponse.Type);
            Assert.AreEqual(category.Id, adResponse.Categories.First().Id);

        }

        private static void SeedDatabase()
        {
            var context = new OnlineShopContext();
            

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new ApplicationUserManager(userStore);

            var user = new ApplicationUser()
            {
                UserName = TestUserName,
                Email = "prakash@yahoo.in"
            };

            var result = userManager.CreateAsync(user, TestUserPassword).Result;

            if (!result.Succeeded)
            {
                Assert.Fail(string.Join(Environment.NewLine, result.Errors));
            }

            SeedCategories(context);
            SeedAdTypes(context);
        }

        private static void SeedAdTypes(OnlineShopContext context)
        {
            var adTypes = new List<AdType>
            {
                new AdType()
                {
                    Name = "Normal",
                    PricePerDay = 3.99m,
                    Index = 100
                },
                new AdType()
                {
                    Name = "Premium",
                    PricePerDay = 5.99m,
                    Index = 200
                },
                new AdType()
                {
                    Name = "Diamond",
                    PricePerDay = 9.99m,
                    Index = 300
                }
            };

            foreach (var adType in adTypes)
            {
                context.AdTypes.Add(adType);
            }

            context.SaveChanges();
        }

        private static void SeedCategories(OnlineShopContext context)
        {
            var categories = new List<Category>()
            {
                new Category() {Name = "Business"},
                new Category() {Name = "Garden"},
                new Category() {Name = "Toys"},
                new Category() {Name = "Pleasure"},
                new Category() {Name = "Electronics"},
                new Category() {Name = "Clothes"}
            };

            foreach (var category in categories)
            {
                context.Categories.Add(category);
            }

            context.SaveChanges();
        }

        private static void CleanDatabase()
        {
            var context = new OnlineShopContext();
            context.Ads.Delete();
            context.Categories.Delete();
            context.AdTypes.Delete();
            context.Users.Delete();
        }

        private HttpResponseMessage Login()
        {
            var loginData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", TestUserName), 
                new KeyValuePair<string, string>("password", TestUserPassword), 
                new KeyValuePair<string, string>("grant_type", "password"), 
            });

            var response = httpClient.PostAsync("/Token", loginData).Result;
            return response;
        }

        private HttpResponseMessage PostNewAd(FormUrlEncodedContent data)
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + this.AccessToken);

            return httpClient.PostAsync("/api/ads", data).Result;
        }


    }

    public class LoginDto
    {
        public string Access_Token { get; set; }
        public string Username { get; set; }
    }
}
