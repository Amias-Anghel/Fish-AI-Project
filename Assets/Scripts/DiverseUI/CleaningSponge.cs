using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleaningSponge : MonoBehaviour
{
    private Vector2 initialPos;
    private UserLimits userLimits;
    private EnvController envController;
    [SerializeField] private GameObject spongeShelf;
    [SerializeField] private GameObject spongeHand;

    void Start()
    {
        userLimits = FindObjectOfType<UserLimits>();
        envController = FindObjectOfType<EnvController>();
        initialPos = transform.position;
        spongeHand.SetActive(false);
        spongeShelf.SetActive(true);
    }

    void OnMouseUp()
    {
        transform.position = initialPos;
        spongeHand.SetActive(false);
        spongeShelf.SetActive(true);
    }

    void OnMouseDrag()
    {
        spongeHand.SetActive(true);
        spongeShelf.SetActive(false);

        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Camera.main.transform.position.z;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0;

        transform.position = mouseWorldPos;

        if (userLimits.IsInAquariumLimits(transform.position)) {
            envController.ClearWater(0.005f);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Dirty")) {
            collision.GetComponent<IDirty>().IsCleaned();
        }
    }
}
