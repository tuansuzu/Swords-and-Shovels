using UnityEngine;
using UnityEngine.Events;

public class MouseManager : MonoBehaviour
{
    public LayerMask clickableLayer; // layermask used to isolate raycasts against clickable layers

    public Texture2D pointer; // normal mouse pointer
    public Texture2D target; // target mouse pointer
    public Texture2D doorway; // doorway mouse pointer
    public Texture2D sword;// sword mouse pointer

    public EventVector3 OnClickEnvironment;
    public EventGameObject OnClickAttackable;

    private bool _useDefaultCursor = false;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
        }
        
    }

    void HandleGameStateChanged(GameManager.GameState currentState, GameManager.GameState previousState)
    {
        _useDefaultCursor = currentState == GameManager.GameState.PAUSED;

    }

    void Update()
    {
        if (_useDefaultCursor)
        {
            Cursor.SetCursor(pointer, new Vector2(16, 16), CursorMode.Auto);
            return;
        }

        // Raycast into scene
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 50, clickableLayer.value))
        {

            //override cursor
            Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);

            bool door = false;
            if (hit.collider.gameObject.tag == "Doorway")
            {
                Cursor.SetCursor(doorway, new Vector2(16, 16), CursorMode.Auto);
                door = true;
            }

            bool chest = false;
            if (hit.collider.gameObject.tag == "Chest")
            {
                Cursor.SetCursor(pointer, new Vector2(16, 16), CursorMode.Auto);
                door = true;

            }

            bool isAttackable = hit.collider.GetComponent(typeof(IAttackable)) != null;
            if (isAttackable)
            {
                Cursor.SetCursor(sword, new Vector2(16, 16), CursorMode.Auto);
            }

            // if environment surface is clicked, invoke callbacks.
            if (Input.GetMouseButtonDown(0))
            {
                if (door)
                {
                    Transform doorway = hit.collider.gameObject.transform;
                    OnClickEnvironment.Invoke(doorway.position + doorway.forward * 10);
                }
                else if (isAttackable)
                {
                    GameObject attackable = hit.collider.gameObject;
                    OnClickAttackable.Invoke(attackable);
                }
                else if (!chest) {
                    OnClickEnvironment.Invoke(hit.point);
                }
               
            }

        }
        else
        {
            Cursor.SetCursor(pointer, Vector2.zero, CursorMode.Auto);
        }
    }
}

[System.Serializable]
public class EventVector3 : UnityEvent<Vector3> { }

[System.Serializable]
public class EventGameObject : UnityEvent<GameObject> { }
