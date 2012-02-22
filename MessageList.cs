using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkeletalTracking
{
    public class MessageList
    {
        List<Message> messages = new List<Message>();

        public void pushMessage(Message m)
        {
            this.messages.Add(m);
        }

        public void pushMessage(double time, String speaker, String text, String imgFile)
        {
            this.pushMessage(new Message(time, speaker,text, imgFile));
        }

        public List<Message> removeOverdueMessages(double time)
        {
            List<Message> overdueMessages = new List<Message>();
            foreach (Message message in this.messages)
            {
                if (message.time > time)
                {
                    overdueMessages.Add(message);
                }
            }
            foreach (Message message in overdueMessages)
            {
                this.messages.Remove(message);
            }
            return overdueMessages;
        }

 
    }
}
