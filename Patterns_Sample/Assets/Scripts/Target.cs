using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Target : MonoBehaviour, IFactoryProduct
{
    private const float TIME_TO_DESTROY = 10F;

    [SerializeField]
    private int maxHP = 1;

    private int currentHP;

    [SerializeField]
    private int scoreAdd = 10;

    private ITargetPool targetPool;

    public delegate void OnTargetDestroyed(int scoreAdd);

    public static event OnTargetDestroyed onTargetDestroyed;

    private void Awake()
    {
        currentHP = maxHP;
    }

    public void SetPool(ITargetPool targetPool)
    {
        this.targetPool = targetPool;
    }

    public void ResetTarget(bool active)
    {
        CancelInvoke("ReturnToPool");
        currentHP = maxHP;
        gameObject.SetActive(active);

        if (active)
        {
            Invoke("ReturnToPool", TIME_TO_DESTROY);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        int collidedObjectLayer = collision.gameObject.layer;

        if (collidedObjectLayer.Equals(Utils.BulletLayer))
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();

            if (bullet != null)
            {
                Pool.Instance.ReturnBullet(bullet);
            }

            currentHP -= 1;

            if (currentHP <= 0)
            {
                onTargetDestroyed?.Invoke(scoreAdd);
                ReturnToPool();
            }
        }
        else if (collidedObjectLayer.Equals(Utils.PlayerLayer) ||
            collidedObjectLayer.Equals(Utils.KillVolumeLayer))
        {
            Player.Instance.OnPlayerHit?.Invoke();
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        if (targetPool != null)
        {
            targetPool.ReturnTarget(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
