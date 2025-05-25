using UnityEngine;

namespace Platformer
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private FloatEventChannel playerHealthChannel;
        [SerializeField] private EntityEventChannel deathChannel;

        private int currentHealth;

        public bool IsDead => currentHealth <= 0;

        void Awake()
        {
            currentHealth = maxHealth;
        }

        void Start()
        {
            PublishHealthPercentage();
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            PublishHealthPercentage();
            
            if (deathChannel != null)
            {
                if (IsDead)
                {
                    deathChannel.Invoke(GetComponent<Entity>());
                }
                
            }
            
        }

        private void PublishHealthPercentage()
        {
            if (playerHealthChannel != null)
            {
                playerHealthChannel.Invoke(currentHealth/(float)maxHealth);    
                
            }
            
        }
    }
}