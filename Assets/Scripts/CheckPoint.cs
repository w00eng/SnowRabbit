using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private int checkPointIndex = -1;

    [Space(10)]
    [SerializeField] private Vector3 VFXPositionOffset = new Vector3(0, 1.43f, 0);
    [SerializeField] private GameObject unactivatedVFXPrefab;
    private GameObject unactivatedVFX;
    [SerializeField] private GameObject activatedVFXPrefab;

    [Space(10)]
    [SerializeField] private GameObject playerPrefab;

    public Vector2 SpawnPos { get; private set; }
    private BoxCollider2D _box;

    private void Awake()
    {
        if (PlayerManager.Instance == null)
        {
            Instantiate(playerPrefab);
        }

        _box = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        if (DataManager.Instance.NowPlayData.CheckPoints[checkPointIndex])
        {
            _box.enabled = false;
            Instantiate(activatedVFXPrefab, transform.position + VFXPositionOffset, Quaternion.identity);
            return;
        }

        LayerMask groundLayer = LayerMask.GetMask("Ground");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 100f, groundLayer);

        // 0.87f: Y position gap between position of the player character and position of the actual collision with the ground
        SpawnPos = new Vector2(hit.point.x, hit.point.y + 0.87f);

        unactivatedVFX = Instantiate(unactivatedVFXPrefab, transform.position + VFXPositionOffset, Quaternion.identity);
    }

    public void SetActivated()
    {
        _box.enabled = false;
        unactivatedVFX.SetActive(false);
        Instantiate(activatedVFXPrefab, transform.position + VFXPositionOffset, Quaternion.identity);

        DataManager.Instance.NowPlayData.CheckPoints[checkPointIndex] = true;
        DataManager.Instance.NowPlayData.lastSpawnPoint = SpawnPos;
        DataManager.Instance.NowPlayData.lastSceneIndex = GameManager.Instance.NowScene;
        Debug.Log("temporaryItemData Save");
        DataManager.Instance.NowPlayData.Items = DataManager.Instance.temporaryItemData;
        DataManager.Instance.SaveData();
    }
}
