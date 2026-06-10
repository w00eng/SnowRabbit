using UnityEngine;
using System.IO;

public class PlayData
{
    public bool[] CheckPoints = new bool[20];
    public Vector2 lastSpawnPoint = new Vector2(0, -6.13f);
    public int lastSceneIndex = 3;
    public bool[] Items = new bool[20];
}

public class DataManager : MonoBehaviour
{
    private static DataManager instance;
    public static DataManager Instance {  get { return instance; } }

    public PlayData NowPlayData { get; set; } = new PlayData();
    public bool[] temporaryItemData;

    private string path;
    public int SaveSlot { private get; set; } = -1;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        path = Application.persistentDataPath + "/saveFile";
    }

    public void SaveData()
    {
        string filePath = path + SaveSlot.ToString();
        //Debug.Log("저장 위치: " + filePath);

        string data = JsonUtility.ToJson(NowPlayData);

        if (!File.Exists(filePath))
        {
            FileStream fileStream = new FileStream(filePath, FileMode.Create);
            fileStream.Dispose();
            File.WriteAllText(filePath, data);
        }
        else
        {
            File.WriteAllText(filePath, data);
        }
}

    public void LoadData()
    {
        string filePath = path + SaveSlot.ToString();

        if (File.Exists(filePath))
        {
            string data = File.ReadAllText(filePath);
            NowPlayData = JsonUtility.FromJson<PlayData>(data);
            temporaryItemData = NowPlayData.Items;
        }
    }
}
