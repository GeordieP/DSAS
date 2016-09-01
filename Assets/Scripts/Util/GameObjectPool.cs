using UnityEngine;
using System.Collections.Generic;

public class GameObjectPool {
    private List<GameObject> _inUse, _available;
    private GameObject _initialStateItem;
    
    public List<GameObject> InUse { get { return _inUse; }}
    public List<GameObject> Available { get { return _available; }}

    public delegate void PoolFullAction();
    public event PoolFullAction onPoolFull;     // emitted when the pool is full; there are no active instances in use

    private GameObject containerObject;         // GameObject used as a parent to all pooled objects in the scene

    public GameObjectPool(int length, GameObject initialStateItem) {
        _initialStateItem = initialStateItem;

        _inUse = new List<GameObject>();
        _available = new List<GameObject>();

        containerObject = MonoBehaviour.Instantiate(new GameObject(), Vector3.zero, Quaternion.identity) as GameObject;
        containerObject.name = _initialStateItem.name + "_pool";
        

        GameObject temp;
        for (int i = 0; i < length; i++) {
            temp = SpawnOne();
            _available.Add(temp);
        }
    }

    // spawn a new pooled entity using the initial state item as a template
    // initial position and rotation don't matter as they should handled by each pooled entity's spawn and despawn methods
    // we tell the entity what pool it's a part of so it can be easily returned to the pool
    private GameObject SpawnOne() {
        GameObject entity = MonoBehaviour.Instantiate(_initialStateItem, Vector3.zero, Quaternion.identity) as GameObject;
        entity.GetComponent<PooledEntity>().SetPool(this);
        entity.transform.parent = containerObject.transform;
        return entity;
    }

    // retrieve an object from the pool
    public GameObject Borrow() {
        GameObject requestedObj;

        lock (_available) {
            if (_available.Count > 0) {
                requestedObj = _available[0];
                _inUse.Add(requestedObj);
                _available.RemoveAt(0);
            } else {
                Debug.LogWarning(System.String.Format("Exceeded {0} object pool size. Creating a new object. ", _initialStateItem.name));
                requestedObj = SpawnOne();
                _inUse.Add(requestedObj);
            }
        }
        
        return requestedObj;
    }

    public GameObject[] Borrow(int count) {
        GameObject[] requestedObjs = new GameObject[count];

        for (int i = 0; i < count; i++) {
            requestedObjs[i] = Borrow();
        }
        return requestedObjs;
    }

    // return an object to the pool
    public void Restore(GameObject retObj) {
        lock (_available) {
            _available.Add(retObj);
            _inUse.Remove(retObj);
        }

        if (onPoolFull != null && _inUse.Count == 0) {
            onPoolFull();
        }
    }

    // return an object to the pool without locking the lists
    // used when a large amount of things are returned all at once
    public void RestoreUnlocked(GameObject retObj) {
        _available.Add(retObj);
        _inUse.Remove(retObj);

        if (onPoolFull != null && _inUse.Count == 0) {
            onPoolFull();
        }
    }

    public GameObject[] GetInUse() {
        return _inUse.ToArray();
    }

    public void Clear() {
        _available = new List<GameObject>();
        _inUse = new List<GameObject>();
    }


    public int AliveCount() {
        return _inUse.Count;
    }
}
