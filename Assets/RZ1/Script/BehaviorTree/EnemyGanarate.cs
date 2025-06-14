using UnityEngine;

public class EnemyGanarate : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab; // プレハブをインスペクターから設定
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instantiate(enemyPrefab,transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
