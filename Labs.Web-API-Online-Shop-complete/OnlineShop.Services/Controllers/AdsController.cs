using System.Linq;
using System.Web.Http;
using OnlineShop.Models;

namespace OnlineShop.Services.Controllers
{
    using System;
    using System.Collections.Generic;
    using Data.Interfaces;
    using Infrastructure;
    using Microsoft.AspNet.Identity;
    using Models.BindingModels;
    using Models.ViewModels;

    [Authorize]
    public class AdsController : BaseApiController
    {
        //public AdsController()
        //    : base()
        //{
            
        //}

        public AdsController(IOnlineShopData data, IUserIdProvider userIdProvider)
            : base(data, userIdProvider)
        {
        }

        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult GetAds()
        {
            var ads = this.Data.Ads
                .All()
                .Where(a=>a.Status == AdStatus.Open)
                .OrderByDescending(a=>a.Type.Index)
                .ThenBy(a=>a.PostedOn)
                .Select(AdViewModel.Create);
            return this.Ok(ads);
        }

        [HttpPost]
        public IHttpActionResult CreateAd(CreateAdBindingModel model)
        {
            if (model == null)
            {
                return this.BadRequest("Model cannot be empty!");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }
            
            var type = Data.AdTypes.All().FirstOrDefault(t => t.Id == model.TypeId);
            if (type == null)
            {
                return this.BadRequest("There is no ad type with such id!");
            }

            int numOfCategories = model.Categories.Count();
            if (numOfCategories < 1 || numOfCategories > 3)
            {
                return this.BadRequest("The number of categories should be between 1 and 3");
            }

            List<Category> categories = new List<Category>();
            foreach (var catId in model.Categories)
            {
                var category = Data.Categories.All().FirstOrDefault(c => c.Id == catId);
                if (category == null)
                {
                    string message = string.Format("There is no category with such id: {0}!", catId);
                    return this.BadRequest(message);
                }
                categories.Add(category);
            }

            var userId = this.UserIdProvider.GetUserId();

            var ad = new Ad()
            {
                Name = model.Name,
                Description = model.Description,
                PostedOn = DateTime.Now,
                TypeId = model.TypeId,
                OwnerId = userId,
                Price = model.Price,
                Categories = categories,

            };

            Data.Ads.Add(ad);
            Data.SaveChanges();

            var result = this.Data.Ads.All()
                .Where(a => a.Id == ad.Id)
                .Select(AdViewModel.Create)
                .FirstOrDefault();

            return this.Ok(result);
        }

        [HttpPut]
        [Route("api/ads/{id}/close")]
        public IHttpActionResult CloseAd([FromUri]int id)
        {
            Ad ad = this.Data.Ads.All().FirstOrDefault(a => a.Id == id);
            if (ad == null)
            {
                return this.NotFound();
            }

            string userId = this.UserIdProvider.GetUserId();

            if (ad.OwnerId != userId)
            {
                return this.Unauthorized();
            }

            ad.Status = AdStatus.Closed;
            ad.ClosedOn = DateTime.Now;
            Data.SaveChanges();


            return this.Ok();
        }
    }
}