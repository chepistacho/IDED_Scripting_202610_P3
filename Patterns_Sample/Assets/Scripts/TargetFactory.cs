using UnityEngine;

public class TargetFactory : FactoryBase<Target>
{
    private static TargetFactory targetInstance;

    [SerializeField]
    private int targetPoolSize = 5;

    private TargetFacade targetFacade;

    public static TargetFactory TargetInstance => targetInstance;

    public TargetFacade TargetFacade
    {
        get
        {
            if (targetFacade == null)
            {
                targetFacade = new TargetFacade(this, transform, targetPoolSize);
            }

            return targetFacade;
        }
    }

    public int TargetCount => spawnObjects.Length;

    protected new void Awake()
    {
        base.Awake();

        if (targetInstance == null)
        {
            targetInstance = this;
        }
        else if (targetInstance != this)
        {
            Destroy(this);
        }
    }

    public Target GetPrefabByIndex(int index)
    {
        if (spawnObjects == null || spawnObjects.Length == 0)
        {
            return null;
        }

        int safeIndex = Mathf.Clamp(index, 0, spawnObjects.Length - 1);
        return spawnObjects[safeIndex];
    }

    public override Target CreateInstance()
    {
        return Instantiate(spawnObjects[Random.Range(0, spawnObjects.Length)]);
    }
}
