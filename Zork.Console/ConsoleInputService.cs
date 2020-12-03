using System;

namespace Zork
{
    internal class ConsoleInputService : IInputService
    {
        public event EventHandler<string> InputReceived;

        public void ProcessInput()
        {
            string inputString = Console.ReadLine();
            if (InputReceived != null)
            {
                InputReceived.Invoke(this, inputString);
            }
        }
    }
}
