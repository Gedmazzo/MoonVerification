using Moon.Asyncs;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyController : MonoBehaviour
{
    [SerializeField] DifficultConfig config;
    public DifficultConfig Config { get => config; }

    public AsyncState ShowCardsInBeginning(List<GameObject> cardsPool)
    {
        var asyncChain = Planner.Chain();
        if (!config.isShowCardInBeginningRound)
            return asyncChain.AddEmpty();

        foreach (var cardP in cardsPool)
        {
            var card = cardP.GetComponent<Card>();
            asyncChain
                .JoinTween(card.Rise)
                .JoinTween(card.Rotate)
                .JoinTween(card.Put);
        }

        asyncChain.AddAction(Debug.Log, "Show");
        asyncChain.AddTimeout(Config.showTime);
        asyncChain.AddAction(Debug.Log, "Close");
        foreach (var cardP in cardsPool)
        {
            var card = cardP.GetComponent<Card>();
            asyncChain
                .JoinTween(card.Rise)
                .JoinTween(card.ReRotate)
                .JoinTween(card.RePut);
        }

        return asyncChain;
    }
}