using Screens.Common;
using Starcounter;
using System;

namespace Screens.ViewModels
{
    partial class ScreenContentPage : Json, IBound<Screen>
    {
        public void Init(Screen screen)
        {
            this.Content = Self.GET("/Screens/screenContent/" + this.Data.GetObjectID());
        }
    }

}
