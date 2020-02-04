using DG.Tweening;
using Moon.Asyncs;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MemoryCardDealCopy : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;

    private Func<CardCopy, int> onAddCardToList;
    private List<CardCopy> currentTurnedCards = new List<CardCopy>();

    private void OnEnable()
    {
        onAddCardToList += AddToList;
    }

    public AsyncState CardDealing(int numberOfPairs)
    {
        var asyncChain = Planner.Chain();
        asyncChain.AddAction(Debug.Log, "Dealing started");

        var moveTargetPosition = new Vector3(-numberOfPairs, .1f, 0f);

        for (var i = 0; i < numberOfPairs; i++)
        {
            asyncChain.AddFunc(InstantiateCard, moveTargetPosition);

            var offset = 2f;
            moveTargetPosition += Vector3.right * offset;
        }
        asyncChain.AddAction(ChangeClickAllowEvent, true);
        
        return asyncChain;
    }

    private AsyncState InstantiateCard(Vector3 moveTargetPosition)
    {
        var asyncChain = Planner.Chain();

        var offset = 10f;
        var cardInstance0 = Instantiate(cardPrefab, Vector3.forward * offset, cardPrefab.transform.rotation, transform);
        var moveTargetPosition0 = moveTargetPosition + (Vector3.right * cardInstance0.transform.localScale.x);
        cardInstance0.GetComponent<CardCopy>().OnAddCardToTurnedList = onAddCardToList;

        var cardInstance1 = Instantiate(cardPrefab, Vector3.forward * offset, cardPrefab.transform.rotation, transform);
        var moveTargetPosition1 = moveTargetPosition0 + (Vector3.right * cardInstance1.transform.localScale.x);
        cardInstance1.GetComponent<CardCopy>().OnAddCardToTurnedList = onAddCardToList;

        asyncChain
                .AddTween(Move, cardInstance0.transform, moveTargetPosition0)
                .AddTween(Move, cardInstance1.transform, moveTargetPosition1)
            ;

        return asyncChain;
    }

    private Tween Move(Transform cardInstance, Vector3 targetPosition)
    {
        return cardInstance
                    .DOMove(targetPosition, 1f)
                    .SetEase(Ease.InExpo)
                ;
    }

    private int AddToList(CardCopy card)
    {
        currentTurnedCards.Add(card);
        if (currentTurnedCards.Count == 2)
        {
            FindAndRemoveMatches();
        }

        return currentTurnedCards.Count;
    }

    private void FindAndRemoveMatches()
    {
        var asyncChain = Planner.Chain();
        var matches = new List<CardCopy>();

        for (var i = 0; i < currentTurnedCards.Count; i++)
        {
            matches.Add(currentTurnedCards[i]);
            for (var j = i + 1; j < currentTurnedCards.Count; j++)
            {
                if (matches[0].Equals(currentTurnedCards[j]))
                {
                    matches.Add(currentTurnedCards[j]);
                    currentTurnedCards.RemoveAt(j);
                }
            }

            if (matches.Count > 1)
                asyncChain.AddFunc(DissapearCards, matches);

            asyncChain.AddAction(matches.Clear);
        }
        asyncChain.AddAction(currentTurnedCards.Clear);
        asyncChain.AddAction(CardCopy.onClickAllowChanged.Invoke, true);
    }

    private AsyncState DissapearCards(List<CardCopy> matches)
    {
        var asyncChain = Planner.Chain();
        asyncChain.AddEmpty();

        foreach (var card in matches)
            asyncChain.AddAction(card.Dissapear);

        return asyncChain;
    }

    private void ChangeClickAllowEvent(bool isAllowed)
    {
        CardCopy.onClickAllowChanged?.Invoke(isAllowed);
    }

    private void OnDisable()
    {
        onAddCardToList -= AddToList;
    }
}