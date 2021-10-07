using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class GameManager : MonoBehaviour
{
    static public GameManager instance;
    public TextMeshProUGUI text;
    public GameObject pauseScreen;
    public Button restartButton;
    public Collider2D leftGameBounds, rightGameBounds;
    public bool isPaused;
    public bool win;
    SoundManager soundManager;
    

    private void Awake()
    {
        instance = this;
        text.text = "Together FOREVER!";
        soundManager = FindObjectOfType<SoundManager>();
    }

    public void Win()
    {
        EndGame();
    }

    public void EndGame()
    {
        if (win) return;
        text.text = "Together FOREVER\n\nIn HEAVEN! :)";
        GetComponent<Animator>().SetBool("win", true);
        soundManager.Play(soundManager.win);
        win = true;
    }

    public void Death()
    {
        text.text = "Together FOREVER\n\nIn HELL! :(";        
        GetComponent<Animator>().SetBool("death", true);
        soundManager.Play(soundManager.death);
    }    

    public void Reset()
    {
        SceneManager.LoadScene("Old Game");
        if (isPaused) PauseUnpauseGame();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PauseUnpauseGame()
    {
        if (isPaused == false)
        {
            Time.timeScale = 0;
            pauseScreen.gameObject.SetActive(true);            
            EventSystem.current.SetSelectedGameObject(restartButton.gameObject);
            isPaused = true;
        }
        else if (isPaused == true)
        {
            Time.timeScale = 1;
            pauseScreen.gameObject.SetActive(false);
            EventSystem.current.SetSelectedGameObject(null);
            isPaused = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (leftGameBounds != null & rightGameBounds != null)
        {
            Vector2 bottomLeft = new Vector2(leftGameBounds.bounds.max.x, leftGameBounds.bounds.min.y);
            Vector2 topLeft = leftGameBounds.bounds.max;
            Gizmos.DrawLine(bottomLeft, topLeft);

            Vector2 bottomRight = rightGameBounds.bounds.min;
            Vector2 topRight = new Vector2(rightGameBounds.bounds.min.x, rightGameBounds.bounds.max.y);
            Gizmos.DrawLine(bottomRight, topRight);
        }
    }
}
