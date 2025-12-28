using Microsoft.EntityFrameworkCore;
using ObjectBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class PostDAO
    {
        #region Variables
        private readonly FBLADbContext db;
        #endregion

        #region Constructor
        public PostDAO(FBLADbContext db)
        {
            this.db = db;
        }
        #endregion

        #region Create post
        public async Task<bool> CreatePost(Posts post)
        {
            post.PostId = new Random().Next();
            var isAdded = db.Posts.Add(post);
            if (isAdded != null)
            {
                await db.SaveChangesAsync();
                return true;
            }
            return false;
        }
        #endregion

        #region All posts
        public IQueryable<Posts> AllPosts()
        {
            var listPosts = db.Posts.AsNoTracking();
            return listPosts;
        }
        #endregion

        #region All posts by user ID
        public IQueryable<Posts> AllPostsByUserId(int userId)
        {
            var listPosts = db.Posts
                              .Where(p => p.UserId == userId)
                              .OrderByDescending(p => p.CreatedAt)
                              .AsNoTracking();
            return listPosts;
        }
        #endregion

        #region Get Post By Id
        public Posts GetPostById(int postId)
        {
            var post = db.Posts
                         .Include(u => u.User)
                         .Include(c => c.CategoryPost)
                         .FirstOrDefault(s => s.PostId == postId);
            return post;
        }
        #endregion

        #region Get Found Posts
        public IQueryable<Posts> GetFoundPosts()
        {
            var post = db.Posts.Include(u => u.User).Where(s => s.TypePost == TypePost.Found);
            return post;
        }
        #endregion

        #region Update post
        public async Task<bool> UpdatePost(Posts post)
        {
            return false;
        }
        #endregion

        #region Delete post
        public async Task<bool> DeletePost(int postId)
        {
            return false;
        }
        #endregion
    }
}
