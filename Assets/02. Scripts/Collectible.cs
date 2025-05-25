using System;
using KBCore.Refs;
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
                
                //오디오 매니저를 만드는게 좋을듯
                var ad  = GetComponent<AudioSource>();
                AudioSource.PlayClipAtPoint(ad.clip, transform.position);
                
                Destroy(gameObject);
            }
        }
        
    }
}