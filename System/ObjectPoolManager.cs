using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager instance;
    Dictionary<string, Stack<GameObject>> objectPool;
    List<GameObject> objectList;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        objectPool = new Dictionary<string, Stack<GameObject>>();
        objectList = new List<GameObject>();
    }

    private GameObject Load(string path)
    {
        GameObject temp = Resources.Load<GameObject>(path);
        return temp;
    }

    public GameObject Instantiate(string path)
    {
        Stack<GameObject> stack = null;
        GameObject obj = null;
        if (!objectPool.TryGetValue(path, out stack))
        {
            GameObject prefab = null;
            prefab = Load(path);
            obj = Instantiate(prefab);
            obj.name = path;
            stack = new Stack<GameObject>();
            objectPool.Add(path, stack);
            objectList.Add(obj);
        }
        else
        {
            if (stack.Count > 0)
            {
                obj = stack.Pop();
                obj.transform.SetParent(null);
                obj.SetActive(true);
            }
            else
            {
                GameObject prefab = Load(path);
                obj = Instantiate(prefab);
                obj.name = path;
            }
        }

        return obj;
    }

    public void Return(GameObject target)
    {
        Return(target, transform.position);
    }
    public void Return(GameObject target, Vector3 position)
    {
        foreach (GameObject obj in objectList)
        {
            if (obj == target)
            {
                objectPool[obj.name].Push(target);
                target.transform.position = position;
                target.transform.SetParent(this.gameObject.transform);
                target.SetActive(false);

            }
        }
    }

}
