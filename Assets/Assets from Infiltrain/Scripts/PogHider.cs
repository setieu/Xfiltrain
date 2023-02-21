using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PogHider : MonoBehaviour
{
    public GameObject objectToSpawn;
    public float spawnDelay = 2f;
    public float waveDelay = 2f;
    public int waveSize = 1;
    public float minSpawnDistance = 5f;
    public float minYOffset = 1f;
    public float maxYOffset = 2f;
    public float minZ = -5f;
    public float maxZ = 5f;
    public float minX = 10f;
    public float maxX = 20f;
    public float leftSpawnDistance = 5f;
    public float rightSpawnDistance = 5f;

    private float lastWaveTime;
    private int objectsSpawnedInCurrentWave;
    private Transform playerTransform;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        playerTransform = GetComponent<Transform>();
    }

    private void Update()
    {
        if (gameManager.gameActive)
        {
            // Check if there are any active poghiders
            if (GameObject.FindGameObjectsWithTag("PogHider").Length == 0)
            {
                if (Time.time - lastWaveTime >= waveDelay)
                {
                    objectsSpawnedInCurrentWave = 0;
                    StartCoroutine(SpawnWave());
                    lastWaveTime = Time.time;
                }
            }
        }
    }

    private IEnumerator SpawnWave()
    {
        // Increment wave size at the start of each wave
        waveSize++;
        objectsSpawnedInCurrentWave = 0;

        if (gameManager.gameActive)
        {
            while (objectsSpawnedInCurrentWave < waveSize)
            {
                Vector3 spawnPosition = GetValidSpawnPosition();

                if (spawnPosition != Vector3.zero)
                {
                    GameObject spawnedObject = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
                    objectsSpawnedInCurrentWave++;
                }

                yield return new WaitForSeconds(spawnDelay);
            }
        }

    }

    private Vector3 GetValidSpawnPosition()
    {
        int tries = 0;
        Vector3 spawnPosition = Vector3.zero;

        while (tries < 1000)
        {
            float randomZ;
            // Calculate Z position
            if (Random.Range(0, 2) == 0) // Spawn on left
            {
                randomZ = playerTransform.position.z - leftSpawnDistance;
            }
            else // Spawn on right
            {
                randomZ = playerTransform.position.z + rightSpawnDistance;
            }
            float randomY = Random.Range(minYOffset, maxYOffset);

            spawnPosition = new Vector3(Random.Range(minX, maxX), randomY, randomZ);

            if (Vector3.Distance(playerTransform.position, spawnPosition) < minSpawnDistance)
            {
                tries++;
                continue;
            }

            Collider[] colliders = Physics.OverlapSphere(spawnPosition, 3.5f);

            if (colliders.Length > 1)
            {
                tries++;
                continue;
            }

            break;
        }

        return spawnPosition;
    }
}
