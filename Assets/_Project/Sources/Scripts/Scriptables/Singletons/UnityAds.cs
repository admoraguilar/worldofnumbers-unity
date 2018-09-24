using UnityEngine;
using UnityEngine.Advertisements;
using System;
using System.Collections;
using System.Collections.Generic;


[CreateAssetMenu(menuName = "Scriptables/Singleton/UnityAds")]
public class UnityAds : AAdsScriptable {
    public override void Show() {
        
    }

    public override void Show(Action<ShowResult> OnResult) {
        if(OnResult == null) { OnResult = OnHandleAdResult;
        } else { OnResult += OnHandleAdResult; }

        Debugger.Log(DebuggerType.Log, "Trying to show ad");
        if(Advertisement.IsReady("rewardedVideo")) {
            var options = new ShowOptions { resultCallback = OnResult };
            Advertisement.Show("rewardedVideo", options);
            Debugger.Log(DebuggerType.Log, "Ad shown");
        }
    }

    private void OnHandleAdResult(ShowResult result) {
        switch(result) {
            case ShowResult.Finished: _OnFinished(); break;
            case ShowResult.Skipped: _OnSkipped(); break;
            case ShowResult.Failed: _OnFailed(); break;
        }
    }
}


public abstract class AAdsScriptable : CustomScriptableObject {
    public event Action OnFinished = delegate { };
    public event Action OnSkipped = delegate { };
    public event Action OnFailed = delegate { };

    protected void _OnFinished() { OnFinished(); }
    protected void _OnSkipped() { OnSkipped(); }
    protected void _OnFailed() { OnFailed(); }


    public abstract void Show();
    public abstract void Show(Action<ShowResult> OnResult);
}