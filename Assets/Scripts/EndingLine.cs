using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingLine : MonoBehaviour
{
    public ParticleSystem[] particle;
    public GameController gameController;


    //if the player enter the ending line tells to the game manager
    //to end the game and plays some particles effects
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            foreach(ParticleSystem particleToPlay in particle)
            {
                particleToPlay.Play();
                gameController.StopGame(true);
            }
        }
    }

}
