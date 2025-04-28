using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFood
{
    public void IsEaten(EnvObservator envObservator, bool training);
}
