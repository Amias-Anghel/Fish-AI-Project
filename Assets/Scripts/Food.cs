using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public void Eaten() {
        Destroy(gameObject);
    }
}
