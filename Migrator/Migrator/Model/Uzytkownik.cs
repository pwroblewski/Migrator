using GalaSoft.MvvmLight;
using Migrator.Helpers;

namespace Migrator.Model
{
    [Table("Uzytkownik")]
    public class Uzytkownik : ObservableObject
    {
        [PrimaryKey, Unique]
        public string IdSrtr { get; set; }
        public string NazwaUzytkownika { get; set; }
        public string KodJednostki { get; set; }
        public string TypUzytkownika { get; set; }
        public string OsobaUpowazniona { get; set; }
        public string Poczta { get; set; }
        public string Ulica { get; set; }
        public string Telefon { get; set; }
        public string Telefax { get; set; }
        public string Mpk { get; set; }
        public string IdZwsiron { get; set; }

        public Uzytkownik()
        {

        }

        //public override bool Equals(object obj)
        //{
        //    return (obj as Uzytkownik).IdSrtr == IdSrtr;
        //}
    }
}
