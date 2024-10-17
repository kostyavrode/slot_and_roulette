using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
namespace Mkey
{
    public class LampsController : MonoBehaviour
    {
        [SerializeField]
        private RectTransform lampsParent;
        private Image[] lamps;
        private int enabledCount = 0;
        private bool cancel = false;

        void Start()
        {
            enabledCount = 0;
            if (lampsParent)
            {
                lamps = lampsParent.GetComponentsInChildren<Image>();
            }

            if (lamps != null)
            {
                for (int i = 0; i < lamps.Length; i++)
                {
                    lamps[i].color = new Color(1, 1, 1, 0);
                    lamps[i].gameObject.SetActive(false);
                }

                StartCoroutine(Flashing());
            }

        }

        IEnumerator Flashing()
        {
            while (!cancel)
            {
                int lampI = UnityEngine.Random.Range(0, lamps.Length - 1);
                float lightDuration = UnityEngine.Random.Range(1, 4);

                if (enabledCount < 5)
                {
                    EnableLamp(lampI, lightDuration, null);
                }
                yield return new WaitForSeconds(0.02f);
            }
        }

        private void EnableLamp(int i, float lightDuration, Action completeCallBack)
        {
            if (!IndexOk(i) || lamps[i].gameObject.activeSelf)
            {
                if (completeCallBack != null) completeCallBack();
                return;
            }
            lamps[i].gameObject.SetActive(true);
            enabledCount++;
            float fadeTime = 0.2f * lightDuration;
            float sumLightDuration = lightDuration + fadeTime + fadeTime;

            SimpleTween.Value(gameObject, 0, 1, sumLightDuration).SetOnUpdate(
                (float val) =>
                {
                    if (val <= 0.2f)
                    {
                        if (lamps[i]) lamps[i].color = new Color(1, 1, 1, val * 5f);
                    }
                    else if (val >= 0.8f)
                    {
                        if (lamps[i]) lamps[i].color = new Color(1, 1, 1, (1.0f - val) * 5f);
                    }
                }).AddCompleteCallBack(()=> 
                {
                    if (lamps[i])
                    {
                        lamps[i].color = new Color(1, 1, 1, 0);
                        lamps[i].gameObject.SetActive(false);
                    }
                    enabledCount--;
                    if (completeCallBack != null) completeCallBack();
                }); 
        }

        private bool IndexOk(int i)
        {
            return ((i >= 0) && (i < lamps.Length));
        }

        public void CancelTween()
        {

            cancel = true;
            SimpleTween.Cancel(gameObject, true);
            StopCoroutine(Flashing());
        }

        private void OnDisable()
        {
            CancelTween();
        }

        private void OnDestroy()
        {
            CancelTween();
        }
    }
}