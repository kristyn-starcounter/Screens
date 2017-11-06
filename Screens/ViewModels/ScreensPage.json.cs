using Starcounter;
using System.Collections.Generic;
using Screens.Common;

namespace Screens.ViewModels
{
    partial class ScreensPage : Json
    {
        public IEnumerable<Screen> Screens => Db.SQL<Screen>("SELECT o.Screen FROM Screens.Common.UserScreenRelation o WHERE o.User = ? ORDER BY o.Screen.Name", UserSession.GetSignedInUser());
    }

    [ScreensPage_json.Screens]
    partial class ScreensPageRoomItem : Json, IBound<Screen>
    {

        public string Url => string.Format("/Screens/screens/{0}", this.Data?.GetObjectID());







    }
}
