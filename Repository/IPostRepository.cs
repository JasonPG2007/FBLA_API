using ObjectBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IPostRepository
    {
        public Task<bool> CreatePost(Posts post);
        public IQueryable<Posts> GetFoundPosts();
        public Posts GetPostById(int postId);
        public IQueryable<Posts> AllPostsByUserId(int userId);
    }
}
