using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    [Header("Fields")]
    public GameObject[] Enemies1;
    public GameObject[] Bosses;
    public float[] Hardness;
    public Dictionary<GameObject, float> Enemies = new Dictionary<GameObject, float>();

    [Header("Settings")]
    public float BigHardnessMultiplyer = 2f;
    public float HardnessMultiplyer = 1.1f;
    public float StartingHardness;
    public int AmountOfWaves;
    public int AmountOfStages;
    public float SpawnCooldown;
    public float WaveCooldown;
    public float StageCooldown;
    public Transform border1;
    public Transform border2;

    [Header("Debug")]
    public int StageIndex = 0;

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
        for (int i = 0; i < AmountOfWaves; i++)
        {
            float HardnessLeft = StartingHardness;
            Dictionary<GameObject, float> EnemiesToUse = Enemies.Where(obj => obj.Value <= HardnessLeft)
                                                    .ToDictionary(kv => kv.Key, kv => kv.Value);
            while (EnemiesToUse.Count != 0)
            {
                System.Random random = new System.Random();

                KeyValuePair<GameObject, float> chosenEnemy = EnemiesToUse.ElementAt(random.Next(0, EnemiesToUse.Count));
                Instantiate(chosenEnemy.Key, new Vector2(Random.Range(border1.position.x, border2.position.x), Random.Range(border1.position.y, border2.position.y)), Quaternion.identity);
                //GameObject enem = ((EnemyAI)GameManager.Instance.pool.Get<EnemyAI>()).gameObject;
                HardnessLeft -= chosenEnemy.Value;
                EnemiesToUse = Enemies.Where(obj => obj.Value <= HardnessLeft)
                                                        .ToDictionary(kv => kv.Key, kv => kv.Value); yield return new WaitForSeconds(SpawnCooldown);
            }
            StartingHardness *= HardnessMultiplyer;
            yield return new WaitForSeconds(WaveCooldown);
        }
        Instantiate(Bosses[StageIndex], transform.position, Quaternion.identity);
        StageIndex++;
        StartingHardness *= BigHardnessMultiplyer;
    }
}