using DG.Tweening;
using Moon.Asyncs;
using System;
using UnityEngine;

public class CardCopy : MonoBehaviour
{
    private SpriteRenderer faceSpriteRenderer;
    private Vector3 position;
    private bool isClickAllowed;
    private bool isClicked;

    public Func<CardCopy, int> OnAddCardToTurnedList { private get; set; }
    public static Action<bool> onClickAllowChanged;

    private void OnEnable()
    {
        onClickAllowChanged += ChangeClickAllowed;
    }

    private void Awake()
    {
        faceSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (faceSpriteRenderer == null)
            Debug.LogError("SpriteRenderer is null");
    }

    private void OnMouseDown()
    {
        if (!isClickAllowed || isClicked)
            return;

        isClicked = true;
        Turn();
    }

    public void Dissapear()
    {
        var asyncChain = Planner.Chain();
        asyncChain
            .AddTween(ShakePosition)
            .AddAction(gameObject.SetActive, false)
        ;
    }

    private void Turn()
    {
        var asyncChain = Planner.Chain();
        asyncChain
            .AddAction(onClickAllowChanged.Invoke, false)
            .AddTween(Up)
            .AddTween(Rotate)
            .AddTween(Down)
            .AddAction(onClickAllowChanged.Invoke, true)
            .AddAction(AddToTurnedList)
        ;
    }

    private Tween Up()
    {
        position = transform.position;
        return transform
                .DOMove(transform.position + Vector3.up, 1f)
            ;
    }

    private Tween Rotate()
    {
        return transform
                .DOLocalRotateQuaternion(Quaternion.Euler(90f, 0f, 0f), 1f)
            ;
    }

    private Tween Down()
    {
        return transform
                .DOMove(position, 1f)
            ;
    }

    private Tween ShakePosition()
    {
        return transform
                .DOShakeRotation(1f)
            ;
    }

    private void AddToTurnedList()
    {
        var count = OnAddCardToTurnedList?.Invoke(this);
        if (count == 2)
            onClickAllowChanged?.Invoke(false);
    }

    private void ChangeClickAllowed(bool isClickAllowed)
    {
        this.isClickAllowed = isClickAllowed;
    }

    public override bool Equals(object other)
    {
        return faceSpriteRenderer.sprite.name == (other as CardCopy).faceSpriteRenderer.sprite.name;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    private void OnDisable()
    {
        onClickAllowChanged -= ChangeClickAllowed;
    }
}