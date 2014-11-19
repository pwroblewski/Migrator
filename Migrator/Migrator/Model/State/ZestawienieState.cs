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
        public ObservableCollection<Zestawienie> ListZestawienie { get; set; }
        public List<Zestawienie> ListZestService { get; set; }
        public List<ZestawienieKlas> ListZestKlasService { get; set; }
    }
}
