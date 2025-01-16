using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Header("Pooling Settings")]
    public GameObject prefab; // Prefab to be pooled
    public int poolSize = 10; // Initial pool size
    public bool expandPool = true; // Allow pool to grow if needed

    private List<GameObject> pool;

    private void Awake()
    {
        InitializePool();
    }

    // Initializes the pool
    private void InitializePool()
    {
        pool = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            CreateNewObject();
        }
    }

    // Creates a new object, adds it to the pool, and returns it
    private GameObject CreateNewObject()
    {
        GameObject newObj = Instantiate(prefab, transform);
        newObj.SetActive(false); // Initially deactivate the object
        pool.Add(newObj);
        return newObj;
    }

    // Gets an available object from the pool
    public GameObject GetObject(Vector3 position, Quaternion rotation)
    {
        foreach (var obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                // Activate and reset the object's state
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                obj.SetActive(true);

                BaseUnit unit = obj.GetComponent<BaseUnit>();
                if (unit != null)
                {
                    unit.ResetUnit(); // Reset the unit state
                }

                return obj;
            }
        }

        // If no inactive object is found, optionally expand the pool
        if (expandPool)
        {
            GameObject newObj = CreateNewObject();
            newObj.transform.position = position;
            newObj.transform.rotation = rotation;
            newObj.SetActive(true);

            BaseUnit unit = newObj.GetComponent<BaseUnit>();
            if (unit != null)
            {
                unit.ResetUnit(); // Reset the unit state
            }

            return newObj;
        }

        Debug.LogWarning("No available objects in pool and pool expansion is disabled.");
        return null;
    }


    // Returns an object back to the pool
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
    }
}
