using Unity.Cinemachine;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private Vector2 spawnPoint = Vector2.zero;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Spike"))
        {
            transform.position = spawnPoint;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Camera"))
        {
            collision.gameObject.GetComponent<CinemachineCamera>().Priority = 1;
        }
        
        if (collision.gameObject.CompareTag("Respawn"))
        {
            spawnPoint = collision.gameObject.GetComponent<SpawnPoint>().spawnPos;
            collision.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Camera"))
        {
            collision.gameObject.GetComponent<CinemachineCamera>().Priority = 0;
            collision.gameObject.GetComponent<BoxCollider2D>().isTrigger = false;
        }
    }
}
