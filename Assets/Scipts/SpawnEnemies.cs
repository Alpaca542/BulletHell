using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    [Header("Fields")]
    public GameObject[] Enemies1;
    public float[] Hardness;
    public Dictionary<GameObject, float> Enemies = new Dictionary<GameObject, float>();

    [Header("Settings")]
    public float HardnessMultiplyer = 1.1f;
    public float StartingHardness;
    public int AmountOfWaves;
    public int AmountOfStages;
    public float SpawnCooldown;
    public float WaveCooldown;
    private void Awake()
    {
        for(int i = 0; i < Enemies1.Length; i ++)
        {
            Enemies[Enemies1[i]] = Hardness[i];
        }
        StartCoroutine(StartSpawningg());
    }
    IEnumerator StartSpawningg()
    {
        for(int i = 0; i < AmountOfWaves; i++)
        {
            float HardnessLeft = StartingHardness;
            Dictionary<GameObject, float> EnemiesToUse = Enemies.Where(obj => obj.Value <= HardnessLeft)
                                                    .ToDictionary(kv => kv.Key, kv => kv.Value);
            while (EnemiesToUse.Count != 0)
            {
                KeyValuePair<GameObject, float> chosenEnemy = EnemiesToUse.ElementAt(Random.Range(0, Enemies.Count - 1));
                Instantiate(chosenEnemy.Key, transform.position, Quaternion.identity);
                HardnessLeft -= chosenEnemy.Value;
                EnemiesToUse = Enemies.Where(obj => obj.Value <= HardnessLeft)
                                                        .ToDictionary(kv => kv.Key, kv => kv.Value); yield return new WaitForSeconds(SpawnCooldown);
            }
            StartingHardness *=HardnessMultiplyer;
            yield return new WaitForSeconds(WaveCooldown);
        }
    }
}
