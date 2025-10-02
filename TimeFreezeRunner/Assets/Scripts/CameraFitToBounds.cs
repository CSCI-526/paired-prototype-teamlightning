using UnityEngine;

[ExecuteAlways, RequireComponent(typeof(Camera))]
public class CameraFitToBounds : MonoBehaviour
{
    public Collider2D bounds;   
    public float padding = 0.5f;  

    Camera cam;

    void OnEnable(){ cam = GetComponent<Camera>(); UpdateFit(); }
    void OnValidate(){ UpdateFit(); }
#if UNITY_EDITOR
    void Update(){ if (!Application.isPlaying) UpdateFit(); }
#endif

    void UpdateFit()
    {
        if (!cam || !bounds) return;
        var b = bounds.bounds;

        float halfH = b.extents.y + padding;
        float halfW = b.extents.x + padding;
        float aspect = (float)Screen.width / Screen.height;

        float sizeByHeight = halfH;
        float sizeByWidth  = halfW / aspect;

        cam.orthographic = true;
        cam.orthographicSize = Mathf.Max(sizeByHeight, sizeByWidth);
        cam.transform.position = new Vector3(b.center.x, b.center.y, cam.transform.position.z);
    }
}
