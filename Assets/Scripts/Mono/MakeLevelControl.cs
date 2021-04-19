using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeLevelControl : MonoBehaviour
{
    [SerializeField] private Camera Camera;

    public float HeightOfNewPoint;

    [SerializeField] private float SensetiveX = 1, SensetiveY = 1;
    public Vector3 Sight { get => _sight; set => _sight = value; }
    private Vector3 _sight;
    //private Vector3 pos1, pos2;
    private float UpdateSens;


    void Start() { Cursor.lockState = CursorLockMode.Locked; }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.LeftControl)) { StateCursor = CursorLockMode.Confined; }

        if (StateCursor == CursorLockMode.Locked)
        {
            CameraSizeScrolling();
            SightMovement();
        }
    }


    public CursorLockMode StateCursor
    {
        get => Cursor.lockState;
        set
        {
            if (Cursor.lockState == CursorLockMode.Locked) Cursor.lockState = CursorLockMode.None;
            else Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void CameraSizeScrolling()
    {
        float dScroll = 0;
        if (Input.GetKey(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Plus)) dScroll = 0.3f;
        else if (Input.GetKey(KeyCode.KeypadMinus) || Input.GetKeyDown(KeyCode.Minus)) dScroll = -0.3f;

        Camera.orthographicSize -= (dScroll * Camera.orthographicSize / 13);
        if (Camera.orthographicSize <= 0.01f) { Camera.orthographicSize = 1; }

        UpdateSens = Camera.orthographicSize / 10;
    }
    private void SightMovement()
    {
        if (StateCursor == CursorLockMode.Locked)
        {
            float addX, addY;
            addX = Input.GetAxis("Mouse X") * SensetiveX * UpdateSens;
            addY = Input.GetAxis("Mouse Y") * SensetiveY * UpdateSens;
            Sight += new Vector3(addY, 0, -addX);
            Sight = new Vector3(Sight.x, Camera.transform.position.y - 1, Sight.z);
            Camera.transform.position = new Vector3(Sight.x, Camera.transform.position.y, Sight.z);
        }
    }
    public bool boolDrawSight = true;
    private void OnDrawGizmos()
    {
        if (boolDrawSight)
        {
            DrawSight();

        }
    }

    private void DrawSight()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.position + Vector3.down * Camera.orthographicSize / 5.0f, Vector3.one * Camera.orthographicSize/30.0f);
    }
}
