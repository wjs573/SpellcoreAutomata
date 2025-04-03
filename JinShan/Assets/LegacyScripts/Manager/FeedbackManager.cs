using System.Collections.Generic;
using JinShan;
using MoreMountains.Feedbacks;
using UnityEngine;

public class FeedbackManager : MonoSingleton<FeedbackManager>
{
    private Dictionary<string, MMFeedback> FeedbacksDict;

    // Start is called before the first frame update
    void Start()
    {
        Transform[] children = transform.GetComponentsInChildren<Transform>();

        FeedbacksDict = new Dictionary<string, MMFeedback>();

        foreach (Transform child in children)
        {
            MMFeedback feedback = child.GetComponent<MMFeedback>();
            if (feedback != null)
            {
                FeedbacksDict.Add(child.name, feedback);
            }
        }
    }
    public void PlayFeedback(string _name, GameObject target = null)
    {
        if (FeedbacksDict.ContainsKey(_name))
        {
            MMFeedback feedback = FeedbacksDict[_name];
            switch (_name)
            {
                case "TransformTrembling":
                    feedback.GetComponent<MMFeedbackScale>().AnimateScaleTarget = target.transform;
                    feedback.GetComponent<MMFeedbackSquashAndStretch>().SquashAndStretchTarget = target.transform;
                    break;
                case "JumpLand":
                    feedback.GetComponent<MMFeedbackSquashAndStretch>().SquashAndStretchTarget = target.transform;
                    break;
                default:
                    break;
            }
            feedback.Play(Vector3.zero);
        }
    }
}
