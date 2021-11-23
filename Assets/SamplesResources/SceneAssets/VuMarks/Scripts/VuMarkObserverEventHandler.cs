/*===============================================================================
Copyright (c) 2021 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.
===============================================================================*/

using System;
using UnityEngine;
using Vuforia;

public class VuMarkObserverEventHandler : DefaultObserverEventHandler
{
    public event Action<VuMarkBehaviour> OnVuMarkFound;
    public event Action<VuMarkBehaviour> OnVuMarkLost;
    
    const int UPDATE_FRAME_COUNT = 15;
    const int PERSISTENT_NUMBER_OF_CHILDREN = 2;  // Persistent Children: 1. Canvas, 2. LineRenderer
    
    LineRenderer mLineRenderer;
    CanvasGroup mCanvasGroup;
    Vector2 mFadeRange;
    Transform mCentralAnchorPointTransform;
    
    protected override void Start()
    {
        base.Start();
        mCanvasGroup = GetComponentInChildren<CanvasGroup>();
        mFadeRange = VuforiaRuntimeUtilities.IsPlayMode() ? new Vector2(0.5f, 0.6f) : new Vector2(0.9f, 1.0f);
        
        VuforiaApplication.Instance.OnVuforiaStarted += OnVuforiaStarted;
    }

    public void AssignLineRenderer(LineRenderer lineRenderer)
    {
        mLineRenderer = lineRenderer;
        mLineRenderer.enabled = ShouldBeRendered(mPreviousTargetStatus.Status);
    }

    // Override, but don't implement these base methods,
    // since VuMark gameobjects are automatically disabled
    protected override void OnTrackingFound()
    {
        var vuMarkBehaviour = mObserverBehaviour as VuMarkBehaviour;
        Debug.Log("<color=cyan>VuMark ID Tracked: </color>" + vuMarkBehaviour.InstanceId);

        //for (int i = 1; i < this.transform.childCount; i++)
        //{
        //    this.transform.GetChild(i).gameObject.SetActive(true);
        //}


        OnVuMarkFound?.Invoke(vuMarkBehaviour);
    }

    protected override void OnTrackingLost()
    {
        // Ignore the initial callback triggered by Start()
        var vuMarkBehaviour = mObserverBehaviour as VuMarkBehaviour;
        if (vuMarkBehaviour.InstanceId == null)
            return;
        
        Debug.Log("<color=cyan>VuMark ID lost: </color>" + vuMarkBehaviour.InstanceId);

        //for(int i = 1; i < this.transform.childCount; i++)
        //{
        //    this.transform.GetChild(i).gameObject.SetActive(false);
        //}
        
        OnVuMarkLost?.Invoke(vuMarkBehaviour);
    }

    void OnVuforiaStarted()
    {
        mCentralAnchorPointTransform = VuforiaBehaviour.Instance.transform;
    }
    
    void OnDisable()
    {
        DestroyChildAugmentationsOfTransform(transform);
    }

    void Update()
    {
        // Every nth frame, update the VuMark border outline to catch any changes to target tracking status
        if (Time.frameCount % UPDATE_FRAME_COUNT == 0)
            UpdateVuMarkBorderOutline();

        // Every frame, check target distance from camera and set alpha value of VuMark info canvas
        UpdateCanvasFadeAmount();
    }

    void UpdateVuMarkBorderOutline()
    {
        if (mLineRenderer)
        {
            // Only enable line renderer when target becomes Extended Tracked or when running in Unity Editor.
            mLineRenderer.enabled = mPreviousTargetStatus.Status != Status.NO_POSE && (mPreviousTargetStatus.Status == Status.EXTENDED_TRACKED || VuforiaRuntimeUtilities.IsPlayMode());

            // If the Device Pose Observer is enabled and the target becomes Extended Tracked,
            // set the VuMark outline to green. If in Unity Editor PlayMode, set to cyan.
            // Note that on HoloLens, the Device Pose Observer is always enabled (as of Vuforia 7.2).
            if (mLineRenderer.enabled)
                mLineRenderer.material.color = (mPreviousTargetStatus.Status == Status.EXTENDED_TRACKED) ? Color.green : Color.cyan;
        }
    }

    void UpdateCanvasFadeAmount()
    {
        if (mCentralAnchorPointTransform != null)
        {
            var positionInCameraSpace = mCentralAnchorPointTransform.InverseTransformPoint(transform.position);
            var distance = Vector3.Distance(Vector2.zero, positionInCameraSpace);
            if(mCanvasGroup!=null)
                 mCanvasGroup.alpha = 1 - Mathf.InverseLerp(mFadeRange.x, mFadeRange.y, distance);
        }
    }

    void DestroyChildAugmentationsOfTransform(Transform parentTransform)
    {
        if (parentTransform.childCount > PERSISTENT_NUMBER_OF_CHILDREN)
            for (var x = PERSISTENT_NUMBER_OF_CHILDREN; x < parentTransform.childCount; x++)
                Destroy(parentTransform.GetChild(x).gameObject);
    }
}
