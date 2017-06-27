using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Task_manager.Hubs
{
    [HubName("tasksHub")]
    public class TasksHub : Hub
    {
        public static void Show()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<TasksHub>();
            context.Clients.All.updatedData();

        }

    }
}