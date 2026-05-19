using System.Collections.Generic;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    [Header("Block List")]
    public List<GameObject> blockOrderList; 

    [Header("Spawn Position")]
    public Transform spawnPosition; 

    private int currentBlockIndex = 0;

    void Start()
    {
        SpawnNext();
    }

    public void SpawnNext()
    {
        if (currentBlockIndex >= blockOrderList.Count) return;

        GameObject prefabToSpawn = blockOrderList[currentBlockIndex];
        Instantiate(prefabToSpawn, spawnPosition.position, Quaternion.identity);

        currentBlockIndex++;
    }
}