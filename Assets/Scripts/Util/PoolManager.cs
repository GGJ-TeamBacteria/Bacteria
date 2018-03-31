using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager> {

    [SerializeField]
    private int _InitialPoolSize = 128;

    private Dictionary<int, List<PoolableBehaviour>> objectPool = new Dictionary<int, List<PoolableBehaviour>>();

    // Points the last index we changed 
    private Dictionary<int, int> indexPool = new Dictionary<int, int>();

    public override void SingletonAwake() { }

    public void CreatePool(PoolableBehaviour prefab, int poolSize)
    {
        // different for each instance
        int ID = prefab.GetInstanceID();
        objectPool[ID] = new List<PoolableBehaviour>();
        indexPool[ID] = 0;

        for (int i = 0; i < poolSize; ++i)
        {
            PoolableBehaviour obj = Instantiate(prefab);
            obj.gameObject.SetActive(false);
            objectPool[ID].Add(obj);
        }

    }

    public PoolableBehaviour GetObjectFromPool(PoolableBehaviour prefab)
    {
        int ID = prefab.GetInstanceID();

        // If there is no key in dic
        if (!objectPool.ContainsKey(ID))
        {
            CreatePool(prefab, _InitialPoolSize);
        }

        List<PoolableBehaviour> objectPoolList = objectPool[ID];
        
        for (int i = 0; i < objectPoolList.Count; ++i)
        {
            IncreaseIndexPool(ID);

            // check if it is active or not
            if (!objectPoolList[indexPool[ID]].gameObject.activeInHierarchy)
            {
                PoolableBehaviour obj = objectPoolList[indexPool[ID]];
                obj.gameObject.SetActive(true);
                return obj;
            }
        }

        PoolableBehaviour tmp = AddObjectToPool(ID ,prefab);
        tmp.gameObject.SetActive(true);
        return tmp;
    }

    private PoolableBehaviour AddObjectToPool(int ID, PoolableBehaviour prefab)
    {
        PoolableBehaviour obj = Instantiate(prefab);
        obj.gameObject.SetActive(false);
        objectPool[ID].Add(obj);

        return obj;

    }

    private void IncreaseIndexPool(int ID)
    {
        ++indexPool[ID];
        if (indexPool[ID] >= objectPool[ID].Count)
        {
            indexPool[ID] = 0;
        }

    }
}
