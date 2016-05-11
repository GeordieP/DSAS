using UnityEngine;
/*
* PooledEntity
* Parent for classes that are intended to be part of a GameObjectPool
* Provides generic methods to be used when operations on object pool, such as deactivating all objects
*/
public class PooledEntity : MonoBehaviour {
    public virtual void Despawn() {
        transform.position = new Vector3(-50f, -50f, 0f);
    }
}
