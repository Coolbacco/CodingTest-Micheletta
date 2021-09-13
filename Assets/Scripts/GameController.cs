using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject player;
    public GameObject gun;
    public Transform enemiesParent;
    public ParticleSystem[] confettiUI;
    public AudioClip start;
    public AudioClip win;
    public AudioClip lost;

    public Text endMessage;

    private int activeChildren;
    private int enemiesToKill;
    private bool isWin = false;

    private AudioSource soundEffects;

    private void Awake()
    {
        TextAsset file = Resources.Load("GameManager") as TextAsset;
        string json = file.ToString();
        GameManagerData loadedGameManagerData = JsonUtility.FromJson<GameManagerData>(json);
        activeChildren = loadedGameManagerData.activeEnemies;
    }

    //gets the player.transform reference
    //sets the value of the enemies to kill for the winning condition
    //activates the enemies and pass the player Transform reference
    void Start()
    {
        soundEffects = GetComponent<AudioSource>();
        soundEffects.clip = start;
        
        Transform playerPos = player.transform;

        int children = enemiesParent.childCount;
        if(activeChildren > children)
        {
            activeChildren = children;
        }
        
        enemiesToKill = activeChildren;
        
        for (int i=0; i < activeChildren; i++)
        {
            GameObject enemy = enemiesParent.GetChild(i).gameObject;
            enemy.SetActive(true);
            enemy.GetComponent<EnemyBehaviour>().playerPos = player.transform;
        }
    }

    //check if the player kills all the enemies
    void Update()
    {
        if(enemiesToKill <= 0 && !isWin)
        {
            isWin = true;
            StopGame(isWin);
        }
    }

    //reset the scene if the reset button is pressed
    public void SceneReset()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    //ends the game, stops player and enemies and deactivate the gun
    //shows a message
    public void StopGame(bool won)
    {
        if (won)
        {
            soundEffects.clip = win;
            soundEffects.Play(0);
        }
        else
        {
            soundEffects.clip = lost;
            soundEffects.Play(0);
        }
 
        player.GetComponent<PlayerMovement>().GameEndedPlayer(won);
        gun.SetActive(false);

        if (won)
        {
            foreach (ParticleSystem particleToPlay in confettiUI)
            {
                particleToPlay.Play();
            }
        }

        for (int i = 0; i < activeChildren; i++)
        {
            GameObject enemy = enemiesParent.GetChild(i).gameObject;
            enemy.GetComponent<EnemyBehaviour>().GameEnded();
        }

        if (won)
        {
            endMessage.text = "You win!";
        }
        else
        {
            endMessage.text = "You lost!";
        }
        
        endMessage.gameObject.SetActive(true);
    }

    //if an enemy dies, it reduces the number of enemies to be killed by 1
    public void EnemyDead()
    {
        enemiesToKill--;
    }

    private class GameManagerData
    {
        public int activeEnemies;
    }
}
