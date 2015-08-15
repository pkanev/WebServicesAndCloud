using System;
using System.Web.Http;

namespace Q03DistCalculatorRestService.Controllers
{
    public class PointsController : ApiController
    {
        //api/points
        [HttpGet]
        public IHttpActionResult GetDistance(int startX, int startY, int endX, int endY)
        {
            int deltaX = endX - startX;
            int deltaY = endY - startY;
            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            return this.Ok(distance);
        }
    }
}
