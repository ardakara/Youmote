using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkeletalTracking
{
    public class MessageList
    {
        List<Message> runningMessages = new List<Message>();
        List<Message> queuedMessages = new List<Message>();

        public void pushMessage(Message m)
        {
            this.queuedMessages.Add(m);
        }

        public void pushMessage(double time, double duration, String speaker, String text, String imgFile)
        {
            this.pushMessage(new Message(time, duration, speaker, text, imgFile));
        }

        public void Clear()
        {
            this.queuedMessages.Clear();
            this.runningMessages.Clear();
        }



        public List<Message> popReadyMessages(double time)
        {
            List<Message> readyMessages = new List<Message>();
            foreach (Message message in this.queuedMessages)
            {
                if (message.time < time)
                {
                    readyMessages.Add(message);
                }
            }
            foreach (Message message in readyMessages)
            {
                this.queuedMessages.Remove(message);
                this.runningMessages.Add(message);
            }
            return readyMessages;
        }

        public List<Message> popFinishedMessages()
        {
            List<Message> finishedMessages = new List<Message>();
            foreach (Message message in this.runningMessages)
            {
                if (message.isFinished())
                {
                    finishedMessages.Add(message);
                }
            }
            foreach (Message message in finishedMessages)
            {
                this.runningMessages.Remove(message);
            }
            return finishedMessages;
        }



    }
}
