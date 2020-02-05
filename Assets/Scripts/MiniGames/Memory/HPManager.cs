using Moon.Asyncs;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HPManager : MonoBehaviour
{
    public static Action onDecrease;

    private List<GameObject> hpPool = new List<GameObject>();

    private int maxHP;

    private void OnEnable()
    {
        onDecrease += Descrease;
    }

    public AsyncState Execute(GameObject prefab, int maxHP)
    {
        var asyncChain = Planner.Chain();
        asyncChain.AddEmpty();
        this.maxHP = maxHP;
        if (hpPool.Count == maxHP)
            return asyncChain;

        gameObject.SetActive(true);
        for (int i = 0; i < maxHP; i++)
        {
            var hp = Instantiate(prefab, transform);
            hpPool.Add(hp);
        }

        return asyncChain;
    }

    public void Descrease()
    {
        maxHP--;
        hpPool.FindLast(h => h.activeSelf == true).SetActive(false);
    }

    private void OnDisable()
    {
        onDecrease -= Descrease;
    }
}