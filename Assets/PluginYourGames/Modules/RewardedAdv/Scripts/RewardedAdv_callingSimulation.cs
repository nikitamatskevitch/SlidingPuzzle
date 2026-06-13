#if UNITY_EDITOR
using System.Collections;
using UnityEngine;

namespace YG.Insides
{
    public partial class AdvCallingSimulation
    {
        public static void RewardedAdvOpen(string id, float duration)
        {
            AdvCallingSimulation call = CreateCallSimulation();
            call.StartCoroutine(call.RewardedAdvOpenCoroutine(id, duration));
        }

        private IEnumerator RewardedAdvOpenCoroutine(string id, float duration)
        {
            yield return new WaitForSecondsRealtime(YG2.infoYG.Simulation.loadAdv);
            YGInsides.OpenRewardedAdv();
            DrawScreen(new Color(0, 1, 0, 0.5f));
            yield return new WaitForSecondsRealtime(duration);
            YGInsides.RewardAdv(id);
            YGInsides.CloseRewardedAdv();
            Destroy(gameObject);
        }
    }
}
#endif
