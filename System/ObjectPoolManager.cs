using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager instance;
    Dictionary<string, Stack<GameObject>> objectPool;        //관리되는 오브젝트가 등록되어 있는 Pool
    List<GameObject> objectList;                            //관리되는 오브젝트의 종류

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

    //새로운 오브젝트 로드
    private GameObject Load(string path)
    {
        GameObject temp = Resources.Load<GameObject>(path);
        return temp;
    }

    //오브젝트 풀의 게임오브젝트를 인스턴스화
    public GameObject Instantiate(string path)
    {
        Stack<GameObject> stack = null;
        GameObject obj = null;
        if (!objectPool.TryGetValue(path, out stack))        //Pool에 원하는 오브젝트가 존재하지 않음
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
            if (stack.Count > 0)                             //Pool에 원하는 오브젝트가 존재
            {
                obj = stack.Pop();
                obj.transform.SetParent(null);
                obj.SetActive(true);
            }
            else                                            //오브젝트 Pool에 등록은 되어 있지만 재사용할수있는 오브젝트가 없음
            {
                GameObject prefab = Load(path);
                obj = Instantiate(prefab);
                obj.name = path;
            }
        }

        return obj;
    }

    //오브젝트 반환
    public void Return(GameObject target)
    {
        Return(target, transform.position);
    }
    
    public void Return(GameObject target, Vector3 position)
    {
        foreach (GameObject obj in objectList)        //관리되는 오브젝트 목록에 반환할 오브젝트가 있다면
        {
            if (obj == target)                        //오브젝트 반환
            {
                objectPool[obj.name].Push(target);
                target.transform.position = position;
                target.transform.SetParent(this.gameObject.transform);
                target.SetActive(false);

            }
        }
    }

}
