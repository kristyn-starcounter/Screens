using Starcounter;

namespace Screens.ViewModels
{
    partial class Menu : Json
    {

        public void Init()
        {
            //var item = this.Items.Add();
            //item.Name = "Home";
            //item.Url = "/Screens";
            var item2 = this.Items.Add();
            item2.Name = "Screens";
            item2.Url = "/Screens/Screens";

        }

    }
}
