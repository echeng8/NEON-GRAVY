using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColorChange : MonoBehaviour
{

    Queue<PlatformState> last3Plats = new Queue<PlatformState>();
    
    public void RecordNewBounce(PlatformState state)
    {
        last3Plats.Enqueue(state); 
        if(last3Plats.Count == 3)
        {
            return; 
        }
    } 
}
