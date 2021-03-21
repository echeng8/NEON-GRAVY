using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; 
using Photon.Pun;

/// <summary>
/// keeps a running list of players in the collider in PlayersInside
/// </summary>
public class ObjectDetector : MonoBehaviourPun
{
    public GameObject ObjectDetected;
    public string ObjectTag;

    public List<GameObject> ObjectsDetected = new List<GameObject>(); 

    public GameObjectEvent OnObjectChange = new GameObjectEvent(); 
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(ObjectTag) && !ObjectsDetected.Contains(other.gameObject))
        {
            print("detected " + other.name); 
            ObjectDetected = other.gameObject;
            ObjectsDetected.Add(other.gameObject); 
            OnObjectChange.Invoke(ObjectDetected); 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(ObjectTag) && ObjectsDetected.Contains(other.gameObject))
        {
            print("left " + other.name); 
            ObjectsDetected.Remove(other.gameObject); 
            if(ObjectsDetected.Count > 0)
            {
                ObjectDetected = ObjectsDetected[0]; 
            } else
            {
                ObjectDetected = null;
            }
            
            OnObjectChange.Invoke(ObjectDetected);
        }
    }
}
