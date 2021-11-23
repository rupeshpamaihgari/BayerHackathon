using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class VumarkHandler_Bayer : DefaultObserverEventHandler
{
    // Start is called before the first frame update

    public VuMarkBehaviour vumarkBehaviour;

    public event Action<VuMarkBehaviour> OnVuMarkFound;
    public event Action<VuMarkBehaviour> OnVuMarkLost;

    void Start()
    {
       
    }

    protected override void OnTrackingFound()
    {
        var vuMarkBehaviour = mObserverBehaviour as VuMarkBehaviour;
        Debug.Log("<color=cyan>VuMark ID Tracked: </color>" + vuMarkBehaviour.InstanceId);



        OnVuMarkFound?.Invoke(vuMarkBehaviour);
    }

    protected override void OnTrackingLost()
    {
        // Ignore the initial callback triggered by Start()
        var vuMarkBehaviour = mObserverBehaviour as VuMarkBehaviour;
        if (vuMarkBehaviour.InstanceId == null)
            return;

        Debug.Log("<color=cyan>VuMark ID lost: </color>" + vuMarkBehaviour.InstanceId);



        OnVuMarkLost?.Invoke(vuMarkBehaviour);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
