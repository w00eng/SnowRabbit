using UnityEngine;

public class MonsterAI_FlyingMonster : MonoBehaviour
{
    public LayerMask layerPlayer;

    private void Start()
    {
        
    }

    void Update()
    {
        bool a = Physics2D.OverlapBox(transform.position, new Vector2(1f, 1f), layerPlayer);
    }
}
