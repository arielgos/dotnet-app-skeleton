using Microsoft.AspNetCore.Mvc;

namespace Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(AppController));

        // GET: api/values
        [HttpGet]
        public ActionResult<DateTime> Get()
        {
            log.Debug("Get Datetime");
            return DateTime.Now;
        }
    }
}