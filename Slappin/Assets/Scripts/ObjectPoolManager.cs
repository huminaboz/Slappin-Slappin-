using System.Collections.Generic;
using UnityEngine;

public interface IObjectPool<T> where T : MonoBehaviour
{
    public void SetupObjectFirstTime();
    public void InitializeObjectFromPool();
    public void ReturnObjectToPool();
}

public static class ObjectPoolManager<T> where T : MonoBehaviour, IObjectPool<T>
{
    private static List<T> poolOfObjects = new List<T>();
    private static List<T> objectsInUse = new List<T>();
    private static int objectNumber = 0;
    private static int maxObjects = 500;

    public static bool ExceedingCapacity()
    {
        return objectsInUse.Count > maxObjects;
    }

    public static T GetObject(GameObject templateObject)
    {
        if (ExceedingCapacity()) return null;

        //Puts a new object in the pool if there's no more unused objects to use
        if (poolOfObjects.Count < 1)
        {
            CreateNewPooledObject(templateObject);
        }

        T nextObject = PopNextObject();
        objectsInUse.Add(nextObject); //Move the object over to the in Use list
        return nextObject;
    }

    private static T PopNextObject()
    {
        //Grab the object at the end of the list
        int index = poolOfObjects.Count - 1;
        T next = poolOfObjects[index];
        poolOfObjects.RemoveAt(index); //Take out of the unused pool
        next.InitializeObjectFromPool(); //Grabbing a function from the interface
        return next;
    }

    private static void CreateNewPooledObject(GameObject templateObject)
    {
        GameObject newObject = Object.Instantiate(templateObject);
        T newComponent = newObject.GetComponent<T>();
        newComponent.SetupObjectFirstTime(); //Another function from the interface
        newComponent.name = templateObject.name + objectNumber++;
        poolOfObjects.Add(newComponent);
    }

    //When cleaning up the object
    public static void ReturnObject(T component)
    {
        // component.ReturnObjectToPool(); //This causes a stack overflow because it recalls this function

        component.gameObject.SetActive(false);
        objectsInUse.Remove(component);
        poolOfObjects.Add(component);
    }
}