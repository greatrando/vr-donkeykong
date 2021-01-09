using UnityEngine;


public class DonkeyKongGame : MonoBehaviour
{


    private Toast _toast;
    private GameBarrels _barrels;
    private FireBalls _fireBalls;


    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;// .Locked;

        _toast = this.gameObject.GetComponent<Toast>();
        _barrels = this.gameObject.GetComponent<GameBarrels>();
        _fireBalls = this.gameObject.GetComponent<FireBalls>();
    }


    void Update()
    {
        // TestToast();
    }


    private void TestToast()
    {
        int test = UnityEngine.Random.Range(0, 1000);
        if (test > 997)
        {
            test = UnityEngine.Random.Range(0, 100);
            PresentToast((test > 50) ? "500" : "100");
        }
    }


    public void PresentToast(string toastMessage)
    {
        _toast.Present(toastMessage);
    }


    public void PresentToast(string toastMessage, float displayTime)
    {
        _toast.Present(toastMessage, 0.5f, displayTime, 0.5f);
    }


    public void Reset()
    {
        _barrels.Reset();
        _fireBalls.Reset();
    }


}
