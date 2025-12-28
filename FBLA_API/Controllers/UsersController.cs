using FBLA_API.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ObjectBusiness;
using Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FBLA_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        #region Variables
        private readonly IUsersRepository usersRepository;
        private readonly IStudentRepository studentRepository;
        private readonly IWebHostEnvironment webHost;
        private readonly IConfiguration configuration;
        #endregion

        #region Constructor
        public UsersController(IUsersRepository usersRepository,
                               IWebHostEnvironment webHost,
                               IConfiguration configuration,
                               IStudentRepository studentRepository)
        {
            this.usersRepository = usersRepository;
            this.webHost = webHost;
            this.configuration = configuration;
            this.studentRepository = studentRepository;
        }
        #endregion

        // GET: api/<UsersController>
        [Authorize]
        [HttpGet]
        public IQueryable<Users> Get()
        {
            return usersRepository.AllUsers();
        }

        // GET My Profile: api/<UsersController>
        [Authorize]
        [HttpGet("profile")]
        public IActionResult GetMyProfile()
        {
            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Get User email from user signed in in JWT (AccessToken)

            if (userEmail == null)
            {
                return Unauthorized("User not authenticated");
            }

            var user = usersRepository.GetUserByEmail(userEmail);

            if (user == null)
            {
                return NotFound("User not found");
            }

            user.UrlAvatar = $"{Request.Scheme}://{Request.Host}/uploads/{user.Avatar}";

            return Ok(new
            {
                user.UserId,
                user.FirstName,
                user.LastName,
                user.Email,
                Role = user.Role.ToString(),

                Student = user.Student == null
                     ? null
                     : new
                     {
                         user.Student.StudentId,
                         user.Student.CreatedAt
                     }
            });
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UsersController>
        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp([FromBody] Users user)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                            .ToList();

                return BadRequest(new { Errors = errors });
            }

            var existingUserByEmail = usersRepository.AllUsers()
                                                     .Any(u => u.Email.Equals(user.Email));

            if (existingUserByEmail)
            {
                return Conflict(new
                {
                    message = "Your email is already in use"
                });
            }

            try
            {
                var isAddedUser = await usersRepository.SignUp(user);
                if (isAddedUser)
                {
                    var existingStudentByStudentId = studentRepository.AllStudents()
                                                                      .Any(s => s.StudentId == user.StudentId);

                    if (existingStudentByStudentId)
                    {
                        return Conflict(new
                        {
                            message = "Your student ID is already in use"
                        });
                    }

                    var student = new Student
                    {
                        StudentId = user.StudentId,
                        UserId = user.UserId,
                    };

                    var isAddedStudent = await studentRepository.SignUpStudent(student);

                    if (isAddedStudent)
                    {
                        return Ok("Sign up successfully");
                    }
                }

                return BadRequest("Failed to sign up. Please try again");
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong!");
            }
        }

        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequestDTO signInRequestDTO)
        {
            try
            {
                var user = await usersRepository.SignIn(signInRequestDTO.StudentId, signInRequestDTO.Password, signInRequestDTO.Email);
                if (user != null)
                {
                    return Ok(); // Ok to change to select image
                }

                return Unauthorized("Student ID or password is invalid");
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong!");
            }
        }

        [HttpPost("select-image")]
        public async Task<IActionResult> SelectImage([FromBody] SignInRequestDTO signInRequestDTO)
        {
            try
            {
                var user = await usersRepository.SignIn(signInRequestDTO.StudentId, signInRequestDTO.Password, signInRequestDTO.Email);
                if (user != null)
                {
                    // Check Pick image
                    var correctImages = new List<int>
                    {
                        Convert.ToInt32(user.PickImage1),
                        Convert.ToInt32(user.PickImage2)
                    };

                    var isCorrect = signInRequestDTO.PickedIndexes.Count == correctImages.Count &&
                                    signInRequestDTO.PickedIndexes.All(i => correctImages.Contains(i));

                    if (isCorrect)
                    {
                        // JWT config
                        var issuer = configuration["JwtConfig:Issuer"];
                        var audience = configuration["JwtConfig:Audience"];
                        var key = configuration["JwtConfig:Key"];
                        var tokenValidityMins = configuration.GetValue<int>("JwtConfig:TokenValidityMins");
                        var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(tokenValidityMins); // Token expiration time

                        // Create JWT access token and assign token
                        var tokenDescriptor = new SecurityTokenDescriptor // Describe token
                        {
                            Subject = new ClaimsIdentity(new[]
                            {
                                new Claim(ClaimTypes.NameIdentifier, user.Email),
                                new Claim(ClaimTypes.Role, user.Role.ToString()),
                            }),
                            Issuer = issuer,
                            Audience = audience,
                            Expires = tokenExpiryTimeStamp,
                            SigningCredentials = new SigningCredentials(
                                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                                SecurityAlgorithms.HmacSha256Signature
                            )
                        };

                        // Process token was described
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var securityToken = tokenHandler.CreateToken(tokenDescriptor); // Create object JWT Token
                        var accessToken = tokenHandler.WriteToken(securityToken); // Serialize token to string  for client

                        // Assign token for client
                        user.AccessToken = accessToken;
                        user.ExpiresIn = (int)tokenExpiryTimeStamp.Subtract(DateTime.Now).TotalSeconds;

                        Response.Cookies.Append("AccessToken", accessToken, new CookieOptions
                        {
                            Secure = true, // Set to true in production
                            HttpOnly = true,
                            SameSite = SameSiteMode.None, // If SameSite is none Secure must be true
                            Expires = tokenExpiryTimeStamp // Set cookie expiration time
                        });

                        Response.Cookies.Append("Username", user.FirstName + user.LastName, new CookieOptions
                        {
                            Secure = true,
                            SameSite = SameSiteMode.None,
                            Expires = tokenExpiryTimeStamp
                        });

                        return Ok();
                    }
                    else
                    {
                        return Unauthorized("Incorrect selection");
                    }
                }

                return Unauthorized("Student ID or password is invalid");
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong!");
            }
        }

        #region Sign out
        [HttpPost("sign-out")]
        public ActionResult SignOut()
        {
            Response.Cookies.Delete("AccessToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/" // Ensure the cookie is deleted from the root path
            });

            Response.Cookies.Delete("Username", new CookieOptions
            {
                Secure = true,
                SameSite = SameSiteMode.None,
            });

            return Ok("Signed out successfully");
        }
        #endregion

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

        #region Upload Files
        private async Task<string> UploadFiles(IFormFile file)
        {
            if (file == null) return "";

            string uploadsFolder = Path.Combine(webHost.WebRootPath, "Uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string fileName = Guid.NewGuid() + Path.GetFileName(file.FileName);
            string fileSavePath = Path.Combine(uploadsFolder, fileName);

            using FileStream stream = new FileStream(fileSavePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return fileSavePath;
        }
        #endregion
    }
}
