using Screens.Common;
using Starcounter;
using System;

namespace Screens.ViewModels
{
    partial class ScreenContentPage : Json, IBound<Screen>
    {


        //public Json Content => GetContents();

        //public Json GetContents()
        //{
        //    return Self.GET("/Screens/screenContent/" + this.Data.GetObjectID());
        //}


        public void Init(Screen screen)
        {
            this.Content = Self.GET("/Screens/screenContent/" + this.Data.GetObjectID());
        }


    }
}
