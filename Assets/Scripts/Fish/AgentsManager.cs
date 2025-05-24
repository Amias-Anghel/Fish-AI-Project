using System.Collections;
using System.Collections.Generic;
using UnityEditor.Hardware;
using UnityEngine;

public class AgentsManager : MonoBehaviour
{
    public SwimAgent swimAgent;
    public AttackAgent attackAgent;

    [SerializeField] private float lifeSpanSeconds = 60f;
    [SerializeField] private GameObject deadFish;

    void Start()
    {
        if (!swimAgent.isTraining)
        {
            StartCoroutine(CountLifeSpan());
        }
    }

    private IEnumerator CountLifeSpan()
    {
        yield return new WaitForSecondsRealtime(lifeSpanSeconds);
        GameObject dead = Instantiate(deadFish, transform.parent);
        dead.transform.localPosition = transform.localPosition;
        dead.transform.localScale = transform.localScale;
        Destroy(gameObject);
    }
}
