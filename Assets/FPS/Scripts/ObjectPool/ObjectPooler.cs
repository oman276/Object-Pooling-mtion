using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This object pooler script contains all data necessary for object pooling,
 * the process of collecting a bunch of similar objects together into a pool
 * from which they can be called as necessary, without expensive 
 * Instantiate() calls.
 * 
 * This implementation also allows the caller to specify if the object should be 
 * automatically deactivated after a certain period of time.
 * 
 * Author: Owen Gallagher
 * Last Updated: 9-28-22
 * 
 */

public class ObjectPooler : MonoBehaviour
{

    //Class that contains all data needed to create a pool in poolDict
    [System.Serializable]
    public class PoolData
    {
        //Name of pool 
        public string name;
        //GameObject to spawn
        public GameObject item;
        //Size of Pool
        public int size;
        //A float indicating how long until the object deactivates on it's own.
        //NOTE: values >= 0 will indicate that it is not to be deactivated.
        public float timeToDeactivate;
    }

    //To ensure this is the only object of it's type
    public static ObjectPooler instance;
    
    //poolDict contains all pool objects, created from the data in the PoolData class
    public Dictionary<string, Queue<GameObject>> poolDict;

    //despawnDict contains all data surrounding despawn times for each object pool, created from 
    //  data in PoolData class
    public Dictionary<string, float> despawnDict;

    //List of all PoolData objects to be implemented as lists
    public List<PoolData> poolList;

    private void Awake()
    {
        if (instance == null) {
            instance = this;
        }

        poolDict = new Dictionary<string, Queue<GameObject>>();
        
        //Create all pools based on the data from poolList and add them to poolDict
        foreach (PoolData pool in poolList) {
            Queue<GameObject> objQueue = new Queue<GameObject>();
            
            for (int i = 0; i < pool.size; ++i) {
                GameObject obj = Instantiate(pool.item);
                obj.SetActive(false);
                objQueue.Enqueue(obj);
            }

            //Add objects to the poolDict AND add relevant data to despawnDict
            poolDict.Add(pool.name, objQueue);
            despawnDict.Add(pool.name, pool.timeToDeactivate);
        }
    }

    //Coroutine to wait a set amount of seconds and then deactivate an object passed to it,
    //  if the object is supposed to deactivate on a set timer
    IEnumerator DeactivationCoroutine(float t, GameObject obj) {
        yield return new WaitForSeconds(t);
        obj.SetActive(false);
    
    }

    //Instantiates an object from a specified pool, at a specified position/rotation
    public GameObject InstantiateFromPool(string name, Vector3 position, Quaternion rotation) {

        if (poolDict.ContainsKey(name))
        {
            GameObject spawnedObj = poolDict[name].Dequeue();

            spawnedObj.transform.position = position;
            spawnedObj.transform.rotation = rotation;
            spawnedObj.SetActive(true);

            poolDict[name].Enqueue(spawnedObj);

            //Only activate despawn coroutine if the user sets despawn time to > 0
            if (despawnDict[name] > 0) {
                StartCoroutine(DeactivationCoroutine(despawnDict[name], spawnedObj));
            }

            return spawnedObj;
        }
        //Called if the name returns no pool in poolDict
        else {
            Debug.LogError("ERROR: No pool with name " + name + "exists. Please check for " +
                "capitalization and spelling");
            return null;
        }
    }
}
