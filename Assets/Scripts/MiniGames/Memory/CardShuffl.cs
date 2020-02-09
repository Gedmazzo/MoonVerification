using DG.Tweening;
using Moon.Asyncs;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CardShuffl : MonoBehaviour
{
    private int maxErrors;
    public static Action onErrorCard;
    public bool IsShuffle { get; private set; }

    public List<GameObject> Cards { private get; set; }

    private void OnEnable()
    {
        onErrorCard += ErrorCard;
    }

    public void SetMaxErrors(int maxErrors)
    {
        this.maxErrors = maxErrors;
    }

    public void ErrorCard()
    {
        maxErrors--;
        if (maxErrors == 0)
            Shuffle();
    }

    public AsyncState Shuffle()
    {
        var asyncChain = Planner.Chain();
        asyncChain.AddEmpty();

        asyncChain.AddAwait((AsyncStateInfo state) => state.IsComplete = !Card.IsTweenRunning);
        IsShuffle = true;
        asyncChain.AddAction(Debug.Log, "Start shuffl");

        var activeCards = Cards.FindAll(c => c.activeSelf == true);

        for (int i = 0; i < activeCards.Count; i++)
        {
            var j = UnityEngine.Random.Range(i, activeCards.Count);

            asyncChain.AddFunc(Switching, activeCards, i, j);
            asyncChain.AddAwait((AsyncStateInfo state) => state.IsComplete = !Card.IsTweenRunning);
        }

        asyncChain.AddAction(Debug.Log, "End Shuffle");
        return asyncChain;
    }

    private AsyncState Switching(List<GameObject> list, int i, int j)
    {
        var asyncChain = Planner.Chain();

        var temp = list[i];
        list[i] = list[j];
        list[j] = temp;

        asyncChain.AddTween(list[j].GetComponent<Card>().MoveTo, list[i].transform.position, 0.25f, Ease.InSine);
        asyncChain.AddTween(list[i].GetComponent<Card>().MoveTo, list[j].transform.position, 0.25f, Ease.InSine);
        asyncChain.AddAwait((AsyncStateInfo state) => state.IsComplete = !Card.IsTweenRunning);
        return asyncChain;
    }

    private void OnDisable()
    {
        onErrorCard -= ErrorCard;
    }
}