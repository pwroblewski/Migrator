using Migrator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrator.Helpers
{
    public class UsersComparer : IEqualityComparer<Uzytkownik>
    {
        public bool Equals(Uzytkownik x, Uzytkownik y)
        {
            return x.IdSrtr == y.IdSrtr;
        }

        public int GetHashCode(Uzytkownik obj)
        {
            return (obj.IdSrtr).GetHashCode();
        }
    }
}
