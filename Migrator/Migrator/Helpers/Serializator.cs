using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Migrator.Helpers
{
    [Serializable]
    class Serializator : ISerializable
    {
        public DataTable dt { get; set; }

        public Serializator()
        {
        }

        public Serializator(SerializationInfo info, StreamingContext ctxt)
        {
            dt = (DataTable)info.GetValue("Kartoteka", typeof(DataTable));
        }


        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Kartoteka", this.dt);
        }
    }
}
