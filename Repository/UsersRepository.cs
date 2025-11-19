using DataAccess;
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
        #endregion

        #region Constructor
        public UsersRepository(UsersDAO usersDAO)
        {
            this.usersDAO = usersDAO;
        }
        #endregion

        public IQueryable<Users> AllUsers()
        {
            var listUser = usersDAO.AllUsers();
            return listUser;
        }
    }
}
