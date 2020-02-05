using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DifficultConfig))]
public class DifficultyConfigEditor : Editor
{
    private DifficultConfig targetObject;

    void OnEnable()
    {
        targetObject = (DifficultConfig)this.target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.BeginVertical();
        targetObject.isShowCardInBeginningRound = EditorGUILayout.Toggle("IsShowCardInBeginningRound", targetObject.isShowCardInBeginningRound);

        GUI.enabled = targetObject.isShowCardInBeginningRound;
        targetObject.showTime = EditorGUILayout.FloatField("ShowTime", targetObject.showTime);

        GUI.enabled = true;

        targetObject.isHPHandle = EditorGUILayout.Toggle("Is HP Handle", targetObject.isHPHandle);
        GUI.enabled = targetObject.isHPHandle;
        targetObject.maxHP = EditorGUILayout.IntField("Max HP", targetObject.maxHP);
        targetObject.hpPrefab = (GameObject)EditorGUILayout.ObjectField((Object)targetObject.hpPrefab, typeof(Object), true);

        GUI.enabled = true;

        targetObject.isShufflingCards = EditorGUILayout.Toggle("Is Shuffling Cards", targetObject.isShufflingCards);
        GUI.enabled = targetObject.isShufflingCards;
        targetObject.maxCountErrors = EditorGUILayout.IntField("Max erros", targetObject.maxCountErrors);

        GUI.enabled = true;

        targetObject.isCardWithoudPairs = EditorGUILayout.Toggle("IsCardWithoudPairs", targetObject.isCardWithoudPairs);

        EditorGUILayout.EndVertical();

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(target);
    }
}