using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{


    public delegate void GrabbedEvent(HandController sender, List<GameObject> grabbedObjects);
    public delegate void ReleasedEvent(HandController sender, List<GameObject> grabbedObjects);
    public delegate void GrabMoveEvent(HandController sender, Vector3 delta);

    public OVRInput.Controller _controller = OVRInput.Controller.None;
    public GrabbedEvent OnGrabbed = null;
    public ReleasedEvent OnReleased = null;
    public GrabMoveEvent OnGrabMove = null;


    private List<GameObject> currentCollisions = new List<GameObject>();
    private Vector3 _grabPosition;
    private Vector3 _deltaPosition;
    private Vector3 _lastDeltaPosition;

    private bool isHandClosed = false;


    void Start()
    {
    }


    void Update()
    {
        CheckHandStateChange();

        if (isHandClosed)
        {
            if (_deltaPosition != Vector3.zero || _lastDeltaPosition != Vector3.zero)
            {
                _lastDeltaPosition = _deltaPosition;
                OnGrabMove?.Invoke(this, _deltaPosition);
            }
        }
    }


    private void FixedUpdate()
    {
        _grabPosition = this.transform.localPosition;
    }


    private void LateUpdate()
    {
        _deltaPosition = (_grabPosition - this.transform.localPosition);
    }

     
    void OnTriggerEnter(Collider col) 
    {
        currentCollisions.Add(col.gameObject);
    }


    void OnTriggerExit(Collider col) 
    {
        if (isHandClosed) return; //don't let go

        OnReleased(this, new List<GameObject>() { col.gameObject });
        currentCollisions.Remove(col.gameObject);
    }

     
    void OnCollisionEnter(Collision col) 
    {
        currentCollisions.Add(col.gameObject);
    }


    void OnCollisionExit(Collision col) 
    {
        if (isHandClosed) return; //don't let go

        OnReleased(this, new List<GameObject>() { col.gameObject });
        currentCollisions.Remove(col.gameObject);
    }


    private void CheckHandStateChange()
    {
        bool isTrigger;
        if (IsLeftHand)
        {
            isTrigger = OVRInput.Get(OVRInput.RawButton.LHandTrigger);
        }
        else
        {
            isTrigger = OVRInput.Get(OVRInput.RawButton.RHandTrigger);
        }

        if (!isHandClosed && isTrigger)
        {
            isHandClosed = true;
            _grabPosition = this.transform.localPosition;

            if (currentCollisions.Count > 0)
            {
                OnGrabbed?.Invoke(this, currentCollisions);
            }

        }
        else if (isHandClosed && !isTrigger)
        {
            OnReleased(this, currentCollisions);
            currentCollisions.Clear();
            isHandClosed = false;
        }
    }


    public bool IsLeftHand
    {
        get
        {
            return (_controller == OVRInput.Controller.LTouch);
        }
    }


    public bool IsRightHand
    {
        get
        {
            return (_controller == OVRInput.Controller.RTouch);
        }
    }


}
