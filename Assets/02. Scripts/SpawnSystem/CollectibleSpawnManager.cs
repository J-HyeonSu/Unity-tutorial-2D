using Utilities;
using UnityEngine;


namespace Platformer
{
    public class CollectibleSpawnManager : EntitySpawnManager
    {
        /***
            CollectibleSpawnManager : EntitySpawnManager
            │
            ├── CollectibleData[] : CollectibleData
            │   └── CollectibleData : EntityData (ScriptableObject)
            │
            ├── EntitySpawner<Collectible>
            │   ├── IEntityFactory<Collectible>
            │   │   └── EntityFactory<Collectible> (EntityData[] data)
            │   │       └── EntityData.prefab 인스턴스 생성
            │   │
            │   └── ISpawnPointStrategy
            │       ├── LinearSpawnPointStrategy(Transform[] spawnPoints)
            │       └── RandomSpawnPointStrategy(Transform[] spawnPoints)

         */         
        //Scriptable Object(CollectibleData로 만든거)
        [SerializeField] private CollectibleData[] collectibleData;
        [SerializeField] private float spawnInterval = 1f;

        
        private EntitySpawner<Collectible> spawner;
        
        private CountdownTimer spawnTimer;
        private int counter;

        protected override void Awake()
        {
            base.Awake();

            spawner = new EntitySpawner<Collectible>(
                new EntityFactory<Collectible>(collectibleData),
                spawnPointStrategy);

            spawnTimer = new CountdownTimer(spawnInterval);
            spawnTimer.OnTimerStop += () =>
            {
                if (counter++ >= spawnPoints.Length)
                {
                    spawnTimer.Stop();
                    return;
                }

                Spawn();
                spawnTimer.Start();
            };
        }

        void Start()
        {
            spawnTimer.Start();
        }

        void Update()
        {
            spawnTimer.Tick(Time.deltaTime);
        }

        public override void Spawn()
        {
            spawner.Spawn();
        }
    }
}