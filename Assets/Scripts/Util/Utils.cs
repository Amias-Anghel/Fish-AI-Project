using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static void Shuffle<T>(this T[] array)
    {
        int n = array.Length;
        for (int i = n - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1); // Include index `i`
            T temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }
}

// TO DO: aquarium limits checks for fish
// TO DO: Food script: comment for training
// TO DO: Fish Agent script: comment end of life
// TO DO: Fish Head script: food collect
