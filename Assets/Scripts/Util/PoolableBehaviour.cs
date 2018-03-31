using System.Collections;
using UnityEngine;

// change name to poolable behavior
public abstract class PoolableBehaviour : MonoBehaviour
{
    protected void OnEnable()
    {
        OnPoolableEnable();
    }

    protected void OnDisable()
    {
        StopAllCoroutines();
        OnPoolableDisable();
    }

    public void SetActive(bool enable)
    {
        gameObject.SetActive(enable);
    }

    protected virtual void OnPoolableEnable() { }
    protected virtual void OnPoolableDisable() { }
}