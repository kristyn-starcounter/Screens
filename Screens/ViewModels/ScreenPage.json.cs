using Starcounter;
using System;
using System.Collections.Generic;
using Screens.Common;

namespace Screens.ViewModels
{
    partial class ScreenPage : Json, IBound<Screen>
    {

        protected override void OnData()
        {
            base.OnData();
            if (this.Data != null)
            {
                this.SelectedTimeZoneId = this.Data.TimeZoneId;
            }
        }

        public void Init()
        {
            this.PluginsContent = Self.GET("/Screens/screenpluginmapping/"+this.Data.GetObjectID());
        }

        public QueryResultRows<ScreenTempCode> ScreenCodes => Db.SQL<ScreenTempCode>("SELECT o FROM Screens.Common.ScreenTempCode o WHERE o.Screen = ? ORDER BY o.Expires", this.Data);


        public IEnumerable<TimeZoneInfo> TimeZones => TimeZoneInfo.GetSystemTimeZones();

        public string SelectedTimeZoneId {
            get {

                if (this.Data == null || string.IsNullOrEmpty(this.Data.TimeZoneId))
                {
                    return TimeZoneInfo.Local.Id;
                }

                return this.Data.TimeZoneId;
            }
            set {
                this.Data.TimeZoneId = value;
            }
        }
         

        public string UrlString {
            get {
                return string.Format("/Screens/screens/{0}", this.Data?.GetObjectID());
            }
        }

        public void Handle(Input.GenerateScreenCodeTrigger action)
        {

            Db.Transact(() =>
            {
                ScreenTempCode screenCode = new ScreenTempCode();
                screenCode.Code = GenerateRandomScreenCode();
                screenCode.Screen = this.Data;
                screenCode.Expires = DateTime.UtcNow.AddHours(1); // TODO: Expire time 1 hour?
            });
        }

        public void Handle(Input.SaveTrigger action)
        {
            this.Transaction.Commit();
            this.RedirectUrl = "/Screens/screens";
        }

        public void Handle(Input.CloseTrigger action)
        {
            this.Transaction.Rollback();
            this.RedirectUrl = "/Screens/screens";
        }

        public string GenerateRandomScreenCode()
        {
            int min = 1000;
            int max = 9999;
            Random rnd = new Random();
            return rnd.Next(min, max).ToString().PadLeft(4,'0');
        }
    }

    [ScreenPage_json.ScreenCodes]
    partial class ScreenPageScreenTempoCode : Json, IBound<ScreenTempCode>
    {

        public void Handle(Input.DeleteTrigger action)
        {

            Db.Transact(() =>
            {
                this.Data.Delete();
            });
        }
    }

}
