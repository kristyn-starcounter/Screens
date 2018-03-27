using System;
using System.Collections.Generic;
using System.Linq;
using Starcounter;

namespace Screens
{
    static class HelperFunctions
    {
        static public IEnumerable<ScreenTempCode> GetAllScreenTempCodes(Screen screen)
        {
            return Db.SQL<ScreenTempCode>($"SELECT o FROM {typeof(ScreenTempCode)} o WHERE o.{nameof(ScreenTempCode.Screen)} = ? ORDER BY o.{nameof(ScreenTempCode.Expires)}", screen);
        }
    }
}
