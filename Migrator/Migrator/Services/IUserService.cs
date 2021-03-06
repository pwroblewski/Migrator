﻿using Migrator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrator.Services
{
    public interface IUserService
    {
        Task<List<Uzytkownik>> GetAllUsers();
        Task UpdateUser(Uzytkownik selectedUser);
    }
}
