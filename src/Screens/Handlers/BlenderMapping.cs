using Starcounter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Screens
{
    static class BlenderMapping
    {
        public static void Register()
        {
            Blender.MapUri("/Screens/screenpluginmapping/{?}", "screen");
            Blender.MapUri("/Screens/menu", "menu");
        }
    }
}