using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoInputWarning : MonoBehaviour
{
    [SerializeField] float timeToWaitForWarning;
    [SerializeField] Image backgroundImage;
    [SerializeField] TMP_Text warningTMPText;
    float timeSinceLastTouch = 0f;
    bool warned = false;
    Tween textFade, imageFade;

    void Awake()
    {
        TextFade(0f, 0f);
        ImageFade(0f, 0f);
    }

    void Update()
    {
        if (Input.touchCount == 0)
        {
            timeSinceLastTouch += Time.deltaTime;

            if (timeSinceLastTouch >= timeToWaitForWarning && !warned)
            {
                warned = true;
                Debug.LogWarning("No touch detected for " + timeToWaitForWarning + " seconds!");
                textFade = TextFade(1f, 1f);
                imageFade = ImageFade(1f, 1f);
            }
        }
        else
        {
            timeSinceLastTouch = 0f;
            warned = false;
            TextFade(0f, 1f);
            textFade.Kill();
            ImageFade(0f, 1f).OnComplete(() => imageFade.Kill());
        }
    }

    Tween TextFade(float targetFade, float timeToFade)
    {
        return warningTMPText.DOFade(targetFade, timeToFade).SetLoops(-1, LoopType.Yoyo);
    }

    Tween ImageFade(float targetFade, float timeToFade)
    {
        return backgroundImage.DOFade(targetFade, timeToFade);
    }
}
