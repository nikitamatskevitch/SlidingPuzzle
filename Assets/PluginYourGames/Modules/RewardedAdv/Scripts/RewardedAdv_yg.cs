using System;
using YG.Insides;
using UnityEngine;

namespace YG
{
    public static partial class YG2
    {
        public static Action onOpenRewardedAdv;
        public static Action onCloseRewardedAdv;
        public static Action<string> onRewardAdv;
        public static Action onErrorRewardedAdv;

        [InitYG]
        private static void InitRewardedAdv()
        {
#if UNITY_EDITOR
            onOpenRewardedAdv = null;
            onCloseRewardedAdv = null;
            onRewardAdv = null;
            onErrorRewardedAdv = null;
#endif
        }

        public static void RewardedAdvShow(string id)
        {
#if UNITY_EDITOR
            if (infoYG.Simulation.testFailAds)
            {
                Message("Error Rewarded Adv simulation");
                YGInsides.ErrorRewardedAdv();
                return;
            }

            AdvCallingSimulation.RewardedAdvOpen(id, infoYG.Simulation.durationAdv);
#else
            if (nowAdsShow)
            {
#if RU_YG2
                Message("Реклама уже открыта");
#else
                Message("The advertisement is already open");
#endif
                return;
            }

            Message("Rewarded Adv");
            iPlatform.RewardedAdvShow(id);
#endif
        }
    }
}

namespace YG.Insides
{
    public static partial class YGInsides
    {
        public static void OpenRewardedAdv()
        {
            YG2.PauseGame(true);
            YG2.onOpenRewardedAdv?.Invoke();
            YG2.onOpenAnyAdv?.Invoke();
            YG2.nowRewardAdv = true;
        }

        public static void CloseRewardedAdv()
        {
            YG2.nowRewardAdv = false;
            YG2.onCloseRewardedAdv?.Invoke();
            YG2.onCloseAnyAdv?.Invoke();
            YG2.PauseGame(false);
        }

        public static void RewardAdv(string id)
        {
            YG2.onRewardAdv?.Invoke(id);
        }

        public static void ErrorRewardedAdv()
        {
            YG2.nowRewardAdv = false;
            YG2.onErrorRewardedAdv?.Invoke();
            YG2.onErrorAnyAdv?.Invoke();
            YG2.PauseGame(false);
        }
    }
}
