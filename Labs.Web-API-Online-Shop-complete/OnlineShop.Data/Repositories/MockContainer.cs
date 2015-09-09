using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using OnlineShop.Data.Interfaces;
using OnlineShop.Models;

namespace OnlineShop.Data.Repositories
{
    public class MockContainer
    {
        public Mock<IRepository<Ad>> AdRepositoryMock { get; set; }
        public Mock<IRepository<Category>> CategoryRepositoryMock { get; set; }
        public Mock<IRepository<AdType>> AdTypeRepositoryMock { get; set; }
        public Mock<IRepository<ApplicationUser>> UserRepositoryMock { get; set; }

        public void PrepareMocks()
        {
            this.SetupFakeAdTypes();
            this.SetupFakeAds();
            this.SetupFakeUsers();
            this.SetupFakeCategories();
        }

        private void SetupFakeAds()
        {
            var adTypes = new List<AdType>()
            {
                new AdType(){Name = "Normal", Index = 100},
                new AdType(){Name = "Premium", Index = 200}
            };

            var fakeAds = new List<Ad>()
            {
                new Ad()
                {
                    Id = 5,
                    Name = "Audi A6",
                    Type = adTypes[0],
                    PostedOn = DateTime.Now.AddDays(-6),
                    Owner = new ApplicationUser(){UserName = "Gosho", Id = "123"},
                    Price = 400
                },
                new Ad()
                {
                    Id = 5,
                    Name = "Dacia Sandero",
                    Type = adTypes[1],
                    PostedOn = DateTime.Now.AddDays(-2),
                    Owner = new ApplicationUser(){UserName = "Sasho", Id = "113"},
                    Price = 250
                },
                new Ad()
                {
                    Id = 6,
                    Name = "BMW m3",
                    Type = adTypes[0],
                    PostedOn = DateTime.Now.AddDays(-10),
                    Owner = new ApplicationUser(){UserName = "Misho", Id = "111"},
                    Price = 1000
                },
                new Ad()
                {
                    Id = 7,
                    Name = "Trabant",
                    Type = adTypes[0],
                    PostedOn = DateTime.Now.AddDays(-20),
                    Owner = new ApplicationUser(){UserName = "Pesho", Id = "101"},
                    Price = 50
                },
                new Ad()
                {
                    Id = 8,
                    Name = "Reliant Robin",
                    Type = adTypes[1],
                    PostedOn = DateTime.Now.AddDays(-1),
                    Owner = new ApplicationUser(){UserName = "Vanyo", Id = "105"},
                    Price = 550
                }
            };

            var repositoryMock = new Mock<IRepository<Ad>>();
            repositoryMock.Setup(r => r.All())
                .Returns(fakeAds.AsQueryable());

            repositoryMock.Setup(r => r.Find(It.IsAny<int>()))
                .Returns((int id) =>
                {
                    return fakeAds.FirstOrDefault(a => a.Id == id);
                });

            this.AdRepositoryMock = repositoryMock;
        }

        private void SetupFakeAdTypes()
        {
            var fakeAdTypes = new List<AdType>()
            {
                new AdType(){Name = "Normal", Index = 100, Id = 1},
                new AdType(){Name = "Premium", Index = 200, Id = 2},
                new AdType(){Name = "Gold", Index = 300, Id = 3}
            };

            var repositoryMock = new Mock<IRepository<AdType>>();
            repositoryMock.Setup(r => r.All())
                .Returns(fakeAdTypes.AsQueryable());
            repositoryMock.Setup(r => r.Find(It.IsAny<int>()))
                .Returns((int id) =>
                {
                    return fakeAdTypes.FirstOrDefault(at => at.Id== id);
                });

            this.AdTypeRepositoryMock = repositoryMock;
        }

        private void SetupFakeUsers()
        {
            var fakeUsers = new List<ApplicationUser>()
            {
                new ApplicationUser(){UserName = "Gosho", Id = "123"},
                new ApplicationUser(){UserName = "Sasho", Id = "113"},
                new ApplicationUser(){UserName = "Misho", Id = "111"},
                new ApplicationUser(){UserName = "Pesho", Id = "101"},
                new ApplicationUser(){UserName = "Vanyo", Id = "105"},
            };

            var repositoryMock = new Mock<IRepository<ApplicationUser>>();
            repositoryMock.Setup(r => r.All())
                .Returns(fakeUsers.AsQueryable());
            repositoryMock.Setup(r => r.Find(It.IsAny<string>()))
                .Returns((string id) =>
                {
                    return fakeUsers.FirstOrDefault(fu => fu.Id == id);
                });
            this.UserRepositoryMock = repositoryMock;
        }

        private void SetupFakeCategories()
        {
            var fakeCategories = new List<Category>()
            {
                new Category(){Id = 1, Name = "Work"},
                new Category(){Id = 2, Name = "Music"},
                new Category(){Id = 3, Name = "Sports"},
                new Category(){Id = 4, Name = "Cinema"},
                new Category(){Id = 5, Name = "Technology"},
            };

            var repositoryMock = new Mock<IRepository<Category>>();
            repositoryMock.Setup(r => r.All())
                .Returns(fakeCategories.AsQueryable());
            repositoryMock.Setup(r => r.Find(It.IsAny<int>()))
                .Returns((int id) =>
                {
                    return fakeCategories.FirstOrDefault(fc => fc.Id == id);
                });
            this.CategoryRepositoryMock = repositoryMock;
        }
    }
}