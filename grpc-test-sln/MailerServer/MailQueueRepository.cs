using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailerServer
{
    public class MailQueueRepository
    {
        private readonly ConcurrentDictionary<string, MailQueue> _mailQueues = new();

        public MailQueue GetMailQueue(string name)
        {
            return _mailQueues.GetOrAdd(name, (n) => new MailQueue(n));
        }
    }
}
