using System.Collections;
using TMPro;
using UnityEngine;

namespace Platformer
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;

        void Start()
        {
            UpdateScore();
        }

        public void UpdateScore()
        {
            StartCoroutine(UpdateScoreNextFrame());
        }

        IEnumerator UpdateScoreNextFrame()
        {
            yield return null;
            scoreText.text = GameManager.Instance.Score.ToString();
        }
    }
}