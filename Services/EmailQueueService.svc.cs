using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using ExpoOrders.Controllers;
using ExpoOrders.Common;
using ExpoOrders.Entities;

namespace ExpoOrders.Web.Services
{
    
    public class EmailQueueService : IEmailQueueService
    {
        public int ProcessEmailQueue()
        {
            EmailController processor = new EmailController();
            return processor.ProcessEmailQueue();
        }
    }
}
