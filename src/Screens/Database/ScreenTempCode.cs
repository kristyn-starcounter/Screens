using System;
using Starcounter;

namespace Screens
{
    [Database]
    public class ScreenTempCode
    {
        public string Code { get; set; }
        public DateTime Expires { get; set; }
        public Screen Screen { get; set; }
    }
}
