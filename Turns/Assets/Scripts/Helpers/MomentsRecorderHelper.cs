using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gamelogic.Extensions;
using Moments;
using Moments.Encoder;
using UnityEngine;
using UnityEngine.UI;

public class MomentsRecorderHelper : Singleton<MomentsRecorderHelper>
{
    public RawImage replayQuad;
    public Recorder recorder;

    List<RenderTexture> history;
    int showFrame = 0;


    void Playback()
    {
        if (history.Count > 0)
        {
            replayQuad.texture = history[showFrame];
            showFrame = (++showFrame) % history.Count;
        }

    }



    public void CaptureReplay()
    {
        recorder.Pause();
        var m_Frames = recorder.Frames;
        history = m_Frames.ToList();
        
    }

    public void StopReplay()
    {
        CancelInvoke(nameof(Playback));
    }

    public void StartPlayback()
    {
        showFrame = 0;
        InvokeRepeating(nameof(Playback),0, 1f / recorder.m_FramePerSecond);
    }
    

    public void ResetRecording()
    {
        recorder.Pause();
        recorder.FlushMemory();
        recorder.Record();
        CancelInvoke(nameof(Playback));
    }
}
