using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VRHands : MonoBehaviour
{


    public enum Hand
    {
        NONE,
        LEFT,
        RIGHT
    }


    public delegate void LeftHandGrabbedEvent(GameObject gameObject);
    public delegate void LeftHandReleasedEvent(GameObject gameObject);
    public delegate void RightHandGrabbedEvent(GameObject gameObject);
    public delegate void RightHandReleasedEvent(GameObject gameObject);


    public LeftHandGrabbedEvent OnLeftHandGrabbed = null;
    public LeftHandReleasedEvent OnLeftHandReleased = null;
    public RightHandGrabbedEvent OnRightHandGrabbed = null;
    public RightHandReleasedEvent OnRightHandReleased = null;


    public GameObject LeftHand;
    public GameObject RightHand;


    public GameObject LeftHandGrabbing { get; private set; } = null;
    public GameObject RightHandGrabbing { get; private set; } = null;
    public Vector3 LeftMoveDelta { get; private set; } = Vector3.zero;
    public Vector3 RightMoveDelta { get; private set; } = Vector3.zero;


    void Start()
    {
        HandController _leftHand = LeftHand.GetComponent<HandController>();
        _leftHand.OnGrabbed += OnGrabbedLeft;
        _leftHand.OnReleased += OnReleasedLeft;
        _leftHand.OnGrabMove += OnGrabMoveLeft;
        
        HandController _rightHand = RightHand.GetComponent<HandController>();
        _rightHand.OnGrabbed += OnGrabbedRight;
        _rightHand.OnReleased += OnReleasedRight;
        _rightHand.OnGrabMove += OnGrabMoveRight;
    }


    private void OnGrabbedLeft(HandController sender, List<GameObject> grabbedObjects)
    {
        LeftHandGrabbing = grabbedObjects[0];
        OnLeftHandGrabbed?.Invoke(LeftHandGrabbing);
    }


    private void OnGrabbedRight(HandController sender, List<GameObject> grabbedObjects)
    {
        RightHandGrabbing = grabbedObjects[0];
        OnRightHandGrabbed?.Invoke(RightHandGrabbing);
    }


    private void OnReleasedLeft(HandController sender, List<GameObject> grabbedObjects)
    {
        LeftMoveDelta = Vector3.zero;

        foreach (GameObject gameObject in grabbedObjects)
        {
            OnLeftHandReleased?.Invoke(gameObject);
        }
    }


    private void OnReleasedRight(HandController sender, List<GameObject> grabbedObjects)
    {
        RightMoveDelta = Vector3.zero;

        foreach (GameObject gameObject in grabbedObjects)
        {
            OnRightHandReleased?.Invoke(gameObject);
        }
    }


    private void OnGrabMoveLeft(HandController sender, Vector3 delta)
    {
        if (delta == Vector3.zero) return;

        LeftMoveDelta = new Vector3(0, delta.y, 0);
    }


    private void OnGrabMoveRight(HandController sender, Vector3 delta)
    {
        if (delta == Vector3.zero) return;

        RightMoveDelta = new Vector3(0, delta.y, 0);
    }


}
