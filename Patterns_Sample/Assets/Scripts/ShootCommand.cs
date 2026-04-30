using System.Collections;
using UnityEngine;

public class ShootCommand : MonoBehaviour, ICommand
{
    #region Bullet

    [Header("Bullet")]
    [SerializeField]
    private Rigidbody bullet;

    [SerializeField]
    private float bulletSpeed = 3F;

    [Header("Triple Shot Decorator")]
    [SerializeField]
    private float tripleShotDuration = 5F;

    [SerializeField]
    private float delayBetweenTripleBullets = 0.15F;

    #endregion Bullet

    private IShotDecorator shotDecorator;
    private Coroutine tripleShotTimer;

    private Transform BulletSpawnPoint => Player.Instance.BulletSpawnPoint;

    private bool CanShoot => BulletSpawnPoint != null && bullet != null && Pool.Instance != null;

    private void Start()
    {
        shotDecorator = new NormalShotDecorator();
    }

    public void Execute()
    {
        if (CanShoot)
        {
            shotDecorator.Shoot(this);
        }
    }

    public void ShootOneBullet()
    {
        Bullet bullet = Pool.Instance.GetBullet();
        bullet.transform.position = BulletSpawnPoint.position;
        bullet.transform.rotation = BulletSpawnPoint.rotation;
        bullet.Rigidbody.AddForce(transform.up * bulletSpeed, ForceMode.Impulse);
    }

    public IEnumerator ShootThreeBullets()
    {
        for (int i = 0; i < 3; i++)
        {
            ShootOneBullet();
            yield return new WaitForSeconds(delayBetweenTripleBullets);
        }
    }

    public void ActivateTripleShotDecorator()
    {
        shotDecorator = new TripleShotDecorator();

        if (tripleShotTimer != null)
        {
            StopCoroutine(tripleShotTimer);
        }

        tripleShotTimer = StartCoroutine(ReturnToNormalShot());
    }

    private IEnumerator ReturnToNormalShot()
    {
        yield return new WaitForSeconds(tripleShotDuration);
        shotDecorator = new NormalShotDecorator();
        tripleShotTimer = null;
    }
}

