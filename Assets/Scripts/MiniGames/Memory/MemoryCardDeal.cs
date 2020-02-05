using Moon.Asyncs;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MemoryCardDeal : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private int countFlipCardATime = 2;

    private DifficultyController difficultyController;

    public bool IsFinishGameRound { get; private set; }

    public static bool IsDealing { get; private set; }
    public static bool IsHandleFlipCards { get; private set; }

    public static Action<Card> OnFlipedCard;

    private void OnEnable()
    {
        OnFlipedCard += AddToFlipedList;
    }

    private List<GameObject> cardsPool = new List<GameObject>();
    private List<Card> flipedCards = new List<Card>();

    private Texture2D[] images;

    public void SetDifficultyController(DifficultyController controler)
    {
        difficultyController = controler;
    }

    public void SetImages(Texture2D[] images)
    {
        this.images = images;
    }

    public AsyncState CardDealing(int numberOfPairs)
    {
        IsFinishGameRound = false;
        var asyncChain = Planner.Chain();
        asyncChain.AddAction(Debug.Log, "Dealing started");
        IsDealing = true;

        var movePosition = new Vector3(-numberOfPairs, .1f, 0f);
        for (var i = 0; i < numberOfPairs * 2; i++)
        {
            Card card = null;

            var instancePosition = Vector3.forward * 10f;
            var instanceRotation = cardPrefab.transform.rotation;

            if (cardsPool.Count > i && !cardsPool[i].activeSelf)
            {
                cardsPool[i].transform.position = instancePosition;
                cardsPool[i].transform.rotation = instanceRotation;

                card = cardsPool[i].GetComponent<Card>();
                card.SetImage(GetImage());
                card.gameObject.SetActive(true);

                asyncChain.AddFunc(card.MoveToTable, movePosition);

                movePosition += Vector3.right;
                continue;
            }

            var cardObj = Instantiate(cardPrefab, instancePosition, instanceRotation, transform);
            cardsPool.Add(cardObj);

            card = cardObj.GetComponent<Card>();
            card.SetImage(GetImage());
            asyncChain.AddFunc(card.MoveToTable, movePosition);

            movePosition += Vector3.right;
        }

        asyncChain.AddFunc(difficultyController.ShowCardsInBeginning, cardsPool);
        asyncChain.AddAction(Debug.Log, "Dealing finished");
        asyncChain.onComplete += () => IsDealing = false;
        return asyncChain;
    }

    private Texture2D GetImage()
    {
        return images[UnityEngine.Random.Range(0, images.Length)];
    }

    private void AddToFlipedList(Card card)
    {
        flipedCards.Add(card);
        if (flipedCards.Count == countFlipCardATime)
        {
            HandleFlipedCards();
        }
    }

    private void HandleFlipedCards()
    {
        IsHandleFlipCards = true;
        var asyncChain = Planner.Chain();
        asyncChain.AddAwait((AsyncStateInfo state) => state.IsComplete = !Card.IsTweenRunning);
        for (int i = 0; i < flipedCards.Count; i++)
        {
            var matches = new List<Card>();
            matches.Add(flipedCards[i]);
            for (int j = i + 1; j < flipedCards.Count; j++)
            {
                if (matches[0].Equals(flipedCards[j]))
                    matches.Add(flipedCards[j]);
            }

            if (matches.Count > 1)
            {
                flipedCards = flipedCards.Except(matches).ToList();
                foreach (var m in matches)
                {
                    asyncChain
                        .JoinTween(m.Hide)
                        .JoinTween(m.MoveTo, Vector3.forward * 10f)
                        .JoinFunc(m.SetActiveGameObject, false)
                    ;
                }
            }

            var activeCards = cardsPool.FindAll(g => g.activeSelf);
            if (activeCards.Count <= countFlipCardATime)
            {
                foreach (var card in activeCards)
                {
                    var m = card.GetComponent<Card>();
                    
                    asyncChain
                        .JoinTween(m.MoveTo, Vector3.forward * 10f)
                        .JoinFunc(m.SetActiveGameObject, false)
                    ;
                }
                asyncChain.AddAction(() => IsFinishGameRound = true);
                break;
            }
            else
            {
                foreach (var f in flipedCards)
                {
                    asyncChain
                        .AddTween(f.Rise)
                        .AddTween(f.ReRotate)
                        .AddTween(f.RePut)
                    ;
                }

                break;
            }
        }

        flipedCards.Clear();
        asyncChain.onComplete += () => IsHandleFlipCards = false;
    }

    private void OnDisable()
    {
        OnFlipedCard -= AddToFlipedList;
    }
}