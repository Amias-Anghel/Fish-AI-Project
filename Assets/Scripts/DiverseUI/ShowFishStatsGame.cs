using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowFishStatsGame : MonoBehaviour
{
    [SerializeField] private FishStatsVisual fishStatsVisual;
    [SerializeField] private LayerMask fishLayer;

    void Start()
    {
        fishStatsVisual.gameObject.SetActive(false);   
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, fishLayer);

            if (hit.collider != null)
            {
                Rigidbody2D fishRigidbody = hit.collider.GetComponentInParent<Rigidbody2D>();
                if (fishRigidbody != null)
                {
                    fishStatsVisual.gameObject.SetActive(true);
                    fishStatsVisual.fishAgent = fishRigidbody.gameObject.GetComponent<FishAgent>();
                }
            } else {
                fishStatsVisual.gameObject.SetActive(false);
            }
        }
    }

}
