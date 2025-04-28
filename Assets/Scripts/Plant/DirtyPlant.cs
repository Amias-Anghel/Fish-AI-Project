using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtyPlant : MonoBehaviour, IDirty
{
    private EnvController envController;
    [SerializeField] private float dirtiness = 0.0001f;

    void Start()
    {
        envController = FindObjectOfType<EnvController>();
        envController.SpeedUpDirtyWater(dirtiness);
    }

    public void IsCleaned() {
        envController.SpeedDownDirtyWater(dirtiness);
        Destroy(gameObject);
    }
}
