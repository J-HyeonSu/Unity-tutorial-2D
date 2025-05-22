using UnityEngine;
using DG.Tweening;

namespace Platformer
{
    public class PlatformMover : MonoBehaviour
    {
        //플렛폼 무빙 구현
        
        [SerializeField] private Vector3 moveTo = Vector3.zero;
        [SerializeField] private float moveTime = 1f;
        [SerializeField] private Ease ease = Ease.InOutQuad;

        private Vector3 startPosition;

        void Start()
        {
            //현재위치 초기화
            startPosition = transform.position;
            Move();
        }

        void Move()
        {
            /***
             * DOTween라이브러리를 사용하는 코드. 유니티에서 오브젝트를 부드럽게 움직일때 쓰임
             * startPosition + moveTo 까지 moveTime초동안 움직임
             * 
             * Ease.Linear → 속도가 일정 (처음부터 끝까지 똑같음)
             * Ease.InOutQuad → 처음엔 느리게, 중간은 빠르게, 끝에 다시 느리게 (부드러운 가속-감속)
             * Ease.OutBounce → 튕기듯이 끝남
             *
             * SetLoops 무한반복(-1)
             * LoopType.Yoyo 왔다갔다
             * LoopType.Restart 끝까지간다음 다시 처음부터
             */
            
            transform.DOMove(startPosition + moveTo, moveTime)
                .SetEase(ease)
                .SetLoops(-1, LoopType.Yoyo);
            
        }
    }
}