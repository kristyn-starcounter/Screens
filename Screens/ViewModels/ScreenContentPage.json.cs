using Screens.Common;
using Starcounter;

namespace Screens.ViewModels
{
    partial class ScreenContentPage : Json
    {
        public void Init(Screen screen)
        {
            this.Content = Self.GET("/Screens/screenContent/" + screen.GetObjectID());
        }


    }
}
