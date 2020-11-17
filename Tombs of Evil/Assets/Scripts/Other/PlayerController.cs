using UnityEngine;

//[RequireComponent(typeof(WalkManager),typeof(AttackManager))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController current;

    public float heightMax = 20f;
    public float heightMin = 3f;
    public float scrollSpeed = 5f;

    public Vector3 mousepos;

    [Range(0, 20)]
    public float DirectionTolerance;

    private float cameraDistance = 5f;
    private Vector3 offset = new Vector3(0.5f, 0.5f,0);
    private AnimationController animationController;


    private void Awake()
    {
        current = this;
    }

    private void Start()
    {
        GameEvents.current.OnDungeonCreated += SpawnInDungeon;
        animationController = GetComponent<AnimationController>();
    }


    void Update()
    {

        CheckInput();

        SetCameraController();// for testing 
        TestInput(); //for testing 

    }

    #region Mobile Game Input

    private void CheckInput()
    {
        //mobile input 
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Derection mouseDirection = GetDirection(touch.position);
            var currentSelected = GetSeleced(touch.position);

            //attacking
            if (currentSelected != null && touch.phase == TouchPhase.Began)
            {
                float distance = Vector3.Distance(transform.position, currentSelected.GetPosition());

                if (distance < 5f)
                {           
                    if (mouseDirection != Derection.None)
                    {
                        currentSelected.TakeDamage(Random.Range(0, 50));
                        animationController.Attack(mouseDirection);
                        return;
                    }
                }
            }

            //walking 
            if (mouseDirection != Derection.None)
            {
                animationController.WalkTowards(mouseDirection, true);
            }
        }
    }

    public void SpawnInDungeon()
    {
        animationController.isWalking = false;
        MapGenerator.current.MoveInDungeon(transform);
        FogOfWar.current.UpdateFog();
    }

    private Vector3 GetMousePosition(Vector3 touchPos)
    {
        var mousePos = touchPos;
        mousePos.z = 20; // select distance = 10 units from the camera
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(mousePos);
        touchPosition.z = 0;
        mousepos = touchPosition;

        return touchPosition;
    }

    private ISelectable GetSeleced(Vector3 touchPos)
    {
        Vector3 inputPosition = GetMousePosition(touchPos);
        var selectables = SelectableManager.current.selectables;
        //check 
        foreach (var selectable in selectables)
        {
            Vector3 directionToTarget = inputPosition - selectable.GetPosition();
            float distance = directionToTarget.sqrMagnitude;
            if (distance < 1f)
            {
                return selectable;
            }
        }

        return null;
    }

    private Derection GetDirection(Vector3 touchPos)
    {
        Derection currentDerection = Derection.None;
        Vector3 visualCenter = transform.position + offset;
        var direction = touchPos - Camera.main.WorldToScreenPoint(visualCenter);
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        //Debug.Log(angle);

        if (angle < (135 + DirectionTolerance) && angle > (135 - DirectionTolerance))
        {
            currentDerection = Derection.UpLeft;
        }
        if (angle < (90 + DirectionTolerance) && angle > (90 - DirectionTolerance))
        {
            currentDerection = Derection.Up;
        }
        if (angle < (45 + DirectionTolerance) && angle > (45 - DirectionTolerance))
        {
            currentDerection = Derection.UpRight;
        }
        if (angle > (0 - DirectionTolerance) && angle < (0 + DirectionTolerance))
        {
            currentDerection = Derection.Right;
        }
        if (angle > (-45 - DirectionTolerance) && angle < (-45 + DirectionTolerance))
        {
            currentDerection = Derection.DownRight;
        }
        if (angle > (-90 - DirectionTolerance) && angle < (-90 + DirectionTolerance))
        {
            currentDerection = Derection.Down;
        }
        if (angle > (-135 - DirectionTolerance) && angle < (-135 + DirectionTolerance))
        {
            currentDerection = Derection.DownLeft;
        }
        if (angle > (180 - DirectionTolerance) && angle < 180
            || angle < (-180 + DirectionTolerance) && angle > -180)
        {
            currentDerection = Derection.Left;
        }

        return currentDerection;
    }

    #endregion

    #region Pc Testing Game Input

    private void TestInput()
    {
        Derection currentDerection;
        //pc keys input
        if (Input.GetKeyDown(KeyCode.W))
        {
            currentDerection = Derection.Up;
            animationController.WalkTowards(currentDerection, true);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            currentDerection = Derection.Down;
            animationController.WalkTowards(currentDerection, true);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            currentDerection = Derection.Right;
            animationController.WalkTowards(currentDerection, true);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            currentDerection = Derection.Left;
            animationController.WalkTowards(currentDerection, true);
        }

        //pc mouse input
        if (Input.GetMouseButtonDown(0)) // walk 
        {
            Vector3 touchPosition = GetMousePosition();
            Derection mouseDirection = GetDirection();

            if (mouseDirection == Derection.None)
                return;

            animationController.WalkTowards(mouseDirection, true);

        }
        if (Input.GetMouseButtonDown(1)) // Selected stuff
        {

            var currentSelected = GetSeleced();

            if (currentSelected == null)
                return;

            float distance = Vector3.Distance(transform.position, currentSelected.GetPosition());

            if (distance > 5f)
                return;

            Derection mouseDirection = GetDirection();
            if (mouseDirection == Derection.None)
                return;

            currentSelected.TakeDamage(Random.Range(0, 50));
            animationController.Attack(mouseDirection);

        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(SelectableManager.current.selectables.Count);
        }
    }

    private void SetCameraController()
    {
        cameraDistance -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        cameraDistance = Mathf.Clamp(cameraDistance, heightMin, heightMax);

        Camera.main.orthographicSize = cameraDistance;
    }

    private Vector3 GetMousePosition()
    {
        var mousePos = Input.mousePosition;
        mousePos.z = 20; // select distance = 10 units from the camera
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(mousePos);
        touchPosition.z = 0;
        mousepos = touchPosition;

        return touchPosition;
    }

    private ISelectable GetSeleced()
    {
        Vector3 inputPosition = GetMousePosition();
        var selectables = SelectableManager.current.selectables;
        //check 
        foreach (var selectable in selectables)
        {
            Vector3 directionToTarget = inputPosition - selectable.GetPosition();
            float distance = directionToTarget.sqrMagnitude;
            if (distance < 1f)
            {
                return selectable;
            }
        }

        return null;
    }

    private Derection GetDirection()
    {
        Derection currentDerection = Derection.None;
        Vector3 visualCenter = transform.position + offset;
        var direction = Input.mousePosition - Camera.main.WorldToScreenPoint(visualCenter);
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        //Debug.Log(angle);

        if (angle < (135 + DirectionTolerance) && angle > (135 - DirectionTolerance))
        {
            currentDerection = Derection.UpLeft;
        }
        if (angle < (90 + DirectionTolerance) && angle > (90 - DirectionTolerance))
        {
            currentDerection = Derection.Up;
        }
        if (angle < (45 + DirectionTolerance) && angle > (45 - DirectionTolerance))
        {
            currentDerection = Derection.UpRight;
        }
        if (angle > (0 - DirectionTolerance) && angle < (0 + DirectionTolerance))
        {
            currentDerection = Derection.Right;
        }
        if (angle > (-45 - DirectionTolerance) && angle < (-45 + DirectionTolerance))
        {
            currentDerection = Derection.DownRight;
        }
        if (angle > (-90 - DirectionTolerance) && angle < (-90 + DirectionTolerance))
        {
            currentDerection = Derection.Down;
        }
        if (angle > (-135 - DirectionTolerance) && angle < (-135 + DirectionTolerance))
        {
            currentDerection = Derection.DownLeft;
        }
        if (angle > (180 - DirectionTolerance) && angle < 180
            || angle < (-180 + DirectionTolerance) && angle > -180)
        {
            currentDerection = Derection.Left;
        }

        return currentDerection;
    }

    #endregion
}
