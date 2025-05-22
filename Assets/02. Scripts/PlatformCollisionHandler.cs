using UnityEngine;

namespace Platformer
{
    public class PlatformCollisionHandler : MonoBehaviour
    {
        private Transform platform;

        void OnCollisionEnter(Collision other)
        {
            //충돌시 MovingPaltform과 같다면
            if (other.gameObject.CompareTag("MovingPlatform"))
            {
                //충돌지점중 첫번째 접촉점 가져오기
                ContactPoint contact = other.GetContact(0);
                //법선 벡터의 y값이 0.5보다 작으면 리턴
                if (contact.normal.y < 0.5f) return;
                
                //발판을 이 오브젝트의 부모로 설정해서 캐릭터와 함께 움직이게 함
                platform = other.transform;
                transform.SetParent(platform);
            }
        }

        void OnCollisionExit(Collision other)
        {
            if (other.gameObject.CompareTag("MovingPlatform"))
            {
                transform.SetParent(null);
                platform = null;
            }
        }
    }
}