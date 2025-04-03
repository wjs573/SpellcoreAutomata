using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using JinShan;

public class UnitFeedback : MonoBehaviour
{
    Dictionary<string, MMFeedbacks> FeedbacksDict;
    public MMFeedbacks damageFeedbacks;

    public void PlayDamageFeedbacks()
    {
        FeedbacksDict = new Dictionary<string, MMFeedbacks>();
        damageFeedbacks.PlayFeedbacks();
    }

    /// <summary>
    /// 播放角色身上的feedback
    /// </summary>
    /// <param name="name"></param>
    public void play(string name)
    {
        if (FeedbacksDict.ContainsKey(name))
        {
            FeedbacksDict[name].PlayFeedbacks();
        }
        else
        {
            Transform feedback = TransformerHelper.FindChildByName(transform, name);
            if (feedback != null)
            {
                MMFeedbacks TargetFeedback = feedback.GetComponent<MMFeedbacks>();
                TargetFeedback.PlayFeedbacks();
                FeedbacksDict.Add(name, TargetFeedback);
            }
        }
    }

    public void Init()
    {
        MMBlink targetBlink = GetComponentInChildren<ViewContainer>().transform.GetChild(0).GetComponent<MMBlink>();
        if (targetBlink != null)
        {
            damageFeedbacks.GetComponent<MMFeedbackBlink>().TargetBlink = targetBlink;
        }
        else
        {
            damageFeedbacks.GetComponent<MMFeedbackBlink>().Active = false;
        }
    }
}
