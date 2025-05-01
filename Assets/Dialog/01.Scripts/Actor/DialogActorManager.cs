using System.Collections.Generic;
using UnityEngine;

namespace Dialog
{
    public class DialogActorManager : MonoBehaviour
    {
        #region Singleton

        public static DialogActorManager instance;
        public static bool destroyed = false;

        public static DialogActorManager Instance
        {
            get
            {
                if (instance == null && !destroyed)
                {
                    instance = GameObject.FindAnyObjectByType<DialogActorManager>();

                    if (instance == null)
                    {
                        Debug.LogError("DialogActorManager is not Exsist");
                    }
                }
                return instance;
            }
        }

        public void OnDestroy()
        {
            destroyed = true;
        }

        #endregion

        public Dictionary<string, Actor> actorDic = new();


        public void AddActor(string key, Actor actor)
        {
            if (actorDic.ContainsKey(key))
            {
                Debug.LogWarning($"actor name of {key} is arleady exsist.\nbut you still trying to add actor with key {key}");
                return;
            }
            actorDic.Add(key, actor);
        }

        public void RemoveActor(string key, Actor actor)
        {
            if (actorDic.ContainsKey(key))
                actorDic.Remove(key);
        }


        public bool TryGetActor(string key, out Actor actor)
        {
            return actorDic.TryGetValue(key, out actor);
        }
    }
}
