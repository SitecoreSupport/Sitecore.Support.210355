using Sitecore.Analytics;
using Sitecore.Analytics.Configuration;
using Sitecore.Analytics.Core;
using Sitecore.Analytics.RobotDetection;
using Sitecore.Analytics.Web;
using Sitecore.Framework.Conditions;
using Sitecore.Xdb.Configuration;
using System;

namespace Sitecore.Support.Analytics.RobotDetection.layouts.system
{
    public class VisitorIdentificationCss : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            Condition.Requires(e, "e").IsNotNull();
            base.OnInit(e);
            base.Response.ContentType = "text/css";
            if (XdbSettings.Tracking.Enabled && Tracker.Current.Sampling.IsSampling() && Tracker.IsActive)
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
