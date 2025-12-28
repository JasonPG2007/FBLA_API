using DataAccess;
using ObjectBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Repository
{
    public class PostRepository : IPostRepository
    {
        #region Variables
        private readonly PostDAO postDAO;
        private readonly MatchDAO matchDAO;
        #endregion

        #region Constructor
        public PostRepository(PostDAO postDAO, MatchDAO matchDAO)
        {
            this.postDAO = postDAO;
            this.matchDAO = matchDAO;
        }
        #endregion

        #region Get Found Posts
        public IQueryable<Posts> GetFoundPosts()
        {
            return postDAO.GetFoundPosts();
        }
        #endregion

        #region All posts by user ID
        public IQueryable<Posts> AllPostsByUserId(int userId)
        {
            return postDAO.AllPostsByUserId(userId);
        }
        #endregion

        #region Get Post By Id
        public Posts GetPostById(int postId)
        {
            return postDAO.GetPostById(postId);
        }
        #endregion

        #region Create Post
        public async Task<bool> CreatePost(Posts post)
        {
            var isAdded = await postDAO.CreatePost(post);
            return isAdded;
        }
        #endregion

        // Funtion not related to Entity
        #region Get random string
        public string GetRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();

            char[] charsArray = new char[length];
            for (int i = 0; i < length; i++)
            {
                charsArray[i] = chars[random.Next(charsArray.Length)];
            }

            return new string(charsArray);
        }
        #endregion
    }
}
