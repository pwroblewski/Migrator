using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrator.Services
{
    public class UserService : IUserService
    {
        public async Task<List<Model.Uzytkownik>> GetAllUsers()
        {
            return await App.Connection.Table<Model.Uzytkownik>().ToListAsync();
        }

        public async Task UpdateUser(Model.Uzytkownik selectedUser)
        {
            await App.Connection.UpdateAsync(selectedUser);
        }
    }
}
