using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Visual References")]
    [SerializeField] private SpriteRenderer bulletRenderer;

    private Vector2 position;
    private Vector2 direction;

    public Vector2 Position => position;
    public Vector2 Direction => direction;

    public void SetupPrefabReferences(SpriteRenderer rendererComponent)
    {
        bulletRenderer = rendererComponent;
    }

    public void Initialize(Vector2 startPosition, Vector2 moveDirection)
    {
        position = startPosition;
        direction = moveDirection;
        transform.position = new Vector3(startPosition.x, startPosition.y, 0f);
    }

    public void UpdatePosition(Vector2 newPosition)
    {
        position = newPosition;
        transform.position = new Vector3(newPosition.x, newPosition.y, 0f);
    }
}
