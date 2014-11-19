using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrator.Services
{
    public interface IFileService<T>
    {
        string OpenFileDialog();
        List<T> GetAll(string path);
        void Clean();
    }
}
