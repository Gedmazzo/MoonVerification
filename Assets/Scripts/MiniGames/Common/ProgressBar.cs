using DG.Tweening;
using Moon.Asyncs;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private Image barBack;
    [SerializeField] private Image barFront;

    [SerializeField] private float fillAmountDuration = 2f;

    private ParticleSystem effectInstance;


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
                    .AddAction(ShowParticleEffect)
                    .AddAction(ToggleBarImages, true)
                    .AddTween(ShowEffect)
                ;
    }

    public AsyncState Close()
    {
        return Planner.Chain()
                    .AddAction(ShowParticleEffect)
                    .AddTween(CloseEffect)
                    .AddAction(ToggleBarImages, false)
                    .AddAwait((AsyncStateInfo state) => state.IsComplete = effectInstance == null)
                ;
    }

    public void ShowParticleEffect()
    {
        effectInstance = Instantiate(effectPrefab, transform).GetComponent<ParticleSystem>();
        var totalDuration = effectInstance.main.duration + effectInstance.main.startLifetime.constant;
        Destroy(effectInstance.gameObject, totalDuration);
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