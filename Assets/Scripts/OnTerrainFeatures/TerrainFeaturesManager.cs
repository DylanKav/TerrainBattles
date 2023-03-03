using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Random = UnityEngine.Random;

public class TerrainFeaturesManager : MonoBehaviour
{
    [SerializeField] private Vector3Int terrainStart = new Vector3Int(0, 0, 0);
    [SerializeField] private Vector3Int terrainEnd = new Vector3Int(100, 100, 100);
    [SerializeField] private List<OnTerrainFeature> SpawnList;
    int layerMask = 1 << 6;
    void Start()
    {
        foreach (var feature in SpawnList)
        {
            for (int i = 0; i < feature.QuantityOnTerrain; i++)
            {
                var prefab = Instantiate(feature.FeaturePrefab, this.transform);
                var raycast = GeneratePosition();
                prefab.transform.position = raycast.point;
                var cross = Vector3.Cross(prefab.transform.right, raycast.normal);

                var _lookRotation = Quaternion.LookRotation(cross, raycast.normal);

                prefab.transform.rotation = _lookRotation;
               // prefab.transform.localRotation = Quaternion.Euler(prefab.transform.localRotation.eulerAngles.x,
               //     Random.Range(0, 360), prefab.transform.localRotation.eulerAngles.z);
                Debug.Log(raycast.normal);
            }
        }
    }


    private RaycastHit GeneratePosition()
    {
        RaycastHit hit = default;
        bool found = false;
        if (Physics.Raycast(
                new Vector3(Random.Range(terrainStart.x, terrainEnd.x), 500,
                    Random.Range(terrainStart.z, terrainEnd.z)),
                Vector3.down, out hit, Mathf.Infinity))
        {
        }

        return hit;
    }

    [Serializable]
    struct OnTerrainFeature
    {
        public GameObject FeaturePrefab;
        public int QuantityOnTerrain;
    }
}
