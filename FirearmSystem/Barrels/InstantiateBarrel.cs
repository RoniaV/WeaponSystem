using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateBarrel : BarrelBase
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float shotForce;
    [SerializeField] bool addInertia;
    [Header("Object Pool Settings")]
    [SerializeField] bool doBulletPool;
    [SerializeField] int poolSize;
    [SerializeField] Transform poolParent;

    private Bullet[] bulletPool;
    private int poolIndex = 0;
    private CharacterController character;

    void Start()
    {
        if (doBulletPool)
        {
            CreatePool();
            poolParent.parent = null;
        }
    }

    protected override void DoShot()
    {

        if (doBulletPool)
        {
            ShotPoolBullet();

            SetPoolIndex();
        }
        else
        {
            Bullet bullet = Instantiate(bulletPrefab, shotPoint.position, shotPoint.rotation).GetComponent<Bullet>();

            if (addInertia)
            {
                SetCharacter();
                bullet.LaunchBullet(shotPoint.forward, shotForce, character.velocity);
            }
            else
                bullet.LaunchBullet(shotPoint.forward, shotForce);
        }
    }

    private void ShotPoolBullet()
    {
        bulletPool[poolIndex].gameObject.SetActive(true);

        bulletPool[poolIndex].transform.position = shotPoint.position;
        bulletPool[poolIndex].transform.rotation = shotPoint.rotation;

        if (addInertia)
        {
            SetCharacter();
            bulletPool[poolIndex].LaunchBullet(shotPoint.forward, shotForce, character.velocity);
        }
        else
            bulletPool[poolIndex].LaunchBullet(shotPoint.forward, shotForce);
    }

    private void CreatePool()
    {
        bulletPool = new Bullet[poolSize];

        for(int i = 0; i < poolSize; i++)
        {
            bulletPool[i] = Instantiate(bulletPrefab, poolParent.position, Quaternion.identity, poolParent).GetComponent<Bullet>();
            bulletPool[i].gameObject.SetActive(false);
        }
    }

    private void SetPoolIndex()
    {
        poolIndex++;

        if (poolIndex >= poolSize)
            poolIndex = 0;
    }

    private void SetCharacter()
    {
        if (character == null)
            character = transform.root.GetComponent<CharacterController>();
    }
}
