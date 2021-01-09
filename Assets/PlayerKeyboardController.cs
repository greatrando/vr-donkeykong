using UnityEngine;


public class PlayerKeyboardController : MonoBehaviour
{
    
    private const bool ENABLED = true;

    public GameObject MainCamera;
    public GameObject GroundCollider;
    public CharacterController controller;

    public float Speed = 12f;
    public float MouseSensitivity = 100f;
    public float Gravity = -9.8f;
    public float GroundDistance = 0.4f;
    public float JumpHeight = 3f;

    private DonkeyKongGame _game;
    private GameObject _capsule;
    private CollisionHelper _collisionHelper;
    private ClickHelper _clickHelper;
    private PlayerClimber _climber;
    private VRHands _hands;
    private float xRotation = 0f;
    private Vector3 velocity;
    private bool isGrounded;
    private bool shouldDie = false;
    private int _dieFrames = 0;
    private GameObject _pickupHammer;
    private bool _hammerEquipped = false;


    void Start()
    {
        if (!ENABLED) return;

        GameObject gameObject = GameObject.Find("Game");
        _game = gameObject.GetComponent<DonkeyKongGame>();

        _capsule = this.transform.Find("Capsule").gameObject;
        CollisionHelper col = _capsule.GetComponent<CollisionHelper>();
        col.OnEnter += OnCollisionHelperEnter;
        col.OnExit += OnCollisionHelperExit;

        _collisionHelper = GroundCollider.GetComponent<CollisionHelper>();
        _collisionHelper.IgnoreObjects.Add(this.gameObject);
        _collisionHelper.IgnoreObjects.Add(this.transform.Find("Capsule").gameObject);
        _collisionHelper.OnEnter += OnGroundCollisionEnter;
        _collisionHelper.OnExit += OnGroundCollisionExit;

        _clickHelper = this.transform.gameObject.GetComponent<ClickHelper>();
        _clickHelper.OnMouseDown += OnMouseDown;
        _clickHelper.ClickObjectNames.Add("Hammer");
        _clickHelper.ClickObjectNames.Add("HammerHead");

        _hands = this.transform.gameObject.GetComponent<VRHands>();
        _hands.OnLeftHandGrabbed += OnLeftHandGrabbed;
        _hands.OnRightHandGrabbed += OnRightHandGrabbed;

        _climber = this.transform.gameObject.GetComponent<PlayerClimber>();
    }


    void Update()
    {
        if (!ENABLED) return;

        if (shouldDie)
        {
            this.gameObject.transform.SetPositionAndRotation(new Vector3(-0.42f, 11.06f, 3.04f), new Quaternion(0, 0, 0, 0));

            _game.Reset();
            shouldDie = false;

            if (_pickupHammer != null)
            {
                _pickupHammer.GetComponent<HammerScript>().UnEquip();
            }
            
            _dieFrames = 10;
            return;
        }
        else if (_dieFrames > 0)
        {
            _dieFrames--;
            return;
        }
        else if (this.transform.position.y < 0)
        {
            shouldDie = true;
        }

        UpdateHammer();
        UpdateCamera();
        UpdateMovement();
        UpdateGravity();
    }
  

    void OnCollisionHelperEnter(CollisionHelper sender, GameObject gameObject) 
    {
        if (gameObject.name == "barrel" || gameObject.name == "fireball")
        {
            _game.PresentToast("Game Over!!");
            shouldDie = true;
        }
    }


    void OnCollisionHelperExit(CollisionHelper sender, GameObject gameObject) 
    {
        // _game.PresentToast("OUT:\t" + gameObject.name);
    }


    private void OnHammerDestroyed(HammerScript sender)
    {
        _hammerEquipped = false;
        _pickupHammer = null;
        _game.PresentToast("Hammer destroyed.");
    }


    private void OnHammerCountdown(HammerScript sender, int secondsRemaining)
    {
        _game.PresentToast(secondsRemaining.ToString(), 0);
    }


    private void OnGroundCollisionEnter(CollisionHelper sender, GameObject gameObject)
    {
        isGrounded = true;
    }


    private void OnGroundCollisionExit(CollisionHelper sender, GameObject gameObject)
    {
        isGrounded = (sender.CurrentCollisions.Count > 0);
    }


    public void OnLeftHandGrabbed(GameObject grabbedObject)
    {
        CheckForGrabbedHammer(grabbedObject);
    }


    public void OnRightHandGrabbed(GameObject grabbedObject)
    {
        CheckForGrabbedHammer(grabbedObject);
    }


    private void OnMouseDown(ClickHelper sender, string name, GameObject gameObject)
    {
        CheckForGrabbedHammer(gameObject);
    }


    private void CheckForGrabbedHammer(GameObject gameObject)
    {
        if (!gameObject.name.StartsWith("Hammer")) return;

        if (gameObject.name != "Hammer")
        {   //the hammer portion, but we want handle (base part)
            gameObject = gameObject.transform.parent.gameObject;
        }

        _pickupHammer = gameObject;
    }


    private void UpdateHammer()
    {
        if (_pickupHammer == null) return;
        if (_hammerEquipped) return;

        _pickupHammer.transform.SetParent(_capsule.transform);
        _pickupHammer.transform.localPosition = new Vector3(0.1f, -0.45f, 1.43f);
        _pickupHammer.transform.localEulerAngles = new Vector3(45f, 0f, 0f);

        HammerScript script = _pickupHammer.GetComponent<HammerScript>();
        script.OnDestroyed += OnHammerDestroyed;
        script.OnCountdown += OnHammerCountdown;
        script.Equiped();
        _game.PresentToast("Hammer equipped");

        _hammerEquipped = true;
    }


    private void UpdateGravity()
    {
        if (_climber.IsClimbing) 
        {
            return;
        }

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (isGrounded)
        {
            bool shouldJump = Input.GetButtonDown("Jump") || Input.GetMouseButtonDown(1);
            if (shouldJump)
            {
                velocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);
            }
        }

        velocity.y += (Gravity * Time.deltaTime);

        controller.Move(velocity * Time.deltaTime);
    }


    private void UpdateMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = (transform.right * x + transform.forward * z) * Speed * Time.deltaTime;

        if (_climber.IsClimbing)
        {
            move.y = 0;
            move.z = 0;
        }
        
        controller.Move(move);
    }


    private void UpdateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        MainCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
        transform.Rotate(Vector3.up * mouseX);
    }


}
