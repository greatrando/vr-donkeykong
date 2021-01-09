using System;
using UnityEngine;


public class HammerScript : MonoBehaviour
{


    public delegate void DestroyedEvent(HammerScript sender);
    public delegate void CountdownEvent(HammerScript sender, int secondsRemaining);


    private const int LIFETIME_SECONDS = 20;


    public DestroyedEvent OnDestroyed = null;
    public CountdownEvent OnCountdown = null;


    private DateTime _destructionTime = DateTime.MinValue;
    private int _secondsRemaining = 0;


    void Start()
    {
        
    }


    void Update()
    {
        if (_destructionTime == DateTime.MinValue) return;

        if (_destructionTime < DateTime.Now)
        {
            UnEquip();
        }
        else
        {
            int seconds = (int)_destructionTime.Subtract(DateTime.Now).TotalSeconds;
            if (seconds != _secondsRemaining)
            {
                _secondsRemaining = seconds;
                OnCountdown?.Invoke(this, _secondsRemaining);
            }
        }
    }


    public void Equiped()
    {
        _destructionTime = DateTime.Now.AddSeconds(LIFETIME_SECONDS);
        _secondsRemaining = LIFETIME_SECONDS;
    }


    public void UnEquip()
    {
        OnDestroyed?.Invoke(this);
        Destroy(this.gameObject);
    }


}
