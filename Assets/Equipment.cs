using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Equipment : MonoBehaviour
{


    private DonkeyKongGame _game;
    private GameObject _capsule;
    private VRHands _hands;
    private GameObject _pickupHammer;
    private bool _hammerEquipped = false;


    // Start is called before the first frame update
    void Start()
    {
        GameObject gameObject = GameObject.Find("Game");
        _game = gameObject.GetComponent<DonkeyKongGame>();

        _capsule = this.transform.Find("Capsule").gameObject;

        _hands = this.transform.gameObject.GetComponent<VRHands>();
        _hands.OnLeftHandGrabbed += OnHandGrabbed;
        _hands.OnRightHandGrabbed += OnHandGrabbed;
    }


    void Update()
    {
        UpdateHammer();
    }


    private void OnHandGrabbed(GameObject grabbedObject)
    {
        if (!grabbedObject.name.StartsWith("Hammer")) return;

        if (grabbedObject.name != "Hammer")
        {   //the hammer portion, but we want handle (base part)
            grabbedObject = gameObject.transform.parent.gameObject;
        }

        _pickupHammer = grabbedObject;
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


}
