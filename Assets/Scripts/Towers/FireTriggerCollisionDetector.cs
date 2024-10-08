using UnityEngine;

public class FireTriggerCollisionDetector : MonoBehaviour
{
    [SerializeField] private FlameThrowerDamage parent;

    GameManager gameManager;
    private EnemySpawner enemySpawner;
    private void Start()
    {
        gameManager = GameManager.Instance;
        enemySpawner = EnemySpawner.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            GameManager.Effect onFire = new GameManager.Effect(GameManager.EffectNames.Fire, parent.fireRate, parent.damage, 5f);
            GameManager.ApplyEffectData effectData = new GameManager.ApplyEffectData(onFire, enemySpawner.enemyTransformDictionary[other.transform]);
            gameManager.EnqueueAffectToApply(effectData);
        }
    }
}