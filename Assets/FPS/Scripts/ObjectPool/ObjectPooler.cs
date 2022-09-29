using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{

    [System.Serializable]
    public class Pool
    {
        public string name;
        public GameObject prefab;
        public int size;
        public float timeToDeactivate;
    }

    public static ObjectPooler instance;
    public Dictionary<string, Queue<GameObject>> poolDict;
    public List<Pool> poolList;


    private void Awake()
    {
        if (instance == null) {
            instance = this;
        }

        poolDict = new Dictionary<string, Queue<GameObject>>();
        foreach (Pool pool in poolList) {
            Queue<GameObject> objQueue = new Queue<GameObject>();
            
            for (int i = 0; i < pool.size; ++i) {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objQueue.Enqueue(obj);
            }

            poolDict.Add(pool.name, objQueue);
        }

    }



    public void InstantiateFromPool(string name) { 
        
    }

}
