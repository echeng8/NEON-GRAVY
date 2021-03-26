using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR 
using UnityEditor;
public class GeneratePlatforms : EditorWindow
{
    [MenuItem("Custom/Generate Platforms %g")]
    public static void OpenWindow()
    {
        GetWindow<GeneratePlatforms>();
    }

    void OnEnable()
    {
        // cache any data you need here.
        // if you want to persist values used in the inspector, you can use eg. EditorPrefs
    }

    void OnGUI()
    {
        //Draw things here. Same as custom inspectors, EditorGUILayout and GUILayout has most of the things you need
        if (GUILayout.Button("UpdatePlatforms"))
        {
            
            GameObject.Find("Platforms").GetComponent<PlatformGenerator>().UpdatePlatforms();
        }

    }
}
#endif


public class PlatformGenerator : MonoBehaviour
{
    public float minDistanceBetweenPoints;
    public Vector2 sampleRegionSize;
    public GameObject MinimapCamera;

    public GameObject PlatformPrefab;

    public void spawnPlatforms(int numPlatforms)
    {
        for (int i = 0; i < numPlatforms; i++)
        {
            GameObject spawnedPlatform = Instantiate(PlatformPrefab, transform);
            spawnedPlatform.GetComponentInChildren<PlatformAppearance>().InitalizeState(); 
        }
    }
  
    public void UpdatePlatforms()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            DestroyImmediate(transform.GetChild(i).gameObject); 
        }
        List<Vector2> positions = PoissonDiscSampling.GeneratePoints(minDistanceBetweenPoints,sampleRegionSize);
        spawnPlatforms(positions.Count); 
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).position = new Vector3(positions[i].x - sampleRegionSize.x/2 + MinimapCamera.transform.position.x, transform.position.y, positions[i].y - sampleRegionSize.y/2 + MinimapCamera.transform.position.z); 
        }
    }
}
