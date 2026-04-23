using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public Vector2 spawnPos;
    private float groundOffset = 0.87f;

    void Start()
    {
        LayerMask groundLayer = LayerMask.GetMask("Ground");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 100f, groundLayer);
        spawnPos = new Vector2(hit.point.x, hit.point.y + groundOffset);
    }
}
