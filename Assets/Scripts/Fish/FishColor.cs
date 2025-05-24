using UnityEngine;
using UnityEngine.UIElements;

public class FishColor : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] sprites;
    [SerializeField] private float changeSpeed = 100f;
    private Color color;

    void Start()
    {
        color = sprites[0].color;
    }

    void Update()
    {
        Color.RGBToHSV(color, out float h, out float s, out float v);

        h += changeSpeed * Time.deltaTime / 360f;
        h %= 1f;
        
        color = Color.HSVToRGB(h, s, v);

        foreach (var spr in sprites)
            spr.color = color;
    }
}
