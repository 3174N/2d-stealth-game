using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Variables

    public float maxZoom;
    public float minZoom;

    public float zoomAmount;
    public float zoomSpeed;
    public float scrollSpeed;

    public bool scrollWithMouse;
    public bool scrollWithKeyboard;

    public Transform startPos;

    private Camera _camera;

    #endregion

    // Start is called before the first frame update
    private void Start()
    {
        _camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    private void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, -50);
        
        // Zoom
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            if (!(_camera.orthographicSize <= minZoom))
                _camera.orthographicSize -= zoomAmount * Time.deltaTime;

            // Strategic zoom
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = Vector3.Lerp(transform.position, (Vector2) mousePos, zoomSpeed * Time.deltaTime);
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            if ((_camera.orthographicSize < maxZoom))
                _camera.orthographicSize += zoomAmount * Time.deltaTime;

            // Strategic zoom
            transform.position = Vector3.Lerp(transform.position, (Vector2) startPos.position, zoomSpeed * Time.deltaTime);
        }

        // Side scrolling
        if (scrollWithMouse)
        {
            if (Input.mousePosition.y >= Screen.height * 0.95)
            {
                transform.Translate(Vector3.up * (Time.deltaTime * scrollSpeed), Space.World);
            }

            if (Input.mousePosition.y <= Screen.height - Screen.height * 0.95)
            {
                transform.Translate(Vector3.up * -(Time.deltaTime * scrollSpeed), Space.World);
            }

            if (Input.mousePosition.x >= Screen.width * 0.95)
            {
                transform.Translate(Vector3.right * (Time.deltaTime * scrollSpeed), Space.World);
            }

            if (Input.mousePosition.x <= Screen.width - Screen.width * 0.95)
            {
                transform.Translate(Vector3.right * -(Time.deltaTime * scrollSpeed), Space.World);
            }
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (scrollWithKeyboard)
        {
            transform.Translate(Vector3.up * (Time.deltaTime * vertical * scrollSpeed), Space.World);
            transform.Translate(Vector3.right * (Time.deltaTime * horizontal * scrollSpeed), Space.World);
        }
    }
}