using GalaSoft.MvvmLight;
using Migrator.Helpers;

namespace Migrator.Model
{
    [Table("Jednostka")]
    public class Jednostka : ObservableObject
    {
        [PrimaryKey, Unique]
        public string KodJednostki { get; set; }
        public string NazwaJednostki { get; set; }
        public string KontoJednostki { get; set; }
        public string TypJednostki { get; set; }
        public string OsobaUpowazniona { get; set; }
        public string Telefax { get; set; }
        public string Zaklad { get; set; }
        public string Sklad { get; set; }

        public Jednostka()
        {

        }
    }
}
