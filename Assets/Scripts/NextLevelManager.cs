using UnityEngine;

public class NextLevelManager : BaseSceneManager
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            LoadScene(nowSceneIndex + 1);
            gameObject.SetActive(false);
        }
    }
}
