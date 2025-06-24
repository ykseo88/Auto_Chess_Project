using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public class MaxSetQueue
    {
        public int maxNum;
        public Queue<GameObject> objQueue;

        public MaxSetQueue(int maxNum, Queue<GameObject> objQueue)
        {
            this.maxNum = maxNum;
            this.objQueue = objQueue;
        }
    }
    
    public static PoolManager Instance;
    [Header("Pool Dictionarys")]
    public Dictionary<ParticleSystem, Queue<ParticleSystem>> PoolParticleSystemDictionary = new Dictionary<ParticleSystem, Queue<ParticleSystem>>();
    public Dictionary<GameObject, Queue<GameObject>> PoolGameObjectDictionary = new Dictionary<GameObject, Queue<GameObject>>();
    public Dictionary<GameObject, MaxSetQueue> PoolGameObjectMaxSetDictionary = new Dictionary<GameObject, MaxSetQueue>();
    

    private void Awake()
    {
        Instance = this;
    }


    public void PoolGameObjectQueuePlus(GameObject key, float defaultNum)
    {
        if (PoolGameObjectDictionary.ContainsKey(key))
        {
            return;
        }
        PoolGameObjectDictionary.Add(key, new Queue<GameObject>());

        for (int i = 0; i < defaultNum; i++)
        {
            GameObject temp = Instantiate(key);
            temp.SetActive(false);
            PoolGameObjectDictionary[key].Enqueue(temp);
        }
    }
    
    public void PoolGameObjectMaxSetQueuePlus(GameObject key, int maxNum)
    {
        if (PoolGameObjectMaxSetDictionary.ContainsKey(key))
        {
            return;
        }
        PoolGameObjectMaxSetDictionary.Add(key, new MaxSetQueue(maxNum, new Queue<GameObject>()));
        Debug.Log("맥스셋 큐 추가: " + key.name);
    }
    
    public void PoolParticleSystemQueuePlus(ParticleSystem key, float defaultNum)
    {
        if (PoolParticleSystemDictionary.ContainsKey(key))
        {
            return;
        }
        PoolParticleSystemDictionary.Add(key, new Queue<ParticleSystem>());

        for (int i = 0; i < defaultNum; i++)
        {
            ParticleSystem temp = Instantiate(key);
            temp.gameObject.SetActive(false);
            PoolParticleSystemDictionary[key].Enqueue(temp);
        }
    }
    
    public GameObject MaxSetGameObjectPoolActive(GameObject key, Vector3 position, Quaternion rotation)
    {
        if (PoolGameObjectMaxSetDictionary[key].objQueue.Count >= PoolGameObjectMaxSetDictionary[key].maxNum)
        {
            GameObject temp = PoolGameObjectMaxSetDictionary[key].objQueue.Dequeue();
            if (temp.transform.TryGetComponent(out Rigidbody rb))
            {
                rb.velocity = Vector3.zero;
            }
            temp.transform.position = position;
            temp.transform.rotation = rotation;
            PoolGameObjectMaxSetDictionary[key].objQueue.Enqueue(temp);
            return temp;
        }
        GameObject newItem = Instantiate(key, position, rotation);
        PoolGameObjectMaxSetDictionary[key].objQueue.Enqueue(newItem);
        return newItem;
    }
    
    public GameObject GameObjectPoolActive(GameObject key, Vector3 position, Quaternion rotation)
    {
        for (int i = 0; i < PoolGameObjectDictionary[key].Count; i++)
        {
            GameObject temp = PoolGameObjectDictionary[key].Dequeue();
            if (temp.activeSelf == false)
            {
                if (temp.transform.TryGetComponent(out Rigidbody rb))
                {
                    rb.velocity = Vector3.zero;
                }
                temp.transform.position = position;
                temp.transform.rotation = rotation;
                temp.gameObject.SetActive(true);
                PoolGameObjectDictionary[key].Enqueue(temp);
                return temp;
            }
            PoolGameObjectDictionary[key].Enqueue(temp);
        }
        GameObject newItem = Instantiate(key, position, rotation);
        PoolGameObjectDictionary[key].Enqueue(newItem);
        return newItem;
    }
    
    public GameObject GameObjectPoolActive(GameObject key)
    {
        for (int i = 0; i < PoolGameObjectDictionary[key].Count; i++)
        {
            GameObject temp = PoolGameObjectDictionary[key].Dequeue();
            if (temp.activeSelf == false)
            {
                temp.gameObject.SetActive(true);
                if (temp.transform.TryGetComponent(out Rigidbody rb))
                {
                    rb.velocity = Vector3.zero;
                }
                PoolGameObjectDictionary[key].Enqueue(temp);
                return temp;
            }
            PoolGameObjectDictionary[key].Enqueue(temp);
        }
        GameObject newItem = Instantiate(key);
        PoolGameObjectDictionary[key].Enqueue(newItem);
        return newItem;
    }
    
    public ParticleSystem ParticleSystemPoolActive(ParticleSystem key, Vector3 position, Quaternion rotation)
    {
        for (int i = 0; i < PoolParticleSystemDictionary[key].Count; i++)
        {
            ParticleSystem temp = PoolParticleSystemDictionary[key].Dequeue();
            if (temp.gameObject.activeSelf == false)
            {
                temp.gameObject.SetActive(true);
                temp.transform.position = position;
                temp.transform.rotation = rotation;
                PoolParticleSystemDictionary[key].Enqueue(temp);
                StartCoroutine(ParticleLast(temp, temp.main.duration));
                return temp;
            }
            PoolParticleSystemDictionary[key].Enqueue(temp);
        }
        Debug.Log($"{key}의 큐 길이: {PoolParticleSystemDictionary[key].Count}");
        ParticleSystem newItem = Instantiate(key, position, rotation);
        StartCoroutine(ParticleLast(newItem, newItem.main.duration));
        PoolParticleSystemDictionary[key].Enqueue(newItem);
        return newItem;
    }
    
    public ParticleSystem ParticleSystemPoolActive(ParticleSystem key, Vector3 position, Quaternion rotation, float duration)
    {
        for (int i = 0; i < PoolParticleSystemDictionary[key].Count; i++)
        {
            ParticleSystem temp = PoolParticleSystemDictionary[key].Dequeue();
            if (temp.gameObject.activeSelf == false)
            {
                temp.gameObject.SetActive(true);
                temp.transform.position = position;
                temp.transform.rotation = rotation;
                PoolParticleSystemDictionary[key].Enqueue(temp);
                StartCoroutine(ParticleLast(temp, duration));
                return temp;
            }
            PoolParticleSystemDictionary[key].Enqueue(temp);
        }
        Debug.Log($"{key}의 큐 길이: {PoolParticleSystemDictionary[key].Count}");
        ParticleSystem newItem = Instantiate(key, position, rotation);
        StartCoroutine(ParticleLast(newItem, duration));
        PoolParticleSystemDictionary[key].Enqueue(newItem);
        return newItem;
    }

    private IEnumerator ParticleLast( ParticleSystem temp, float duration)
    {
        temp.Play();
        yield return new WaitForSeconds(duration);
        temp.gameObject.SetActive(false);
    }
}
