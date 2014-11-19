using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Migrator.Helpers
{
    public class Message
    {
        public string MessageText { get; private set; }
        public object MessageObject { get; set; }

        public Message(string messageText, object messageObject = null)
        {
            MessageText = messageText;
            MessageObject = messageObject;
        }
    }
}
