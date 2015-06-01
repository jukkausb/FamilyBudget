using System.Collections.Generic;

namespace FamilyBudget.Www.Models
{
    public class MessageModel
    {
        public MessageModel()
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