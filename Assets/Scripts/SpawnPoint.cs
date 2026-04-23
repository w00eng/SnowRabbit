using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public Vector2 spawnPos;

    void Start()
    {
        LayerMask groundLayer = LayerMask.GetMask("Ground");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 100f, groundLayer);

        // 0.87f: Y position gap between position of the player character and position of the actual collision with the ground
        spawnPos = new Vector2(hit.point.x, hit.point.y + 0.87f);
    }
}
