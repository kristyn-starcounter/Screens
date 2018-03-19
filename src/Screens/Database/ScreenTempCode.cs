using System;
using Starcounter;

namespace Screens
{
    [Database]
    public class ScreenTempCode
    {
        public string Code;
        public DateTime Expires;
        public Screen Screen;
    }
}
