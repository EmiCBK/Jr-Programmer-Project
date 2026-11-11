using UnityEngine;
using UnityEngine.InputSystem;

public class UserControl : MonoBehaviour
{
    public Camera GameCamera;
    public float PanSpeed = 10.0f;
    public GameObject Marker;

    private Unit selected = null;

    private InputAction moveAction;
    private InputAction leftClickAction;
    private InputAction rightClickAction;

    private void Awake()
    {
        moveAction = new InputAction("Move", InputActionType.Value);

        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        leftClickAction = new InputAction(
            "LeftClick",
            InputActionType.Button,
            "<Mouse>/leftButton"
        );

        rightClickAction = new InputAction(
            "RightClick",
            InputActionType.Button,
            "<Mouse>/rightButton"
        );
    }

    private void OnEnable()
    {
        moveAction.Enable();
        leftClickAction.Enable();
        rightClickAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        leftClickAction.Disable();
        rightClickAction.Disable();
    }

    private void Start()
    {
        Marker.SetActive(false);
    }

    // Abstracción: reúne la lógica para seleccionar y mostrar información.
    public void HandleSelection()
    {
        var ray = GameCamera.ScreenPointToRay(
            Mouse.current.position.ReadValue()
        );

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            var unit = hit.collider.GetComponentInParent<Unit>();
            selected = unit;

            var uiInfo = hit.collider
                .GetComponentInParent<MainUIHandler.IUIInfoContent>();

            MainUIHandler.Instance.SetNewInfoContent(uiInfo);
        }
    }

    // Abstracción: reúne la lógica para dar órdenes a la unidad.
    public void HandleAction()
    {
        if (selected == null)
            return;

        var ray = GameCamera.ScreenPointToRay(
            Mouse.current.position.ReadValue()
        );

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            var building = hit.collider.GetComponentInParent<Building>();

            if (building != null)
            {
                selected.GoTo(building);
            }
            else
            {
                selected.GoTo(hit.point);
            }
        }
    }

    private void Update()
    {
        Vector2 move = moveAction.ReadValue<Vector2>();

        GameCamera.transform.position +=
            new Vector3(move.y, 0, -move.x) * PanSpeed * Time.deltaTime;

        if (leftClickAction.WasPressedThisFrame())
        {
            HandleSelection();
        }
        else if (selected != null && rightClickAction.WasPressedThisFrame())
        {
            HandleAction();
        }

        MarkerHandling();
    }

    private void MarkerHandling()
    {
        if (selected == null && Marker.activeInHierarchy)
        {
            Marker.SetActive(false);
            Marker.transform.SetParent(null);
        }
        else if (selected != null &&
                 Marker.transform.parent != selected.transform)
        {
            Marker.SetActive(true);
            Marker.transform.SetParent(selected.transform, false);
            Marker.transform.localPosition = Vector3.zero;
        }
    }
}