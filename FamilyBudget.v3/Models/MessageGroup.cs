using System.Collections.Generic;

namespace FamilyBudget.v3.Models
{
    public class MessageGroup
    {
        public string Name { get; set; }

        public MessageGroup()
        {
            Messages = new List<Message>();
        }

        public bool HasMessages
        {
            get { return Messages.Count > 0; }
        }

        public List<Message> Messages { get; set; }
    }
}