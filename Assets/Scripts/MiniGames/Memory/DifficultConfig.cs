using UnityEngine;

[CreateAssetMenu(fileName = "Difficult Config")]
public class DifficultConfig : ScriptableObject
{
    public bool isShowCardInBeginningRound;
    public float showTime; 

    public bool isHPHandle;
    public int maxHP;
    public GameObject hpPrefab;

    public bool isShufflingCards;
    public int maxCountErrors;

    public bool isCardWithoutPairs;
}