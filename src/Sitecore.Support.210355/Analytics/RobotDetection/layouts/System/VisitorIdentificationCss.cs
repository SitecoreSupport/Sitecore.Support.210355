using Sitecore.Analytics;
using Sitecore.Analytics.Configuration;
using Sitecore.Analytics.Core;
using Sitecore.Analytics.RobotDetection;
using Sitecore.Analytics.Web;
using Sitecore.Diagnostics;
using Sitecore.Xdb.Configuration;
using System;
using System.Web.UI;

namespace Sitecore.Support.Analytics.RobotDetection.layouts.system
{
    public class VisitorIdentificationCss : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");
            base.OnInit(e);
            base.Response.ContentType = "text/css";
            #region - changed order of if clauses
            if (XdbSettings.Tracking.Enabled && Tracker.IsActive && Tracker.Current.Sampling.IsSampling())
            #endregion
            {
                Tracker.Current.CurrentPage.Cancel();
                if (Tracker.Current.Interaction.PageCount != 1 && AnalyticsSettings.Robots.AutoDetect)
                {
                    UpdateVisitorIdentification();
                    SessionUtil.ResetSessionTimeout();
                }
            }
        }

        private void UpdateVisitorIdentification()
        {
            ContactKeyCookie contactKeyCookie = new ContactKeyCookie();
            if (!contactKeyCookie.IsClassificationGuessed)
            {
                contactKeyCookie.IsClassificationGuessed = true;
                contactKeyCookie.Update();
                if (Tracker.Current.Session != null && ContactClassification.IsAutoDetectedRobot(Tracker.Current.Session.Contact.System.Classification))
                {
                    Tracker.Current.Session.SetClassification(0, 0, isContactClassificationGuessed: true);
                }
            }
        }
    }
}