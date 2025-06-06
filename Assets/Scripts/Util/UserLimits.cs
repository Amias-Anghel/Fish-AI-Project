using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserLimits : MonoBehaviour
{
    [Header("— User Drag Limits —")]
    [SerializeField] private Transform upleft;
    [SerializeField] private Transform downleft;
    [SerializeField] private Transform upright;
    [SerializeField] private Transform downright;

    public bool IsInUserLimits(Vector2 pos)
    {
        return pos.x >= upleft.position.x && pos.x <= upright.position.x
            && pos.y >= downleft.position.y && pos.y <= upleft.position.y;
    }

    [Header("— Movement Limits —")]
    [SerializeField] public Transform aq_up;
    [SerializeField] public Transform aq_down;
    [SerializeField] public Transform aq_right;
    [SerializeField] public Transform aq_left;

    [Header("— Walls —")]
    [SerializeField] public Transform wall_aq_up;
    [SerializeField] public Transform wall_aq_down;
    [SerializeField] public Transform wall_aq_right;
    [SerializeField] public Transform wall_aq_left;

    public bool IsInAquariumLimits(Vector2 pos)
    {
        return pos.x >= wall_aq_left.position.x && pos.x <= wall_aq_right.position.x
            && pos.y >= wall_aq_down.position.y && pos.y <= wall_aq_up.position.y;
    }

    public bool IsOverAquarium(Vector2 pos)
    {
        return pos.x >= aq_left.position.x && pos.x <= aq_right.position.x
            && pos.y >= aq_up.position.y;
    }

    public Vector2 GetPositionInAquarium(float delay = 2f)
    {
        float x = Random.Range(aq_left.position.x + delay, aq_right.position.x - delay);
        float y = Random.Range(aq_down.position.y + delay, aq_up.position.y - delay);
        return new Vector2(x, y);
    }
    
}
