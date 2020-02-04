using DG.Tweening;
using Moon.Asyncs;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image barBack;
    [SerializeField] private Image barFront;

    [SerializeField] private float fillAmountDuration = 2f;


    private void OnEnable()
    {
        ToggleBarImages(false);
    }

    private void Start()
    {
        transform.localScale = Vector3.zero;
    }

    public Tween SetCurrentValue(float value)
    {
        return barFront
                .DOFillAmount(barFront.fillAmount + value, fillAmountDuration)
            ;
    }

    public AsyncState Show()
    {
        return Planner.Chain()
                    .AddAction(ToggleBarImages, true)
                    .AddTween(ShowEffect)
                ;
    }

    public AsyncState Close()
    {
        return Planner.Chain()
                    .AddTween(CloseEffect)
                    .AddAction(ToggleBarImages, false)
                ;
    }

    private void ToggleBarImages(bool isEnable)
    {
        barBack.enabled = isEnable;
        barFront.enabled = isEnable;
    }

    public Tween ShowEffect()
    {
        return transform.DOScale(Vector3.one, 1f);
    }

    public Tween CloseEffect()
    {
        return transform.DOScale(Vector3.zero, 1f);
    }
}