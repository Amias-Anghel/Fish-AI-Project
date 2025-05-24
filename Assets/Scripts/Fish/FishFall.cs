using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishFall : MonoBehaviour
{
    private bool falling;
    private GameObject fish;

    [Header("— Random Variant —")]
    public bool isRandom;
    [SerializeField] GameObject[] colorSet;

    public void SetFall()
    {
        falling = true;
        fish = transform.parent.parent.gameObject;

        fish.GetComponent<SwimAgent>().enabled = false;
        fish.GetComponent<FishVisuals>().enabled = false;
        transform.parent.Find("body").gameObject.SetActive(false);
        transform.parent.Find("head").gameObject.SetActive(false);
        fish.GetComponent<Rigidbody2D>().gravityScale = 10f;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Wall") && falling)
        {
            falling = false;
            fish.GetComponent<FishVisuals>().enabled = true;
            fish.GetComponent<SwimAgent>().enabled = true;
            transform.parent.Find("body").gameObject.SetActive(true);
            transform.parent.Find("head").gameObject.SetActive(true);
            fish.GetComponent<Rigidbody2D>().gravityScale = 0.05f;

            if (isRandom)
            {
                SetRandomData();
            }
        }
    }

    private void SetRandomData()
    {
        AgentsManager agents = fish.GetComponent<AgentsManager>();
        agents.swimAgent.SetRandom();
        agents.attackAgent.SetRandom();

        foreach (GameObject g in colorSet)
        {
            Destroy(g);
        }
    }
}
