using DataAccess;
using FBLA_API.DTOs.Match;
using Microsoft.AspNetCore.Mvc;
using ObjectBusiness;
using Repository;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FBLA_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        #region Variables
        private readonly IPostRepository postRepository;
        private readonly IUsersRepository userRepository;
        private readonly IWebHostEnvironment webHost;
        #endregion

        #region Constructor
        public PostController(IPostRepository postRepository,
                               IUsersRepository userRepository,
                               IWebHostEnvironment webHost)
        {
            this.postRepository = postRepository;
            this.userRepository = userRepository;
            this.webHost = webHost;
        }
        #endregion

        // GET: api/<PostController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("my-posts")]
        public async Task<ActionResult<List<Posts>>> GetAllPostsByUserId()
        {
            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userEmail == null)
            {
                return Unauthorized("User not authenticated");
            }

            var user = userRepository.GetUserByEmail(userEmail);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var posts = await postRepository.AllPostsByUserId(user.UserId).ToListAsync();

            foreach (var post in posts)
            {
                if (!string.IsNullOrEmpty(post.Image))
                {
                    post.UrlImage = $"{Request.Scheme}://{Request.Host}/Uploads/{post.Image}";
                }
            }

            return Ok(posts);
        }

        #region Suggest Post
        [HttpGet("suggest-post/{postId}")]
        public ActionResult<List<MatchSuggestionDTO>> SuggestPost(int postId)
        {
            var post = postRepository.GetPostById(postId);
            if (post == null)
            {
                return NotFound("This post does not found");
            }

            if (post.TypePost != TypePost.Lost)
            {
                return Ok("Only lost posts can suggest found posts");
            }

            var suggestion = new List<MatchSuggestionDTO>();

            // Check similar post (Suggest post)
            foreach (var found in postRepository.GetFoundPosts())
            {
                // Calculate score each found post
                var score = CalculateSimilarity(post.Description, found.Description);

                if (score >= 65)
                {
                    suggestion.Add(new MatchSuggestionDTO
                    {
                        PostId = found.PostId,
                        Score = score,
                        CategoryPostId = found.CategoryPostId,
                        CreatedAt = found.CreatedAt,
                        Description = found.Description,
                        Image = found.Image,
                        Title = found.Title,
                        TypePost = found.TypePost,
                        Vector = found.Vector,
                        Code = found.Code,
                        UrlImage = found.UrlImage = $"{Request.Scheme}://{Request.Host}/Uploads/{found.Image}",
                        User = new Users
                        {
                            UserId = found.User.UserId,
                            FirstName = found.User.FirstName,
                            LastName = found.User.LastName,
                            Email = found.User.Email,
                            UrlAvatar = $"{Request.Scheme}://{Request.Host}/Uploads/{found.User.Avatar}"
                        }
                    });
                }
            }

            return Ok(suggestion.OrderByDescending(x => x.Score).Take(5).ToList());
        }
        #endregion

        // GET api/<PostController>/5
        [HttpGet("{postId}")]
        public ActionResult<Posts> Get(int postId)
        {
            var post = postRepository.GetPostById(postId);

            if (post == null)
            {
                return NotFound("This post does not found");
            }

            post.UrlImage = $"{Request.Scheme}://{Request.Host}/Uploads/{post.Image}";
            post.User = new Users
            {
                UserId = post.UserId,
                FirstName = post.User.FirstName,
                LastName = post.User.LastName,
                Email = post.User.Email,
                UrlAvatar = $"{Request.Scheme}://{Request.Host}/Uploads/{post.User.Avatar}"
            };
            return Ok(post);
        }

        // POST api/<PostController>
        [HttpPost]
        public async Task<ActionResult> Post([FromForm] Posts post)
        {
            if (post.ImageUpload != null)
            {
                post.Image = await UploadFiles(post.ImageUpload);
            }

            if (post.TypePost == TypePost.Found)
            {
                var isAdded = await postRepository.CreatePost(post);
                if (isAdded)
                {
                    return Ok("Create post successfully");
                }
            }
            else
            {
                var isAdded = await postRepository.CreatePost(post);
                if (isAdded)
                {
                    return Ok("Create post successfully");
                }
            }
            return BadRequest("Create post failed");
        }

        // PUT api/<PostController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<PostController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        // Funtion not related to Entity
        #region Function calculation similarity
        private int CalculateSimilarity(string lostDesc, string foundDesc)
        {
            var lostWords = NormalizeText(lostDesc);
            var foundWords = new HashSet<string>(NormalizeText(foundDesc));

            int matchCount = 0;

            foreach (var word in lostWords)
            {
                if (foundWords.Contains(word))
                {
                    matchCount++;
                }
            }

            if (lostWords.Count == 0)
            {
                return 0;
            }

            return (int)Math.Round((double)matchCount / lostWords.Count * 100);
        }
        #endregion

        #region Normalize Text
        private List<string> NormalizeText(string text)
        {
            text = text.ToLower();
            text = RemoveVietnameseTones(text);

            text = Regex.Replace(text, @"[^a-z0-9\s]", " ");

            var stopWords = new HashSet<string>()
            {
                "la", "va", "tai", "toi", "bi"
            };

            return text
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Where(w => w.Length > 1 && !stopWords.Contains(w))
                .ToList();
        }
        #endregion

        #region Remove Vietnamese Tones
        private string RemoveVietnameseTones(string text)
        {
            string normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                var unicodeCategory = Char.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
        #endregion

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

            return fileName;
        }
        #endregion
    }
}
