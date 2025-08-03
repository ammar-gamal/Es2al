using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es2al.Services.Events
{
    public class AsyncEventHandlers
    {
        public delegate Task EventHandlerAsync(object? sender, EventArgs e);
        public delegate Task EventHandlerAsync<TEventArgs>(object? sender, TEventArgs e) where TEventArgs : EventArgs;
    }
}
