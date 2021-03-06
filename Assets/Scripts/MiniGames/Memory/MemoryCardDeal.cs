﻿using Moon.Asyncs;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MemoryCardDeal : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private int countFlipCardATime = 2;
    [SerializeField] private CardShuffl cardShuffl;

    private DifficultyController difficultyController;
    private AudioManager audioManager;
    public Button HelpButton { get; set; }
    public bool IsFinishGameRound { get; private set; }

    public static bool IsDealing { get; private set; }
    public static bool IsHandleFlipCards { get; private set; }
    public int MaxHelpCount { get; set; }

    public static Action<Card> OnFlipedCard;

    private void OnEnable()
    {
        OnFlipedCard += AddToFlipedList;
    }

    private List<GameObject> cardsPool = new List<GameObject>();
    private List<Card> flipedCards = new List<Card>();

    private Texture2D[] images;

    private void SetCardShufflData()
    {
        if (cardShuffl != null)
        {
            if (!difficultyController.Config.isShufflingCards)
                CardShuffl.onErrorCard = null;

            cardShuffl.SetMaxErrors(difficultyController.Config.maxCountErrors);
            cardShuffl.Cards = cardsPool;
        }
    }

    public void SetAudioManager(AudioManager audioManager)
    {
        this.audioManager = audioManager;
    }

    public void SetDifficultyController(DifficultyController controler)
    {
        difficultyController = controler;
        SetCardShufflData();
    }

    public void SetImages(Texture2D[] images)
    {
        this.images = images;
    }

    public AsyncState CardDealing(int numberOfPairs)
    {
        if (HelpButton != null)
            HelpButton.interactable = true;

        IsFinishGameRound = false;
        var asyncChain = Planner.Chain();
        asyncChain.AddAction(Debug.Log, "Dealing started");
        IsDealing = true;

        var movePosition = new Vector3(-numberOfPairs / 2f, 2.86f, 0f);
        var moveOffset = Vector3.right * 0.5f;
        for (var i = 0; i < numberOfPairs * 2; i++)
        {
            Card card = null;

            var instancePosition = new Vector3(0f, 2.86f, 10f);
            var instanceRotation = cardPrefab.transform.rotation;

            if (cardsPool.Count > i && !cardsPool[i].activeSelf)
            {
                cardsPool[i].transform.position = instancePosition;
                cardsPool[i].transform.rotation = instanceRotation;

                card = cardsPool[i].GetComponent<Card>();
                card.SetImage(GetImage(i));
                card.gameObject.SetActive(true);
                card.AudioManager = audioManager;
                asyncChain.AddFunc(card.MoveToTable, movePosition);

                movePosition += moveOffset;
                continue;
            }

            var cardObj = Instantiate(cardPrefab, instancePosition, instanceRotation, transform);
            cardsPool.Add(cardObj);

            card = cardObj.GetComponent<Card>();
            card.SetImage(GetImage(i));
            card.AudioManager = audioManager;
            asyncChain.AddFunc(card.MoveToTable, movePosition);

            movePosition += moveOffset;
        }

        asyncChain.AddFunc(difficultyController.ShowCardsInBeginning, cardsPool);
        asyncChain.AddAction(Debug.Log, "Dealing finished");
        asyncChain.onComplete += () => IsDealing = false;
        return asyncChain;
    }

    private Texture2D GetImage(int index)
    {
        Texture2D image = null;
        if (difficultyController.Config.isCardWithoutPairs)
            return images[UnityEngine.Random.Range(0, images.Length)];

        if (cardsPool.Count == 1 || index < cardsPool.Count / 2)
            image = images[UnityEngine.Random.Range(0, images.Length)];
        else
        {
            var ind = UnityEngine.Random.Range((index - cardsPool.Count / 2), cardsPool.Count / 2);
            image = cardsPool[ind].GetComponent<Card>().GetImage();
        }

        return image;
    }

    private void AddToFlipedList(Card card)
    {
        flipedCards.Add(card);
        if (flipedCards.Count == countFlipCardATime)
            HandleFlipedCards();
    }

    private void HandleFlipedCards()
    {
        var asyncChain = Planner.Chain();
        asyncChain.AddAwait((AsyncStateInfo state) => state.IsComplete = !Card.IsTweenRunning);

        IsHandleFlipCards = true;
        var matches = new Dictionary<Card, List<Card>>();
        foreach (var fCard in flipedCards)
        {
            if (!matches.ContainsKey(fCard))
                matches.Add(fCard, flipedCards.FindAll(c => c.Equals(fCard)));
        }

        var isMatch = false;
        foreach (var mCard in matches.Values)
        {
            if (mCard.Count > 1)
            {
                isMatch = true;
                mCard.ForEach((Card card)
                    => asyncChain
                            .JoinTween(card.Shake)
                            .JoinTween(card.MoveTo, new Vector3(0f, 2.86f, 10f))
                            .JoinFunc(card.SetActiveGameObject, false)
                      );
            }
            else
            {
                asyncChain
                    .AddTween(mCard[0].Rise)
                    .AddTween(mCard[0].ReRotate)
                    .AddTween(mCard[0].RePut)
                ;
            }
        }

        if (!isMatch)
        {
            asyncChain.AddAction(() => HPManager.onDecrease?.Invoke());
            asyncChain.AddAction(() => CardShuffl.onErrorCard?.Invoke());
        }

        asyncChain.AddAction(() =>
        {
            var activeCards = cardsPool.FindAll(card => card.activeSelf);

            var isMatchesExists = false;
            foreach (var aCard in activeCards)
            {
                if (activeCards.FindAll(c => c.GetComponent<Card>().Equals(aCard.GetComponent<Card>())).Count > 1)
                    isMatchesExists = true;
            }

            if (!isMatchesExists)
            {
                foreach (var card in activeCards)
                {
                    var cardComponent = card.GetComponent<Card>();
                    asyncChain
                            .JoinTween(cardComponent.MoveTo, new Vector3(0f, 2.86f, 10f))
                            .JoinFunc(cardComponent.SetActiveGameObject, false)
                        ;

                    asyncChain.AddFunc(() => cardComponent.SetActiveGameObject(false));
                }
                asyncChain.AddAction(() => IsFinishGameRound = true);
            }
        });

        asyncChain.AddAwait((AsyncStateInfo state) => state.IsComplete = !Card.IsTweenRunning);
        asyncChain.AddAction(flipedCards.Clear);
        asyncChain.onComplete += () => IsHandleFlipCards = false;
    }

    public void HighlightMatcheCards()
    {
        var asyncChain = Planner.Chain();
        asyncChain.AddEmpty();
        var activeCards = cardsPool.FindAll(p => p.activeSelf);
        for (int i = 0; i < activeCards.Count; i++)
        {
            var matches = activeCards.FindAll(c
                    => c.GetComponent<Card>().GetImage()
                    .Equals(activeCards[i].GetComponent<Card>().GetImage())
                );

            if (matches.Count >= countFlipCardATime)
            {
                for (int j = 0; j < countFlipCardATime; j++)
                    asyncChain.JoinTween(matches[j].GetComponent<Card>().Shake);

                MaxHelpCount--;
                if (MaxHelpCount == 0 && HelpButton != null)
                    HelpButton.interactable = false;
                return;
            }
        }
    }

    private void OnDisable()
    {
        OnFlipedCard -= AddToFlipedList;
    }
}