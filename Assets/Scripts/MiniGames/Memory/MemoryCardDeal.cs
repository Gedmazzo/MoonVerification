using DG.Tweening;
using Moon.Asyncs;
using UnityEngine;

public class MemoryCardDeal : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;

    public AsyncState CardDealing(int numberOfPairs)
    {
        var asyncChain = Planner.Chain();
        asyncChain.AddAction(Debug.Log, "Dealing started");

        var moveTargetPosition = new Vector3(-numberOfPairs, 0f, 0f);

        for (var i = 0; i < numberOfPairs; i++)
        {
            asyncChain
                    .AddFunc(InstantiateCard, moveTargetPosition)
                ;

            var offset = 2f;
            moveTargetPosition = new Vector3(moveTargetPosition.x + offset, 0f, 0f);
        }

        asyncChain.AddAction(Debug.Log, "Dealing finished");
        return asyncChain;
    }

    private AsyncState InstantiateCard(Vector3 moveTargetPosition)
    {
        var asyncChain = Planner.Chain();

        var offset = 10f;
        var cardInstance0 = Instantiate(cardPrefab, Vector3.forward * offset, cardPrefab.transform.rotation, transform);
        var cardInstance1 = Instantiate(cardPrefab, Vector3.forward * offset, cardPrefab.transform.rotation, transform);

        var moveTargetPosition0 = new Vector3(moveTargetPosition.x + cardInstance0.transform.localScale.x, 0f, 0f);
        var moveTargetPosition1 = new Vector3(moveTargetPosition0.x + cardInstance0.transform.localScale.x, 0f, 0f);

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
}