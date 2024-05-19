using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class SpawnEnemies : MonoBehaviour
{
    [Header("Fields")]
    public GameObject[] Enemies1;
    public GameObject[] Bosses;
    public float[] Hardness;
    public GameObject cnvSpawn;
    public Dictionary<GameObject, float> Enemies = new Dictionary<GameObject, float>();
    public DialogueScript dlg;
    public Sprite SlimeSprite;
    public GameObject prnt;

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
    public int PriorIndex = 0;
    public bool UsedTutorial;
    public bool roundended;

    private void Update()
    {
        if(GameObject.FindGameObjectsWithTag("Enemy").Length == 0 && roundended && !cnvSpawn.activeSelf)
        {
            if (!UsedTutorial)
            {
                UsedTutorial = true;
                dlg.StartCrtnRemotely("Yes! I've cured the slimes you caught. Now, you can deploy them, and they'll assist you in battling the mutant", SlimeSprite, true);
            }
            cnvSpawn.SetActive(true);
        }
    }
    public void SpawnABoss()
    {
        roundended = false;
        Instantiate(Bosses[StageIndex], transform.position, Quaternion.identity);
    }
    private void OnEnable()
    {
        for (int i = 0; i < Enemies1.Length; i++)
        {
            Enemies[Enemies1[i]] = Hardness[i];
        }
        StartCoroutine(StartSpawningg());
    }
    public IEnumerator BossKilled()
    {
        foreach(EnemyAI gmb in GameObject.FindObjectsByType<EnemyAI>(FindObjectsSortMode.None))
        {
            gmb.Die();
        }
        StageIndex++;
        StartingHardness *= BigHardnessMultiplyer;
        WaveCooldown /= BigHardnessMultiplyer;
        yield return new WaitForSeconds(4f);
        StartCoroutine(StartSpawningg());
    }
    IEnumerator StartSpawningg()
    {
        roundended = false;
        for (int i = 0; i < AmountOfWaves; i++)
        {
            float HardnessLeft = StartingHardness;
            Dictionary<GameObject, float> EnemiesToUse = Enemies.Where(obj => obj.Value <= HardnessLeft)
                                                    .ToDictionary(kv => kv.Key, kv => kv.Value);
            while (EnemiesToUse.Count != 0)
            {
                System.Random random = new System.Random();

                KeyValuePair<GameObject, float> chosenEnemy = EnemiesToUse.ElementAt(random.Next(0, EnemiesToUse.Count));
                GameObject enem = Instantiate(chosenEnemy.Key, new Vector3(Random.Range(border1.position.x, border2.position.x), Random.Range(border1.position.y, border2.position.y), 0), Quaternion.identity);
                enem.GetComponent<NavMeshAgent>().avoidancePriority = PriorIndex;
                PriorIndex++;
                //GameObject enem = ((EnemyAI)GameManager.Instance.pool.Get<EnemyAI>()).gameObject;
                HardnessLeft -= chosenEnemy.Value;
                EnemiesToUse = Enemies.Where(obj => obj.Value <= HardnessLeft)
                                                        .ToDictionary(kv => kv.Key, kv => kv.Value); yield return new WaitForSeconds(SpawnCooldown);
            }
            StartingHardness *= HardnessMultiplyer;
            WaveCooldown /= HardnessMultiplyer;
            yield return new WaitForSeconds(Random.Range(WaveCooldown*10 - 3, WaveCooldown*10 + 3)/10);
        }
        roundended = true;
    }
}