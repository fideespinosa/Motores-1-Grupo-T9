using UnityEngine;
using UnityEngine.UI;
public class StaticScript : MonoBehaviour
{
    public RawImage noiseImage;
    public float speed = 5f;

    void Update()
    {
        Rect uv = noiseImage.uvRect;
        uv.x += Time.deltaTime * speed;
        uv.y += Time.deltaTime * speed * 0.7f;
        noiseImage.uvRect = uv;
    }
}
