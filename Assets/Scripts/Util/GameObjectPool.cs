using UnityEngine;
using System.Collections.Generic;

public class GameObjectPool {
    private List<GameObject> _inUse, _available;
    private GameObject _initialStateItem;

    public GameObjectPool(int length, GameObject initialStateItem) {
        _initialStateItem = initialStateItem;

        _inUse = new List<GameObject>();
        _available = new List<GameObject>();
        
        for (int i = 0; i < length; i++) {
            _available.Add(MonoBehaviour.Instantiate(initialStateItem, Vector3.zero, Quaternion.identity) as GameObject);
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

    // return an object to the pool
    public void Restore(GameObject retObj) {
        lock (_available) {
            _available.Add(retObj);
            _inUse.Remove(retObj);
        }
    }
}
