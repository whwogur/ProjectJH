using UnityEngine;
using TMPro;
using System.Collections;

namespace JH
{
    public class PlayerUI_PopupManager : MonoBehaviour
    {
        [Header("Death Popup")]
        [SerializeField] GameObject deathPopupGameObject;
        [SerializeField] TextMeshProUGUI deathPopupBackgroundText;
        [SerializeField] TextMeshProUGUI deathPopupText;
        [SerializeField] CanvasGroup deathPopupCanvasGroup; // fade in / out

        public void SendDeathPopup()
        {
            // activate post processing effects

            deathPopupGameObject.SetActive(true);
            deathPopupBackgroundText.characterSpacing = 0;
            // strech out popup
            StartCoroutine(StretchPopupTextOverTime(deathPopupBackgroundText, 8, 0.3f));
            // fade in
            StartCoroutine(FadeInPopupOverTime(deathPopupCanvasGroup, 5));
            // fade out
            StartCoroutine(WaitAndFadeoutPopup(deathPopupCanvasGroup, 2, 5));
        }

        private IEnumerator StretchPopupTextOverTime(TextMeshProUGUI text, float duration, float strechAmount)
        {
            if (0.0f < duration)
            {
                text.characterSpacing = 0; // resets chrarcter spacing
                float timer = 0.0f;

                yield return null;

                while (timer < duration)
                {
                    timer += Time.deltaTime;
                    text.characterSpacing = Mathf.Lerp(text.characterSpacing, strechAmount, duration * (Time.deltaTime * 0.05f));
                    yield return null;
                }
            }
        }

        private IEnumerator FadeInPopupOverTime(CanvasGroup canvas, float duration)
        {
            if (0.0f < duration)
            {
                canvas.alpha = 0;
                float timer = 0.0f;
                yield return null;

                while (timer < duration)
                {
                    timer += Time.deltaTime;
                    canvas.alpha = Mathf.Lerp(canvas.alpha, 1, duration * Time.deltaTime);
                    yield return null;
                }
            }

            canvas.alpha = 1;

            yield return null;
        }

        private IEnumerator WaitAndFadeoutPopup(CanvasGroup canvas, float duration, float delay)
        {
            if (0.0f < duration)
            {
                while (delay > 0.0f)
                {
                    delay -= Time.deltaTime;
                    yield return null;
                }

                canvas.alpha = 1;
                float timer = 0.0f;
                yield return null;

                while (timer < duration)
                {
                    timer += Time.deltaTime;
                    canvas.alpha = Mathf.Lerp(canvas.alpha, 0, duration * Time.deltaTime);
                    yield return null;
                }
            }

            canvas.alpha = 0;

            yield return null;
        }
    }
}
