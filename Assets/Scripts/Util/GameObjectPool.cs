using UnityEngine;
using System.Collections.Generic;

public class GameObjectPool {
    private List<GameObject> _inUse, _available;
    private GameObject _initialStateItem;
    
    public List<GameObject> InUse { get { return _inUse; }}
    public List<GameObject> Available { get { return _available; }}

    public delegate void PoolFullAction();
    public event PoolFullAction onPoolFull;     // emitted when the pool is full; there are no active instances in use

    public GameObjectPool(int length, GameObject initialStateItem) {
        _initialStateItem = initialStateItem;

        _inUse = new List<GameObject>();
        _available = new List<GameObject>();
        GameObject parent, temp;

        parent = MonoBehaviour.Instantiate(new GameObject(), Vector3.zero, Quaternion.identity) as GameObject;
        parent.name = _initialStateItem.name + "_pool";
        
        for (int i = 0; i < length; i++) {
            temp = MonoBehaviour.Instantiate(initialStateItem, Vector3.zero, Quaternion.identity) as GameObject;
            temp.transform.parent = parent.transform;
            _available.Add(temp);
        }
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
                requestedObj = MonoBehaviour.Instantiate(_initialStateItem, Vector3.zero, Quaternion.identity) as GameObject;
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

    // public void RestoreAll() {
    //     _available.AddRange(_inUse);
    //     for (int i = 0; i < _inUse.Count; i++) {
    //         _inUse[i].GetComponent<PooledEntity>().Despawn();
    //     }
    //     _inUse = new List<GameObject>();
    // }

    public void Clear() {
        _available = new List<GameObject>();
        _inUse = new List<GameObject>();
    }


    public int AliveCount() {
        return _inUse.Count;
    }


/*
    private GameObject[] restoreQueue;
    private delegate void restoreCompleteEvent(int restoreQueueIndex);
    private event restoreCompleteEvent onRestoreComplete;

    public void RestoreAll() {
        restoreQueue = new GameObject[_inUse.Count];
        _inUse.CopyTo(restoreQueue);

        // add event to thing += etc whatever
        onRestoreComplete += SpecialRestoreComplete;
        SpecialRestore(0);
    }

    public void SpecialRestore(int restoreQueueIndex) {
        lock (_available) {
            _available.Add(restoreQueue[restoreQueueIndex]);
            _inUse.Remove(restoreQueue[restoreQueueIndex]);
        }
        onRestoreComplete(restoreQueueIndex);
        // call event complete(restoreQueueIndex)
        // SpecialRestoreComplete(restoreQueueIndex);

    }

    public void SpecialRestoreComplete(int restoreQueueIndex) {
        if (restoreQueueIndex < restoreQueue.Length)
            SpecialRestore(restoreQueueIndex + 1);
        // else
        // reset restorequeue?


    }





/*



    private delegate void restoreFinishCallback(int nextIndex);
    private GameObject[] restoreQueue;


    // starts the chain of restore -> callback -> restore
    private void RestoreAll() {
        int iterations = _inUse.Count;
        restoreQueue = new GameObject[iterations];
        _inUse.CopyTo(restoreQueue);
        restoreFinishCallback = RestoreComplete;
        int currentindex = 0;

    }

    private void RestoreComplete(int nextIndex) {

    }

    private void SpecialRestore(GameObject retObj, int nextIndex, )










*/
}
