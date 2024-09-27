using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T instance;
    public static bool isDestroyed = false;

    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                instance = GameObject.FindAnyObjectByType<T>();

                if(instance == null || isDestroyed)
                {
                    Debug.LogError($"Singleton not Exist");
                }
            }

            return instance;
        }
    }

    private void OnDestroy()
    {
        isDestroyed = true;
    }
}
