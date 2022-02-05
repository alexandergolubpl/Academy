using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameManager GameManager;
    List<GameObject> items = new List<GameObject>();
    [SerializeField] private LayerMask raycastMask;
    [SerializeField] private LayerMask explosionMask;

    private float multiplier = 5;
    private bool isMovement;
    private Collider2D playerCollider;
    private Rigidbody2D rb;

    public GameObject _field;
    public GameObject bomb;
    private GameObject _bomb;
    private void Awake()
    {
    }
    private void Start()
    {
        playerCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }
    private void PlaceBomb()
    {
        if (_bomb == null)
        {
            _bomb = Instantiate(bomb, transform.position, Quaternion.identity, _field.transform);
        }
    }

    private void Update()
    {
        if (isMovement)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow)) MovePlayerTo(Vector2.left);
        if (Input.GetKeyDown(KeyCode.RightArrow)) MovePlayerTo(Vector2.right);
        if (Input.GetKeyDown(KeyCode.UpArrow)) MovePlayerTo(Vector2.up);
        if (Input.GetKeyDown(KeyCode.DownArrow)) MovePlayerTo(Vector2.down);
        if (Input.GetKeyDown(KeyCode.Space))  PlaceBomb();

        if (Input.GetMouseButtonDown(0))
        {
            var obj = RaycastFromCamera();
            if (obj != null && obj.CompareTag("Explosive"))
            {
                Destroy(obj);

                var colliders = Physics2D.OverlapCircleAll(obj.transform.position, 1f, explosionMask);

                foreach (var cldr in colliders)
                {
                    Destroy(cldr.gameObject);
                }
            }
        }
    }
    private void MovePlayerTo(Vector2 dir)
    {
        if (Raycast(dir))
        {
            return;
        }

        isMovement = true;
        if (dir.x>0)
        {
            transform.GetChild(0).transform.DOScale(new Vector2(1,1), 0.01f);
        }
        if (dir.x < 0)
        {
            transform.GetChild(0).transform.DOScale(new Vector2(-1, 1), 0.01f);
        }
        var pos = (Vector2)transform.position + dir;
        transform.DOMove(pos, 0.1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            isMovement = false;
        });
    }
    private bool Raycast(Vector2 dir)
    {
        var hit = Physics2D.Raycast(transform.position, dir, 1f, raycastMask);
        return hit.collider != null;
    }

    private GameObject RaycastFromCamera()
    {
        var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        return hit.collider != null ? hit.collider.gameObject : null;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Collectable")
        {
            items.Add(collision.gameObject);
            collision.gameObject.GetComponent<Collider2D>().enabled = false;
            collision.gameObject.transform.DOScale(0, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
            {

                foreach (var item in items)
                {
                    Destroy(item);
                }
                GameManager.checkGameComplete();
            });
            GameManager.AddPoints();
        }
    }
}
