using System;
using Migrator.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrator.Model
{
    [Table("JednostkaMiary")]
    public class JednostkaMiary
    {
        [PrimaryKey, Unique]
        public string KodJmSrtr { get; set; }
        public string OpisJmSrtr { get; set; }
        public string KodJmZwsiron { get; set; }
        public string OpisJmZwsiron { get; set; }
    }
}
