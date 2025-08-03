using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es2al.Services.Events.CustomEventArgs
{
    public class FollowingEventArgs : EventArgs
    {
        public int UserId { get; set; }
    }
}
