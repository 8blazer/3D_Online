using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{

    private float timer;
    private int seconds;
    private int minutes;

    [SyncVar(hook = nameof(HandlePointUpdateCanvas))]
    public int points;

    private bool gameOver = false;
    [SerializeField] private int gameTime;// in seconds
    [SerializeField] private Text timeText;
    [SerializeField] private Text pointText;
    [SerializeField] private Canvas EndCanvas;

    [ServerCallback]
    private void Start()
    {
        float check = gameTime / 60;
        minutes = Mathf.FloorToInt(check);
        if(minutes == 0)
        {
            seconds = gameTime;
        }
        else
        {
            seconds = gameTime - (Mathf.FloorToInt(check) * 60);
        }
        RpcUpdateCanvas();
    }
    [ServerCallback]
    void FixedUpdate()
    {
        #region Timestuff
        timer += Time.deltaTime;
        if (timer > 1 && seconds >= 1)
        {
            seconds--;
            timer = 0;
        }
        else if(timer > 1 && minutes > 0)
        {
            minutes--;
            seconds = 59;
            timer = 0;
        }
        else if(timer > 1)
        {
            timer = 0;
            //seconds--;
            gameOver = true;
            SvrEndGame();
        }
        RpcUpdateCanvas();
        #endregion
    }

    public void HandlePointUpdateCanvas(int oldPoints, int newPoints)
    {
        pointText.text = ("Points: " + newPoints);
    }
    #region MoreTime stuff
    [ClientRpc]
    private void RpcUpdateCanvas()
    {
        if(seconds >= 10)
        {
            timeText.text = (minutes + " : " + seconds);
        }
        else
        {
            timeText.text = (minutes + " : 0" + seconds);
        }
    }
    [Server]
    private void SvrEndGame()
    {
        RpcEndGame();
       Time.timeScale = 0;
 
    }
    [ClientRpc]
    private void RpcEndGame()
    {
        Transform text = EndCanvas.transform.GetChild(1);
        Camera.main.gameObject.GetComponent<AudioSource>().Play();
        text.gameObject.GetComponent<Text>().text = ("Congratulations! \n You earned " + points + " points!");
        EndCanvas.enabled = true;
        Time.timeScale = 0;
    }
    #endregion
    
}
