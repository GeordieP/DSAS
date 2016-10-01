using UnityEngine;
/*
* PooledEntity
* Parent for classes that are intended to be part of a GameObjectPool
* Provides generic methods to be used when operations on object pool, such as deactivating all objects
*/
public abstract class PooledEntity : MonoBehaviour {
    private GameObjectPool myPool;

    public virtual void SetPool(GameObjectPool pool) {
        myPool = pool;
    }

    public virtual void ReturnToPool() {
        myPool.RestoreUnlocked(gameObject);
    }

    public virtual void Despawn() {
        ReturnToPool();
        transform.position = new Vector3(-50f, -50f, 0f);
        gameObject.SetActive(false);
    }
}
