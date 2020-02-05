using Moon.Asyncs;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyController : MonoBehaviour
{
    [SerializeField] DifficultConfig config;
    [SerializeField] HPManager hpManager;

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
                ;
        }

        asyncChain.AddAction(Debug.Log, "Show");
        asyncChain.AddTimeout(Config.showTime);
        asyncChain.AddAction(Debug.Log, "Close");
        foreach (var cardP in cardsPool)
        {
            var card = cardP.GetComponent<Card>();
            asyncChain
                .JoinTween(card.ReRotate)
                .JoinTween(card.RePut);
        }

        return asyncChain;
    }

    public AsyncState HandleHP()
    {
        var asyncChain = Planner.Chain();
        if (!config.isHPHandle)
            return asyncChain.AddEmpty();

        asyncChain.AddFunc(hpManager.Execute, config.hpPrefab, config.maxHP);
        return asyncChain;
    }
}