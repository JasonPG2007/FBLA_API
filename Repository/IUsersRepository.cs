using ObjectBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IUsersRepository
    {
        public IQueryable<Users> AllUsers();
        public Task<bool> SignUp(Users user);
        public Task<Users> SignIn(int studentId, string password, string? email);
        public Users GetUserByStudentId(int studentId);
        public Users GetUserByEmail(string email);
    }
}
