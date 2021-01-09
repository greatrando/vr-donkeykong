using UnityEngine;


public class BarrelShortCut : MonoBehaviour
{


    private const int PERCENT_SHORTCUT = 25;

     
    void OnTriggerEnter(Collider col) 
    {
        int random = UnityEngine.Random.Range(0, 100);
        if (random < PERCENT_SHORTCUT)
        {
            GameBarrel script = col.gameObject.GetComponent<GameBarrel>();
            if (script != null)
            {
                script.PerformShortcut();
            }
        }
    }


}
