using UnityEngine;
using System.Collections;

public class Pokeball : MonoBehaviour
{
    [SerializeField]
    private float throwSpeed = 35f;
    private float speed;
    private float lastMouseX, lastMouseY;

    private bool thrown;
    private bool holding;
    private bool canThrow;

    private Rigidbody _rigidbody;
    private Vector3 newPosition;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        EventManager.Instance.AddListener<OnTextCounterDone>(OnTextCounterDoneListener);
        EventManager.Instance.AddListener<OnPokedexOpenEvent>(OnPokedexOpenEventListener);
        EventManager.Instance.AddListener<OnPokedexClosedEvent>(OnPokedexClosedEventListener);
        canThrow = true;
        Reset();
    }

    private void OnDestroy()
    {
        if (EventManager.HasInstance())
        {
            EventManager.Instance.RemoveListener<OnTextCounterDone>(OnTextCounterDoneListener);
            EventManager.Instance.RemoveListener<OnPokedexOpenEvent>(OnPokedexOpenEventListener);
            EventManager.Instance.RemoveListener<OnPokedexClosedEvent>(OnPokedexClosedEventListener);
        }
    }

    void Update()
    {
        if (!canThrow)
        {
            return;
        }

        if (holding)
        {
            OnTouch();
        }

        if (thrown)
        {
            return;
        }

        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                if (hit.transform == transform)
                {
                    holding = true;
                    transform.SetParent(null);
                }
            }
        }

        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            if (lastMouseY < Input.GetTouch(0).position.y)
            {
                ThrowBall(Input.GetTouch(0).position);
            }
        }

        if (Input.touchCount == 1)
        { 
            lastMouseX = Input.GetTouch(0).position.x;
            lastMouseY = Input.GetTouch(0).position.y;
        }
    }

    void Reset()
    {
        CancelInvoke();
        transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.15f, Camera.main.nearClipPlane * 25f));
        newPosition = transform.position;
        thrown = holding = false;

        _rigidbody.useGravity = false;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        transform.SetParent(Camera.main.transform);
    }

    void OnTouch()
    {
        Vector3 mousePos = Input.GetTouch(0).position;
        mousePos.z = Camera.main.nearClipPlane * 25f;

        newPosition = Camera.main.ScreenToWorldPoint(mousePos);

        transform.localPosition = Vector3.Lerp(transform.localPosition, newPosition, 50f * Time.deltaTime);
    }

    void ThrowBall(Vector2 mousePos)
    {
        _rigidbody.useGravity = true;

        float differenceY = (mousePos.y - lastMouseY) / Screen.height * 100;
        speed = throwSpeed * differenceY;

        float x = (mousePos.x / Screen.width) - (lastMouseX / Screen.width);
        x = Mathf.Abs(Input.GetTouch(0).position.x - lastMouseX) / Screen.width * 100 * x;

        Vector3 direction = new Vector3(x, 0f, 1f);
        direction = Camera.main.transform.TransformDirection(direction);

        _rigidbody.AddForce((direction * speed / 2f) + (Vector3.up * speed));

        holding = false;
        thrown = true;

        Invoke("Reset", 5.0f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        CancelInvoke();

        _rigidbody.useGravity = false;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.Euler(0f, 180f, 0f);

        EventManager.Instance.Trigger(new OnPokemonHittedEvent
        {
            pokemonHitted = collision.gameObject,
            wasCaptured = WasCaptured()
        });
    }

    private bool WasCaptured()
    {
        return Random.Range(0, 1f) > 0.5f;
    }

    private void OnTextCounterDoneListener(OnTextCounterDone e)
    {
        Reset();
    }

    private void OnPokedexOpenEventListener(OnPokedexOpenEvent e)
    {
        canThrow = false;
    }

    private void OnPokedexClosedEventListener(OnPokedexClosedEvent e)
    {
        canThrow = true;
    }
}