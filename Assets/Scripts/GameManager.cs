using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static int currentSceneIndex = 0;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
