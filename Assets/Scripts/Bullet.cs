using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public int damage;

    //destroy the bullet after 3 seconds
    private void Start()
    {
        Destroy(this.gameObject, 3);
    }

}
