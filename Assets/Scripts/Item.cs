using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private int itemIndex = -1;


    void Start()
    {
        if (DataManager.Instance.NowPlayData.Items[itemIndex])
        {
            gameObject.SetActive(false);
        }
    }

    public void SetActivated()
    {
        DataManager.Instance.temporaryItemData[itemIndex] = true;

        gameObject.SetActive(false);
    }
}
