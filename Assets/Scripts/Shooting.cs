using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    private Rigidbody rbBullet;
    private float rateOfFire;
    private float bulletSpeed;
    private int bulletDamage;
    private AudioSource gunSE;

    private bool canShoot;

    private ParticleSystem gunFlash;

    private void Awake()
    {
        TextAsset file = Resources.Load("Gun") as TextAsset;
        string json = file.ToString();
        BulletData loadedBulletData = JsonUtility.FromJson<BulletData>(json);
        bulletSpeed = loadedBulletData.bulletSpeed;
        rateOfFire = loadedBulletData.rateOfFireInSeconds;
        bulletDamage = loadedBulletData.bulletDamage;
    }

    private void Start()
    {
        gunFlash = GetComponentInChildren<ParticleSystem>();
        gunSE = GetComponent<AudioSource>();
        canShoot = true;
    }

    //every "rateOfFire" seconds shoot
    void Update()
    {

        if (canShoot)
        {
            StartCoroutine(ShootTimer());
        }
    }

    IEnumerator ShootTimer()
    {
        canShoot = false;
        yield return new WaitForSeconds(rateOfFire);
        Shoot();
        canShoot = true;
    }

    //instantiate a bullet at the gun position
    //set the damage to the bullet gameObject
    //add a force to the bullet
    private void Shoot()
    {
        
        GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
        rbBullet = bullet.GetComponent<Rigidbody>();
        bullet.SetActive(true);
        bullet.GetComponent<Bullet>().damage = bulletDamage;

        gunSE.Play(0);

        gunFlash.Play();

        rbBullet.AddRelativeForce(Vector3.forward * bulletSpeed);
    }

    private class BulletData
    {
        public float bulletSpeed;
        public float rateOfFireInSeconds;
        public int bulletDamage;
    }
}
