using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Migrator.Model.State
{
    public class ZestawienieState
    {
        public string ViewName { get; set; }
        public List<Zestawienie> Zestawienia { get; set; }
        public List<ZestawienieKlas> ZestawieniaKlas { get; set; }
    }
}
