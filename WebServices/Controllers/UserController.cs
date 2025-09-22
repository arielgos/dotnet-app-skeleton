using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BSE = Business.Security;
using ENT = Entities;
using ESE = Entities.Security;

namespace Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(UserController));

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<ESE.User>> Get()
        {
            try
            {
                return Ok(new BSE.User().List());
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<ESE.User> Get(string id)
        {
            try
            {
                BSE.User bUser = new BSE.User();
                ESE.User result = bUser.Search(id);
                if (result != null && result.Id.Length>0)
                {
                    return Ok(result);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }

        // POST api/values
        [HttpPost]
        public ActionResult Post([FromBody] ESE.User user)
        {
            try
            {
                BSE.User bUser = new BSE.User();
                user.Action = ENT.Action.Insert;
                if (bUser.Save(user))
                {
                    return CreatedAtRoute(routeValues: new { id = user.Id }, value: user);
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public ActionResult Put(string id, [FromBody] ESE.User user)
        {
            try
            {
                BSE.User bUser = new BSE.User();
                ESE.User result = bUser.Search(id);
                if (result != null && result.Id.Length > 0)
                {
                    user.Id = id;
                    user.Action = ENT.Action.Update;
                    if (bUser.Save(user))
                    {
                        return Accepted();
                    }

                    return BadRequest();
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            try
            {
                BSE.User bUser = new BSE.User();
                ESE.User result = bUser.Search(id);
                if (result != null && result.Id.Length > 0)
                {
                    if (bUser.Delete(result))
                    {
                        return NoContent();
                    }

                    return BadRequest();
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }
    }
}