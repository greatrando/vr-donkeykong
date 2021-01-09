using UnityEngine;


public class PlayerClimber : MonoBehaviour
{


    public bool IsClimbing { get; private set; }


    private OVRPlayerController _playerController;
    private CharacterController _characterController;
    private ClickHelper _clickHelper;
    private Vector3 _mouseMoveDelta = Vector3.zero;

    private VRHands _hands;
    private VRHands.Hand _lastHand;
    private bool _leftHandGrabbing = false;
    private bool _rightHandGrabbing = false;


    void Start()
    {
        _playerController = this.gameObject.GetComponent<OVRPlayerController>();
        _characterController = this.gameObject.GetComponent<CharacterController>();

        _hands = this.transform.gameObject.GetComponent<VRHands>();
        _hands.OnLeftHandGrabbed += OnLeftHandGrabbed;
        _hands.OnLeftHandReleased += OnLeftHandReleased;
        _hands.OnRightHandGrabbed += OnRightHandGrabbed;
        _hands.OnRightHandReleased += OnRightHandReleased;

        _clickHelper = this.transform.gameObject.GetComponent<ClickHelper>();
        _clickHelper.OnMouseDown += OnMouseDown;
        _clickHelper.OnMouseDownMove += OnMouseDownMove;
        _clickHelper.OnMouseUp += OnMouseUp;
        _clickHelper.ClickObjectNames.Add("Rung");
    }


    void Update()
    {
        UpdateMouse();
        UpdateVR();
    }


    void UpdateMouse()
    {
        if (_mouseMoveDelta == Vector3.zero) return;

        _characterController.Move(_mouseMoveDelta);

        _mouseMoveDelta = Vector3.zero;
    }


    void UpdateVR()
    {
        if (!IsClimbing || _lastHand == VRHands.Hand.NONE) return;

        Vector3 delta = (_lastHand == VRHands.Hand.LEFT) ? _hands.LeftMoveDelta : _hands.RightMoveDelta;
        if (delta == Vector3.zero) return;

        _characterController.Move(delta);
    }


    public void OnLeftHandGrabbed(GameObject grabbedObject)
    {
        if (grabbedObject.name != "Rung") return;

        _leftHandGrabbing = true;
        _lastHand = VRHands.Hand.LEFT;

        IsClimbing = true;
        _playerController.IsClimbing = IsClimbing;
    }


    public void OnRightHandGrabbed(GameObject grabbedObject)
    {
        if (grabbedObject.name != "Rung") return;

        _rightHandGrabbing = true;
        _lastHand = VRHands.Hand.RIGHT;

        IsClimbing = true;
        _playerController.IsClimbing = IsClimbing;
    }


    public void OnLeftHandReleased(GameObject grabbedObject)
    {
        if (grabbedObject.name != "Rung") return;

        _leftHandGrabbing = false;

        _lastHand = _rightHandGrabbing ? VRHands.Hand.RIGHT : VRHands.Hand.NONE;
        
        IsClimbing = _rightHandGrabbing;
        _playerController.IsClimbing = IsClimbing;
    }


    public void OnRightHandReleased(GameObject grabbedObject)
    {
        if (grabbedObject.name != "Rung") return;

        _rightHandGrabbing = false;

        _lastHand = _leftHandGrabbing ? VRHands.Hand.LEFT : VRHands.Hand.NONE;

        IsClimbing = _leftHandGrabbing;
        _playerController.IsClimbing = IsClimbing;
    }


    private void OnMouseDown(ClickHelper sender, string name, GameObject gameObject)
    {
        if (name != "Rung") return;

        IsClimbing = true;
    }


    private void OnMouseUp(ClickHelper sender, string name, GameObject gameObject)
    {
        if (name != "Rung") return;

        IsClimbing = false;
    }


    private void OnMouseDownMove(ClickHelper sender, Vector3 delta)
    {
        _mouseMoveDelta = delta;
    }


}