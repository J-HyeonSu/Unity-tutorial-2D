using UnityEngine;

namespace Platformer
{
    public class Collectible : Entity
    {
        [SerializeField] private int score = 10;
        [SerializeField] private IntEventChannel scoreChannel;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                scoreChannel.Invoke(score);
                Destroy(gameObject);
            }
        }
    }
}