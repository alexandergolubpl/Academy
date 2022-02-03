using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private LayerMask raycastMask;
    [SerializeField] private LayerMask explosionMask;

    private float multiplier = 5;
    private bool isMovement;
    private Collider2D playerCollider;
    private Rigidbody2D rb;

    public GameObject _field;
    public GameObject bomb;
    public GameObject _bomb;

    private void Start()
    {
        playerCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        if (isMovement)
        {
            return;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            PlaceBomb();
        }
        var sumDir = Vector2.zero;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            sumDir += Vector2.left;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            sumDir += Vector2.right;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            sumDir += Vector2.up;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            sumDir += Vector2.down;
        }
        if (sumDir != Vector2.zero)
        {
            MovePlayerTo(sumDir);
        }

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
    private void PlaceBomb()
    {
        if (_bomb == null)
        {
            _bomb = Instantiate(bomb, transform.position, Quaternion.identity, _field.transform);
        }
    }

    private void MovePlayerTo(Vector2 dir)
    {
        if (Raycast(dir))
        {
            return;
        }
        var pos = (Vector2)transform.position + dir * multiplier;
        rb.MovePosition(pos);
    }

    private bool Raycast(Vector2 dir)
    {
        var hit = Physics2D.Raycast(transform.position, dir, 1f * multiplier, raycastMask);

        bool hitBool = hit.collider != null;
        return hitBool;
    }

    private GameObject RaycastFromCamera()
    {
        var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        return hit.collider != null ? hit.collider.gameObject : null;
    }
}
