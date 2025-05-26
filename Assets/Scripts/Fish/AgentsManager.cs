using System.Collections;
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
        Die();
    }

    public void Die()
    {
        GameObject dead = Instantiate(deadFish, transform.parent);
        dead.transform.localPosition = transform.localPosition;
        dead.transform.localScale = transform.localScale;
        swimAgent.envObservator.RemoveFish(transform);
        Destroy(gameObject);
    }
}
