using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private Vector2 startPos;

    [Range(0, 100)]
    [SerializeField] private int moveModifier;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        Vector2 zPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        float posX = Mathf.Clamp(Mathf.Lerp(transform.position.x, startPos.x - zPos.x * moveModifier, 10f * Time.deltaTime), startPos.x - moveModifier, startPos.x + moveModifier);
        float posY = Mathf.Clamp(Mathf.Lerp(transform.position.y, startPos.y - zPos.y * moveModifier, 10f * Time.deltaTime), startPos.y - moveModifier, startPos.y + moveModifier);

        transform.position = new Vector2(posX, posY);
    }
}
