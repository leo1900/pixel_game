using System;
namespace DragonU3DSDK
{
    interface IPrivacyResponsor
    {
        void OnPrivacyAccepted(string message);
#if UNITY_IOS && !UNITY_EDITOR
        void OnPrivacyRefused(string message);
        void LaterOneDay();
        void LaterOneMonth();
        void RateUsNow();
#endif
    }
}
