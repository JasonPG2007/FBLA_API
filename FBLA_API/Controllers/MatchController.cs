using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using ObjectBusiness;
using Repository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FBLA_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        #region Variables
        private readonly IMatchRepository matchRepository;
        private readonly IVerificationCodeRepository verificationCodeRepository;
        #endregion

        #region Constructor
        public MatchController(IMatchRepository matchRepository, IVerificationCodeRepository verificationCodeRepository)
        {
            this.matchRepository = matchRepository;
            this.verificationCodeRepository = verificationCodeRepository;
        }
        #endregion

        // GET: api/<MatchController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<MatchController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<MatchController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Match match)
        {
            var postExists = matchRepository.AllMatches().Any(m => m.LostPostId == match.LostPostId);
            if (postExists)
            {
                return Conflict(new
                {
                    message = "Your lost post is already matched"
                });
            }

            var isAddedMatch = await matchRepository.CreateMatch(match);
            if (isAddedMatch)
            {
                var isAddedVerificationCode = await verificationCodeRepository.CreateVerificationCode(new VerificationCode
                {
                    MatchId = match.MatchId,
                    Code = match.Code
                });
                return Ok("Create match successfully");
            }

            return BadRequest("Create match failed");
        }

        // PUT api/<MatchController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<MatchController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
