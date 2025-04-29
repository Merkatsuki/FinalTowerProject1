using UnityEngine;

[ExecuteAlways]
public class ParallaxLayer : MonoBehaviour
{
    [Range(0f, 1f)] public float parallaxFactor = 0.5f;
    public bool infiniteScrollX = false;
    public bool infiniteScrollY = false;

    private Transform cam;
    private Vector3 lastCamPos;
    private Vector2 textureUnitSize;

    void Start()
    {
        cam = Camera.main.transform;
        lastCamPos = cam.position;

        if (TryGetComponent<SpriteRenderer>(out var sr) && sr.sprite != null)
        {
            var sprite = sr.sprite;
            var texture = sprite.texture;
            textureUnitSize = new Vector2(
                texture.width / sprite.pixelsPerUnit,
                texture.height / sprite.pixelsPerUnit
            );
        }
    }

    void Update()
    {
        Vector3 deltaMovement = cam.position - lastCamPos;
        transform.position += new Vector3(deltaMovement.x * parallaxFactor, deltaMovement.y * parallaxFactor, 0);
        lastCamPos = cam.position;

        if (infiniteScrollX)
        {
            float offsetX = cam.position.x - transform.position.x;
            if (Mathf.Abs(offsetX) >= textureUnitSize.x)
            {
                float offset = (offsetX % textureUnitSize.x);
                transform.position = new Vector3(cam.position.x + offset, transform.position.y, transform.position.z);
            }
        }

        if (infiniteScrollY)
        {
            float offsetY = cam.position.y - transform.position.y;
            if (Mathf.Abs(offsetY) >= textureUnitSize.y)
            {
                float offset = (offsetY % textureUnitSize.y);
                transform.position = new Vector3(transform.position.x, cam.position.y + offset, transform.position.z);
            }
        }
    }
}
