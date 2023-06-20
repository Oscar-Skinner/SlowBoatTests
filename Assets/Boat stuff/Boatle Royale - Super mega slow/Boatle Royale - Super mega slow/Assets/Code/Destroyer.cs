using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    public static List<GameObject> boatsToDestroy = new List<GameObject>();

    void Update()
    {
        if (boatsToDestroy != null)
        {
            foreach (var boats in boatsToDestroy)
            {
                StartCoroutine(DestroyTheObject(boats));
            }
            boatsToDestroy.Clear();
        }
    }
    
    private IEnumerator DestroyTheObject(GameObject boatObj)
    {
        yield return new WaitForSeconds(1);
        Destroy(boatObj);
    }
}
