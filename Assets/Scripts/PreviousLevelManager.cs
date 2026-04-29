using UnityEngine;

public class PreviousManager : BaseSceneManager
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            LoadScene(nowSceneIndex - 1);
            collision.transform.position = new Vector2(-17f, collision.transform.position.y);
            gameObject.SetActive(false);
        }
    }
}
