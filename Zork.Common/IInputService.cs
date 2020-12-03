using System;

namespace Zork
{
    public interface IInputService
    {
        event EventHandler<string> InputReceived;
    }
}
