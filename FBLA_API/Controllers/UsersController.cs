using Microsoft.AspNetCore.Mvc;
using ObjectBusiness;
using Repository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FBLA_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        #region Variables
        private readonly IUsersRepository usersRepository;
        #endregion

        #region Constructor
        public UsersController(IUsersRepository usersRepository)
        {
            this.usersRepository = usersRepository;
        }
        #endregion

        // GET: api/<UsersController>
        [HttpGet]
        public IQueryable<Users> Get()
        {
            return usersRepository.AllUsers();
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UsersController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
