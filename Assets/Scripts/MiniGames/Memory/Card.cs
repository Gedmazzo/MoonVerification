using DG.Tweening;
using Moon.Asyncs;
using System;
using UnityEngine;

public class Card : MonoBehaviour
{
    private MeshRenderer faceImage;

    private Vector3 originPosition;
    private Vector3 originRotation;

    private bool isFliped;
    public static bool IsTweenRunning { get; private set; }

    private void OnEnable()
    {
        isFliped = false;
    }

    private void Awake()
    {
        faceImage = transform.GetChild(0).GetComponent<MeshRenderer>();
    }

    private void OnMouseDown()
    {
        if (isFliped || IsTweenRunning || MemoryCardDeal.IsDealing || MemoryCardDeal.IsHandleFlipCards)
            return;

        Flip();
        TutorialHand.onClickCard?.Invoke();
    }

    public void Flip()
    {
        isFliped = true;
        var asyncChain = Planner.Chain();
        asyncChain
            .AddTween(Rise)
            .AddTween(Rotate)
            .AddTween(Put)
            .AddAction(MemoryCardDeal.OnFlipedCard.Invoke, this)
        ;
    }

    public AsyncState MoveToTable(Vector3 movePosition)
    {
        return Planner.Chain()
                    .AddTween(MoveTo, movePosition)
                ;
    }

    public void SetImage(Texture2D image)
    {
        faceImage.material.mainTexture = image;
    }

    public Texture2D GetImage()
    {
        return (Texture2D)faceImage.material.mainTexture;
    }

    public AsyncState SetActiveGameObject(bool isActive)
    {
        return Planner.Chain()
            .AddAwait((AsyncStateInfo state) => state.IsComplete = !IsTweenRunning)
            .AddAction(gameObject.SetActive, isActive);
    }

    #region TWEENS

    public Tween MoveTo(Vector3 movePosition)
    {
        IsTweenRunning = true;
        var tween = transform
                    .DOMove(movePosition, 1f)
                    .SetEase(Ease.InExpo);
        tween.onComplete += () => IsTweenRunning = false;
        return tween;
    }

    public Tween MoveTo(Vector3 movePosition, float duration, Ease easyType)
    {
        IsTweenRunning = true;
        var tween = transform
                    .DOMove(movePosition, duration)
                    .SetEase(easyType);
        tween.onComplete += () => IsTweenRunning = false;
        return tween;
    }

    public Tween Rise()
    {
        IsTweenRunning = true;
        originPosition = transform.position;
        var tween = transform
                .DOMove(transform.position + Vector3.up * .5f, 1f);
        return tween;
    }

    public Tween ReRotate()
    {
        return transform
                .DOLocalRotateQuaternion(Quaternion.Euler(originRotation), 1f);
    }

    public Tween Rotate()
    {
        originRotation = transform.eulerAngles;
        return transform
                .DOLocalRotateQuaternion(Quaternion.Euler(90f, 0f, 0f), 1f);
    }

    public Tween RePut()
    {
        isFliped = false;
        var tween = transform
                .DOMove(originPosition, 1f);
        tween.onComplete += () => IsTweenRunning = false;
        return tween;
    }

    public Tween Put()
    {
        var tween = transform
                .DOMove(originPosition, 1f);
        tween.onComplete += () => IsTweenRunning = false;
        return tween;
    }

    public Tween Shake()
    {
        IsTweenRunning = true;
        var tween = transform.DOPunchRotation(Vector3.up * 30f, 1f);
        tween.onComplete += () => IsTweenRunning = false;
        return tween;
    }

    #endregion

    #region OVERRIDED

    public override bool Equals(object other)
    {
        var otherCard = other as Card;
        return faceImage.material.mainTexture.name == otherCard.faceImage.material.mainTexture.name;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    #endregion
}