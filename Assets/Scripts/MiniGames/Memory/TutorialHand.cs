using DG.Tweening;
using Moon.Asyncs;
using System;
using UnityEngine;

public class TutorialHand : MonoBehaviour
{
    [SerializeField] private GameObject hand;
    [SerializeField] private Camera uICamera;
    private Card targetCard;
    private Vector3 offset = Vector3.zero;
    private Canvas canvas;

    public static Action onClickCard;

    private void OnEnable()
    {
        onClickCard += () => gameObject.SetActive(false);
    }

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    public  AsyncState StartTutorial()
    {
        var asyncChain = Planner.Chain();
        if (PlayerPrefs.HasKey("IsFirstStart"))
            return asyncChain.AddEmpty();

        asyncChain
                .AddAction(gameObject.SetActive, true)
                .AddAction(() => targetCard = FindObjectOfType<Card>())
                .AddAwait((AsyncStateInfo state) => state.IsComplete = targetCard != null)
                .AddFunc(MoveHand)
                .AddAction(() => PlayerPrefs.SetInt("IsFirstStart", 1))
            ;
        return asyncChain;
    }

    private AsyncState MoveHand()
    {
        var asyncChain = Planner.Chain();
        asyncChain.AddTween(MoveTo);
        return asyncChain;
    }

    private Tween MoveTo()
    {
        var position = WorldToUISpace(canvas, targetCard.transform.position);
        return hand.GetComponent<RectTransform>().DOMove(position, 1f);
    }

    public Vector3 WorldToUISpace(Canvas parentCanvas, Vector3 worldPos)
    {
        worldPos -= Vector3.one * 1.5f; //...
        Vector3 screenPos = uICamera.WorldToScreenPoint(worldPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, screenPos, parentCanvas.worldCamera, out Vector2 movePos);
        return parentCanvas.transform.TransformPoint(movePos);
    }

    private void OnDisable()
    {
        onClickCard -= () => gameObject.SetActive(false);
    }
}