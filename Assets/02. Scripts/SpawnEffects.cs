using DG.Tweening;
using UnityEngine;

namespace Platformer
{
    [RequireComponent(typeof(AudioSource))]
    public class SpawnEffects : MonoBehaviour
    {
        [SerializeField] private GameObject spawnVFX;
        [SerializeField] private float animationDuration = 1f;

        /***
         * 오브젝트가 씬에 등장할때 효과
         * DOScale: DOTween에서 제공하는 "트윈 애니메이션"
         * transform.localScale을 Vector3.one까지 애니메이션으로 서서히 변화
         * animationDuration: 이 변화에 걸리는 시간
         * .SetEase(Ease.OutBack):
         * Ease 종류: 애니메이션의 움직임 곡선(속도 느낌)
         * OutBack: 보통 튕기듯이 커졌다가 원래 크기로 돌아오는 애니메이션 (약간의 반동 효과 있음)
         *
         * Instantiate: 게임 오브젝트를 복제해서 씬에 생성
         * spawnVFX: 이펙트 프리팹
         * transform.position: 이 오브젝트의 위치
         * Quaternion.identity: 회전 없음 (0도, 원래 방향)
         */
        void Start()
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, animationDuration).SetEase(Ease.OutBack);

            if (spawnVFX != null)
            {
                Instantiate(spawnVFX, transform.position, Quaternion.identity);
            }

            //RequireComponent(typeof(AudioSource)) 덕분에 AudioSource가 무조건 붙어 있어야 컴파일 에러 없이 실행됨
            GetComponent<AudioSource>().Play();
        }
    }
}