using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{

    [SerializeField] protected GameObject pooled_object;

    List<GameObject> pooledObjects = new List<GameObject>();

    public virtual GameObject GetObj(Vector3 pos, Transform parent, bool enableOnPooled)
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            // stops calling dead stuff
            if(pooledObjects[i] == null)
            {
                continue;
            }
            if (!pooledObjects[i].activeInHierarchy)
            {
                pooledObjects[i].transform.position = pos;
                pooledObjects[i].transform.SetParent(parent);
                pooledObjects[i].SetActive(enableOnPooled);
                return pooledObjects[i];
            }
        }
        return CreateObj(pos, parent, enableOnPooled);
    }

    public virtual GameObject CreateObj(Vector3 pos, Transform parent, bool enableOnPooled)
    {
        GameObject obj = Instantiate(pooled_object);
        obj.transform.position = pos;
        obj.transform.SetParent(parent);
        pooledObjects.Add(obj);
        obj.SetActive(enableOnPooled);
        return obj;
    }

    public virtual GameObject GetObj(GameObject prefabToGet, Vector3 pos, Transform parent, bool enableOnPooled)
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (pooledObjects[i].name.Contains(prefabToGet.name) && !pooledObjects[i].activeInHierarchy)
            {
                pooledObjects[i].transform.position = pos;
                pooledObjects[i].transform.SetParent(parent);
                pooledObjects[i].SetActive(enableOnPooled);
                return pooledObjects[i];
            }
        }
        return CreateObj(prefabToGet, pos, parent, enableOnPooled);
    }

    public virtual GameObject CreateObj(GameObject prefabToGet, Vector3 pos, Transform parent, bool enableOnPooled)
    {
        GameObject obj = Instantiate(prefabToGet);
        obj.transform.position = pos;
        obj.transform.SetParent(parent);
        pooledObjects.Add(obj);
        obj.SetActive(enableOnPooled);
        return obj;
    }
}
