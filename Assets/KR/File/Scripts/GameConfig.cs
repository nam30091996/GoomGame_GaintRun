using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using KR;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class GameConfig : KR.ScriptableSingleton<GameConfig>
{
#if UNITY_EDITOR
    [MenuItem("KR/GameConfig")]
    public static void CreateConfig()
    {
        KR.Scriptable.CreateAsset<GameConfig>("Assets/KR/Resources/");
    }

    [MenuItem("KR/Clear PlayerPrefs")]
    public static void ClearPrefs()
    {
        UnityEngine.PlayerPrefs.DeleteAll();


    }
#endif
    [System.Serializable]
    public class Data
    {
        public int maxLevel = 20;
        public List<Material> materials;
        public List<GameObject> pops;
        public List<GameObject> explosions;

        public List<GameObject> fireworks;
    }

    public Data data;
 

}


