using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserLimits : MonoBehaviour
{
    [SerializeField] private Transform upleft;
    [SerializeField] private Transform downleft;
    [SerializeField] private Transform upright;
    [SerializeField] private Transform downright;

    public bool IsInLimits(Vector2 pos) {
        return pos.x >= upleft.position.x && pos.x <= upright.position.x 
            && pos.y >= downleft.position.y && pos.y <= upleft.position.y;
    }

    // TO DO: aquarium limits checks for fish
}
