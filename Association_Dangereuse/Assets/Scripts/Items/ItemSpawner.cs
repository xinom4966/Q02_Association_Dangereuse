using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class ItemSpawner : NetworkBehaviour
{
    [SerializeField] private List<GameObject> itemPrefabs;
    [SerializeField] private int numberToSpawn = 1;
    [SerializeField] private Vector3 spawnLimitsMax = Vector3.one;
    [SerializeField] private Vector3 spawnLimitsMin = Vector3.one;
    private GameObject itemInstance;
    private NetworkObject instanceObject;

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += SpawnerStart;
    }

    private void SpawnerStart()
    {
        if (!NetworkManager.Singleton.IsHost)
        {
            NetworkObject.Despawn();
            return;
        }
        SpawnItemStart();
    }

    private void SpawnItemStart()
    {
        for (int i = 0; i < numberToSpawn; i++)
        {
            Vector3 spawnPos = new Vector3(Random.Range(spawnLimitsMin.x,spawnLimitsMax.x), Random.Range(spawnLimitsMin.y, spawnLimitsMax.y), Random.Range(spawnLimitsMin.z, spawnLimitsMax.z));
            itemInstance = Instantiate(itemPrefabs[Random.Range(0,itemPrefabs.Count-1)],spawnPos, Quaternion.identity);
            instanceObject = itemInstance.GetComponent<NetworkObject>();
            instanceObject.Spawn(true);
        }
    }

    #region depricated functions

    /*public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (NetworkManager.Singleton.IsClient)
        {
            NetworkObject.Despawn();
        }
    }*/

    #endregion
}
