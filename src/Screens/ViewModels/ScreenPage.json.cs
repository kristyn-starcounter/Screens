using Starcounter;
using System;
using System.Collections.Generic;

namespace Screens.ViewModels
{
    partial class ScreenPage : Json, IBound<UserScreenRelation>
    {

        protected override void OnData()
        {
            // This "workaround" is needed see github issue
            // https://github.com/Starcounter/Home/issues/316

            if (this.Data == null && !string.IsNullOrEmpty(this.MorphUrl))
            {
                this.MorphUrl = this.MorphUrl;
            }
            base.OnData();
        }


        public void Init()
        {
            this.PluginsContent = Self.GET("/Screens/screenpluginmapping/" + this.Data?.Screen?.GetObjectID());
        }

        public IEnumerable<ScreenTempCode> ScreenCodes => HelperFunctions.GetAllScreenTempCodes(this.Data?.Screen);
       

        public void Handle(Input.GenerateScreenCodeTrigger action)
        {
            ScreenTempCode screenCode = new ScreenTempCode();
            screenCode.Code = GenerateRandomScreenCode();
            screenCode.Screen = this.Data?.Screen;
            screenCode.Expires = DateTime.UtcNow.AddHours(1); // TODO: Expire time 1 hour?
        }

        public void Handle(Input.SaveTrigger action)
        {
            this.Transaction.Commit();
            this.MorphUrl = "/Screens/screens";
        }

        public void Handle(Input.CloseTrigger action)
        {
            if (this.Transaction.IsDirty)
            {
                this.Transaction.Rollback();
            }
            this.MorphUrl = "/Screens/screens";
        }

        public void Handle(Input.DeleteTrigger action)
        {
            MessageBoxButton deleteButton = new MessageBoxButton() { ID = (long)MessageBox.MessageBoxResult.Yes, Text = "Delete", CssClass = "btn btn-sm btn-danger" };
            MessageBoxButton cancelButton = new MessageBoxButton() { ID = (long)MessageBox.MessageBoxResult.Cancel, Text = "Cancel" };

            MessageBox.Show("Delete Screen", "This Screen will be deleted.", cancelButton, deleteButton, (result) =>
            {
                if (result == MessageBox.MessageBoxResult.Yes)
                {

                    // TODO: how to check that this.Data can be deleted (if it has not been commited?)
                    this.Data.Delete();
                    if (this.Transaction.IsDirty)
                    {
                        this.Transaction.Commit();
                    }

                    this.MorphUrl = "/Screens/screens";  // TODO: This does not work!. (maybe of some commit-hooks activity?)
                }
            });
        }

        public string GenerateRandomScreenCode()
        {
            int min = 1000;
            int max = 9999;
            Random rnd = new Random();
            return rnd.Next(min, max).ToString().PadLeft(4, '0');
        }
    }

    [ScreenPage_json.ScreenCodes]
    partial class ScreenPageScreenTempoCode : Json, IBound<ScreenTempCode>
    {
        public void Handle(Input.DeleteTrigger action)
        {
            //This also does not work if the screen hasn't been saved!
            Db.Transact(() => this.Data?.Delete());
        }
    }

}
