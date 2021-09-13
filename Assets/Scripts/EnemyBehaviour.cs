using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    private Animator anim;
    private Rigidbody rb;
    private AudioSource deadSE;
    private float enemySpeed;
    private int hp;
    private float distance = 0;

    private bool dead = false;
    private bool stop = false;
    private bool near = false;

    public Transform playerPos;

    public GameController gameController;


    private void Awake()
    {
        TextAsset file = Resources.Load("PlayerAndEnemyData") as TextAsset;
        string json = file.ToString();
        EnemyData loadedEnemyData = JsonUtility.FromJson<EnemyData>(json);
        enemySpeed = loadedEnemyData.forwardMaxSpeedPlayer + loadedEnemyData.speedAddToEnemy;
        hp = loadedEnemyData.enemyHealth;
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        deadSE = GetComponent<AudioSource>();
    }

    void Update()
    {
        //if dead do not move and ignore everything
        if (dead)
            return;
        
        //check if the enemy is close to the player
        distance = playerPos.position.z - transform.position.z;
        if (distance <= 4)
        {
            near = true;
        }
        else if (distance > 4 && near)
        {
            near = false;
        }

        if (hp <= 0 && !dead)
        {
            anim.SetTrigger("IsDead");
            dead = true;
            deadSE.Play(0);
            gameController.EnemyDead();
            Destroy(this.GetComponentInChildren<CapsuleCollider>(), 1);
        }

    }

    private void FixedUpdate()
    {
        //if dead or the game is ended do not move and ignore everything
        if (dead)
            return;
        if (stop)
            return;

        //move forward at enemySpeed
        Vector3 forwardMove = transform.forward * enemySpeed * Time.fixedDeltaTime;

        Vector3 horizontalMove;
        //if the enemy is close to the player
        //then it goes after him on the same X axis
        if (near)
        {
            float horPos = playerPos.position.x - transform.position.x;
            horizontalMove = (new Vector3(horPos, 0f, 0f)) * 1 * Time.fixedDeltaTime;
        }
        //if the enemy is far from the player
        //then it goes straight on
        else
        {
            horizontalMove = Vector3.zero;
        }
        rb.MovePosition(rb.position + forwardMove + horizontalMove);

    }

    //deals damage to the enemy
    public void Damage(int damage)
    {
        hp -= damage;
    }

    //tell the GameManager to end the game
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            gameController.StopGame(false);
        }
    }

    //check if the enemy is hit by the bullet
    //if so, suffers damage
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            Bullet bulletInfo = other.gameObject.GetComponent<Bullet>();
            int damage = bulletInfo.damage;
            Damage(damage);
            Destroy(other.gameObject);
        }
    }

    //if the game is ended stops the enemy
    public void GameEnded()
    {
        stop = true;
        anim.SetTrigger("GameStops");
    }

    private class EnemyData
    {
        public float forwardMaxSpeedPlayer;
        public float speedAddToEnemy;
        public int enemyHealth;
    }

}
