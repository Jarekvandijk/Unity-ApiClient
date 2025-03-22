using UnityEngine;

public class DragAndHover : MonoBehaviour
{
    public float scaleFactor = 1.1f;
    private Vector3 originalScale;

    [SerializeField] private bool isDragging = false;

    private float minX = 136f;
    private float maxX = 1755f;
    private float minY = 136f;
    private float maxY = 674f;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (isDragging)
        {
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            newPosition.z = 0;

            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

            transform.position = newPosition;
        }
    }

    void OnMouseEnter()
    {
        if (!isDragging)
        {
            transform.localScale = originalScale * scaleFactor;
        }
    }

    void OnMouseExit()
    {
        if (!isDragging)
        {
            transform.localScale = originalScale;
        }
    }

    private void OnMouseDown()
    {
        isDragging = true;
        transform.localScale = originalScale;
    }

    private void OnMouseUp()
    {
        isDragging = false;
    }
}
