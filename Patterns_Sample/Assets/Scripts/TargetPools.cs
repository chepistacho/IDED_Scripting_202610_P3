using System.Collections.Generic;
using UnityEngine;

public enum TargetType
{
    Basic = 0,
    Fast = 1,
    Strong = 2
}

public interface ITargetPool
{
    void ReturnTarget(Target target);
}

public abstract class AbstractTargetPool<T> : ITargetPool where T : Target
{
    private readonly List<T> targets = new List<T>();

    protected T prefab;
    protected Transform parent;

    public AbstractTargetPool(T prefab, Transform parent, int initialSize)
    {
        this.prefab = prefab;
        this.parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            AddTargetToPool();
        }
    }

    public T GetTarget()
    {
        if (prefab == null)
        {
            return null;
        }

        if (targets.Count == 0)
        {
            AddTargetToPool();
        }

        T target = targets[0];
        targets.RemoveAt(0);
        target.transform.SetParent(null);
        target.ResetTarget(true);
        return target;
    }

    public void ReturnTarget(Target target)
    {
        T typedTarget = target as T;

        if (typedTarget == null || targets.Contains(typedTarget))
        {
            return;
        }

        typedTarget.transform.SetParent(parent);
        typedTarget.transform.localPosition = Vector3.zero;
        typedTarget.ResetTarget(false);
        targets.Add(typedTarget);
    }

    private void AddTargetToPool()
    {
        if (prefab == null)
        {
            return;
        }

        T target = Object.Instantiate(prefab);
        target.SetPool(this);
        ReturnTarget(target);
    }
}

public class BasicTargetPool : AbstractTargetPool<Target>
{
    public BasicTargetPool(Target prefab, Transform parent, int initialSize)
        : base(prefab, parent, initialSize)
    {
    }
}

public class FastTargetPool : AbstractTargetPool<Target>
{
    public FastTargetPool(Target prefab, Transform parent, int initialSize)
        : base(prefab, parent, initialSize)
    {
    }
}

public class StrongTargetPool : AbstractTargetPool<Target>
{
    public StrongTargetPool(Target prefab, Transform parent, int initialSize)
        : base(prefab, parent, initialSize)
    {
    }
}

public class TargetFacade
{
    private TargetFactory targetFactory;

    private BasicTargetPool basicPool;
    private FastTargetPool fastPool;
    private StrongTargetPool strongPool;

    public TargetFacade(TargetFactory targetFactory, Transform parent, int poolSize)
    {
        this.targetFactory = targetFactory;

        basicPool = new BasicTargetPool(targetFactory.GetPrefabByIndex(0), parent, poolSize);
        fastPool = new FastTargetPool(targetFactory.GetPrefabByIndex(1), parent, poolSize);
        strongPool = new StrongTargetPool(targetFactory.GetPrefabByIndex(2), parent, poolSize);
    }

    public Target GetTarget(TargetType targetType)
    {
        switch (targetType)
        {
            case TargetType.Fast:
                return fastPool.GetTarget();

            case TargetType.Strong:
                return strongPool.GetTarget();

            default:
                return basicPool.GetTarget();
        }
    }

    public Target GetRandomTarget()
    {
        int targetCount = Mathf.Clamp(targetFactory.TargetCount, 1, 3);
        TargetType randomType = (TargetType)Random.Range(0, targetCount);
        return GetTarget(randomType);
    }
}
