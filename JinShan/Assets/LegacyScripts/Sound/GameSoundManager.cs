using System.Collections;
using System.Collections.Generic;
using JinShan;
using MoreMountains.Tools;
using UnityEngine;

public class GameSoundManager : MonoSingleton<GameSoundManager>
{
    public SoundDatabase soundDatabase;

    public Dictionary<string, AudioSource> AudioSourceDict = new Dictionary<string, AudioSource>();

    public void PlaySoundOneTimes(string clipName, MMSoundManager.MMSoundManagerTracks track = MMSoundManager.MMSoundManagerTracks.UI)
    {
        foreach (SoundClip soundClip in soundDatabase.data)
        {
            if (soundClip.id == clipName)
            {
                MMSoundManagerSoundPlayEvent.Trigger(soundClip.AudioClip, track, this.transform.position);
            }
        }
    }

    public void PlaySound(string clipName, bool IsLoop = false)
    {
        if (AudioSourceDict.ContainsKey(clipName))
        {
            MMSoundManagerSoundControlEvent.Trigger(MMSoundManagerSoundControlEventTypes.Resume, 0, AudioSourceDict[clipName]);
            return;
        }

        foreach (SoundClip soundClip in soundDatabase.data)
        {
            if (soundClip.id == clipName)
            {
                AudioSourceDict.Add(clipName,
                MMSoundManagerSoundPlayEvent.Trigger(soundClip.AudioClip,
                    MMSoundManager.MMSoundManagerTracks.UI, this.transform.position, IsLoop));
            }
        }
    }

    public void StopSound(string clipName)
    {
        if (AudioSourceDict.ContainsKey(clipName))
        {
            MMSoundManagerSoundControlEvent.Trigger(MMSoundManagerSoundControlEventTypes.Pause, 0, AudioSourceDict[clipName]);
        }
    }
}
