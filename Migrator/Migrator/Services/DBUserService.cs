using Migrator.Helpers;
using Migrator.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace Migrator.Services
{
    public class DBUserService : IDBUserService
    {
        // wyszukuje wczytane z pliku dane w bazie
        // jeżeli nie ma w bazie informacji o użytkowniku to ją dodaje
        public async Task<List<Uzytkownik>> SyncUsersData(List<Uzytkownik> listUzytkownicy)
        {
            List<Uzytkownik> _listUzytkownicy = new List<Uzytkownik>();

            foreach (Uzytkownik uzyt in listUzytkownicy)
            {
                var q = from f in App.Connection.Table<Uzytkownik>()
                        where f.IdSrtr == uzyt.IdSrtr
                        select f;
                var user = q.FirstOrDefaultAsync();

                if (user.Result != null)
                {
                    _listUzytkownicy.Add(await q.FirstAsync());
                }
                else
                {
                    _listUzytkownicy.Add(uzyt);
                    await App.Connection.InsertAsync(uzyt);
                }
            }

            return _listUzytkownicy;
        }

        public async Task SyncFileUserData(List<Uzytkownik> listUzytkownicy)
        {
            foreach (Uzytkownik uzyt in listUzytkownicy)
            {
                var q = from f in App.Connection.Table<Uzytkownik>()
                        where f.IdSrtr == uzyt.IdSrtr
                        select f;
                var user = await q.FirstOrDefaultAsync();

                try
                {
                    if (user != null)
                    {
                        user = ExtensionMethods.MergeWith<Uzytkownik>(user, uzyt);
                        await App.Connection.UpdateAsync(user);
                    }
                    else
                    {
                        await App.Connection.InsertAsync(uzyt);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public async Task<List<Uzytkownik>> GetAll()
        {
            return await App.Connection.Table<Uzytkownik>().ToListAsync();
        }

        public async Task Update(Uzytkownik selectedUser)
        {
            await App.Connection.UpdateAsync(selectedUser);
        }


        
    }
}
