using Microsoft.EntityFrameworkCore;
using ObjectBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class UsersDAO
    {
        #region Variables
        private readonly FBLADbContext db;
        #endregion

        #region Constructor
        public UsersDAO(FBLADbContext db)
        {
            this.db = db;
        }
        #endregion

        #region Create user (Sign up)
        public async Task<bool> SignUp(Users user)
        {
            user.UserId = new Random().Next();
            var isAdded = db.Users.Add(user);
            if (isAdded != null)
            {
                await db.SaveChangesAsync();
                return true;
            }
            return false;
        }
        #endregion

        #region All users
        public IQueryable<Users> AllUsers()
        {
            var listUsers = db.Users.AsNoTracking();
            return listUsers;
        }
        #endregion

        #region Get User By Student Id
        public Users GetUserByStudentId(int studentId)
        {
            var user = db.Users
                         .Include(u => u.Student)
                         .FirstOrDefault(s => s.Student.StudentId == studentId);
            return user;
        }
        #endregion

        #region Get User By Email
        public Users GetUserByEmail(string email)
        {
            var user = db.Users.Include(s => s.Student).FirstOrDefault(s => s.Email.Equals(email));
            return user;
        }
        #endregion

        #region Update user
        public async Task<bool> UpdateUser(Users user)
        {
            return false;
        }
        #endregion

        #region Delete user
        public async Task<bool> DeleteUser(int userId)
        {
            return false;
        }
        #endregion
    }
}
