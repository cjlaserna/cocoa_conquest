using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private bool hasBeenCollected = false;

    public bool HasBeenCollected
    {
        get { return hasBeenCollected; }
        set { hasBeenCollected = value; }
    }
}
