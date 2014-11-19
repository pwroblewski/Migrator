using GalaSoft.MvvmLight;
using Migrator.Helpers;

namespace Migrator.Model
{
    [Table("Magazyn")]
    public class Magazyn : ObservableObject
    {
        [PrimaryKey, Unique]
        public string NrMagazynu { get; set; }
        public string NazwaMagazynu { get; set; }
        public string Zaklad { get; set; }
        public string Sklad { get; set; }

        public Magazyn()
        {

        }
    }
}
