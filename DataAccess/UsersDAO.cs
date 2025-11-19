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
        public bool SignUp(Users user)
        {
            return false;
        }
        #endregion

        #region All users
        public IQueryable<Users> AllUsers()
        {
            var listUsers = db.Users;
            return listUsers;
        }
        #endregion

        #region Update user
        public bool UpdateUser(Users user)
        {
            return false;
        }
        #endregion

        #region Delete user
        public bool DeleteUser(int userId)
        {
            return false;
        }
        #endregion
    }
}
