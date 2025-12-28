using DataAccess;
using Microsoft.AspNetCore.Http;
using ObjectBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class UsersRepository : IUsersRepository
    {
        #region Variables
        private readonly UsersDAO usersDAO;
        private readonly StudentDAO studentDAO;
        #endregion

        #region Constructor
        public UsersRepository(UsersDAO usersDAO, StudentDAO studentDAO)
        {
            this.usersDAO = usersDAO;
            this.studentDAO = studentDAO;
        }
        #endregion

        #region GET All Users
        public IQueryable<Users> AllUsers()
        {
            var listUser = usersDAO.AllUsers();
            return listUser;
        }
        #endregion

        #region GET User By Student Id
        public Users GetUserByStudentId(int studentId)
        {
            var user = usersDAO.GetUserByStudentId(studentId);
            return user;
        }
        #endregion

        #region Get user By Email
        public Users GetUserByEmail(string email)
        {
            var user = usersDAO.GetUserByEmail(email);
            return user;
        }
        #endregion

        #region Sign Up
        public async Task<bool> SignUp(Users user)
        {
            // Hash password
            var hashPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(user.Password);

            user.Password = hashPassword;
            user.Role = Role.Student;

            var isAdded = await usersDAO.SignUp(user);
            return isAdded;
        }
        #endregion

        #region Sign In
        public async Task<Users> SignIn(int studentId, string password, string? email)
        {
            if (studentId == 0 && email != null)
            {
                var user = studentId > 0 ? usersDAO.GetUserByStudentId(studentId) : GetUserByEmail(email);
                if (user != null && BCrypt.Net.BCrypt.EnhancedVerify(password, user.Password))
                {
                    return user;
                }
                return null;
            }
            else
            {
                var user = GetUserByStudentId(studentId);
                if (user != null && BCrypt.Net.BCrypt.EnhancedVerify(password, user.Password))
                {
                    return user;
                }
                return null;
            }
        }
        #endregion
    }
}
