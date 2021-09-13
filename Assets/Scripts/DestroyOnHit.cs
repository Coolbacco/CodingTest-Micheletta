using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnHit : MonoBehaviour
{
    public GameObject wallDestroyed;

    private void Start()
    {
        wallDestroyed.SetActive(false);
    }

    //if the player or an enemy hits the wall, destroy the wall
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            DestroyWall();
        }
        else if(collision.gameObject.tag == "Enemy")
        {
            DestroyWall();
        }
    }

    private void DestroyWall()
    {
        this.gameObject.SetActive(false);
        wallDestroyed.SetActive(true);
    }

}
