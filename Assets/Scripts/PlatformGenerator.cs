using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; 

public class GeneratePlatforms : EditorWindow
{
    [MenuItem("Custom/Generate Walls %g")]
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
        if (GUILayout.Button("Generate Platforms"))
        {
            GameObject.Find("Platforms").GetComponent<PlatformGenerator>().SetPlatformsToPositions();
        }

    }
}



public class PlatformGenerator : MonoBehaviour
{
    public int numPoints;
    public float circleRadius; 
    public float minDistanceBetweenPoints;

    public GameObject PlatformPrefab; 

    public List<Vector2> GeneratePoints(int numP, float circleRad, float minDistBetweenPoints)
    {
        List<Vector2> positions = new List<Vector2>();

        bool positionTooClose = false; 
        for(int i = 0; i < numP; i++)
        {
            Vector2 position = Vector2.zero;
            do
            {
                position = Random.insideUnitCircle * circleRad;
                positionTooClose = false;
                foreach (Vector2 pos in positions)
                {
                    if (Vector2.Distance(pos, position) < minDistBetweenPoints)
                    {
                        positionTooClose = true;
                        break;
                    }
                }
                
            } while (positionTooClose);
            positions.Add(position);
        }
        return positions; 
    }

    public void spawnPlatforms()
    {
        for (int i = 0; i < numPoints; i++)
            Instantiate(PlatformPrefab, transform); 
    }
  
    public void SetPlatformsToPositions()
    {

        for (int i = 0; i < transform.childCount; i++)
        {
            DestroyImmediate(transform.GetChild(i).gameObject); 
        }

        spawnPlatforms(); 

        List<Vector2> positions = GeneratePoints(transform.childCount,circleRadius, minDistanceBetweenPoints);
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).position = new Vector3(positions[i].x, transform.position.y, positions[i].y); 
        }
    } 
}
