using System.Web.Http;
using OnlineShop.Data;

namespace OnlineShop.Services.Controllers
{
    using Data.Interfaces;
    using Infrastructure;

    public class BaseApiController : ApiController
    {
        private IOnlineShopData data;
        private IUserIdProvider userIdProvider;

        public BaseApiController()
            : this(new OnlineShopData(new OnlineShopContext()), new AspNetUserIdProvider())
        {

        }

        public BaseApiController(IOnlineShopData data, IUserIdProvider userIdProvider)
        {
            this.Data = data;
            this.UserIdProvider = userIdProvider;
        }

        protected IOnlineShopData Data
        {
            get
            {
                return this.data;
            }
            private set
            {
                this.data = value;
            }
        }

        protected IUserIdProvider UserIdProvider
        {
            get { return this.userIdProvider; }
            private set { this.userIdProvider = value; }
        }
    }
}