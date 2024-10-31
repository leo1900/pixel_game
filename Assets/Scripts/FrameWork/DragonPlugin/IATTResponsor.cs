using System;
namespace DragonU3DSDK
{
    interface IATTResponsor
    {
#if UNITY_IOS && !UNITY_EDITOR
        void OnATTAccepted(string message);
        void OnATTRefused(string message);
#endif
    }
}
