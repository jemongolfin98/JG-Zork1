using System;

namespace Zork
{
    public interface IOutputService
    {
        void WriteLine(string value, bool isBold = false);

        void WriteLine(object value, bool isBold = false);

        void Write(string value);

        void Write(object value);
    }
}
