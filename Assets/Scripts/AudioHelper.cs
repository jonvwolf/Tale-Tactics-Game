using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public static class AudioHelper
    {
        public static IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
        {
            float startVolume = audioSource.volume;
            while (audioSource.volume > 0)
            {
                audioSource.volume -= startVolume * Time.deltaTime / FadeTime;
                yield return null;
            }
            
            audioSource.volume = 0;
            audioSource.Stop();
            Debug.Log("Audio is faded out");
            yield break;
        }
        public static IEnumerator FadeIn(AudioSource audioSource, float FadeTime)
        {
            // The reason is set to 0, (different than image) is that is hard to notice the change
            // because the other sound was fading in, so the fadeout that was converted into fade in, will be suddenly 0 but
            // the other sound that was fading in but now fading out, is sounds ok, because you can't hardly notice the change
            audioSource.volume = 0;
            audioSource.Play();
            
            while (audioSource.volume < 1)
            {
                    audioSource.volume += Time.deltaTime / FadeTime;
                yield return null;
            }

            audioSource.volume = 1f;
            Debug.Log("Audio is faded in");
            yield break;
        }

        public static IEnumerator FadeInImage(Image image, float FadeTime)
        {
            while (image.color.a < 1)
            {
                var newColor2 = image.color;
                newColor2.a += Time.deltaTime / FadeTime;
                image.color = newColor2;
                yield return null;
            }

            var c = image.color;
            c.a = 1f;
            image.color = c;
            yield break;
        }

        public static IEnumerator FadeOutImage(Image image, float FadeTime)
        {
            float startAlpha = image.color.a;
            while (image.color.a > 0)
            {
                var newColor = image.color;
                newColor.a -= startAlpha * Time.deltaTime / FadeTime;
                image.color = newColor;
                yield return null;
            }

            var c = image.color;
            c.a = 0f;
            image.color = c;
            yield break;
        }

        public static IEnumerator FadeOutCanvasGroup(CanvasGroup cg, float FadeTime, Canvas canvas)
        {
            Debug.Log("CG alpha: " + cg.alpha);
            float startAlpha = cg.alpha;
            while (cg.alpha > 0)
            {
                cg.alpha -= startAlpha * Time.deltaTime / FadeTime;
                yield return null;
            }

            cg.alpha = 0f;
            canvas.gameObject.SetActive(false);
            yield break;
        }
    }
}
