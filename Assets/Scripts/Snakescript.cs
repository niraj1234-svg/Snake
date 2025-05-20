using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    private Vector2 _direction = Vector2.right;
    private List<Transform> _segments = new List<Transform>();
    public Transform segmentPrefab;
    public int initialSize = 4;
    public float moveRate = 0.2f; // Time between moves (lower = faster)

    private float _nextMoveTime;

    private void Start()
    {
        ResetState();
    }

    private void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        // Move only when the timer exceeds the move rate
        if (Time.time >= _nextMoveTime)
        {
            MoveSegments();
            MoveHead();
            _nextMoveTime = Time.time + moveRate;
        }
    }

    // Handle input with direction reversal checks
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.W) && _direction != Vector2.down)
            _direction = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.S) && _direction != Vector2.up)
            _direction = Vector2.down;
        else if (Input.GetKeyDown(KeyCode.A) && _direction != Vector2.right)
            _direction = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.D) && _direction != Vector2.left)
            _direction = Vector2.right;
    }

    private void MoveSegments()
    {
        for (int i = _segments.Count - 1; i > 0; i--)
        {
            _segments[i].position = _segments[i - 1].position;
        }
    }

    private void MoveHead()
    {
        transform.position = new Vector3(
            Mathf.Round(transform.position.x) + _direction.x,
            Mathf.Round(transform.position.y) + _direction.y,
            0.0f
        );
    }

    private void Grow()
    {
        Transform segment = Instantiate(segmentPrefab);
        segment.position = _segments[_segments.Count - 1].position;
        segment.gameObject.tag = "Obstacle"; // Ensure tag is set
        _segments.Add(segment);
    }

    // Reset snake to initial state
    private void ResetState()
    {
        _direction = Vector2.right; // Reset direction to default

        // Destroy existing segments (excluding head)
        for (int i = 1; i < _segments.Count; i++)
        {
            Destroy(_segments[i].gameObject);
        }

        _segments.Clear();
        _segments.Add(transform); // Add head

        // Initialize segments in a line behind the head
        for (int i = 1; i < initialSize; i++)
        {
            Transform segment = Instantiate(segmentPrefab);
            segment.position = _segments[i - 1].position - (Vector3)_direction;
            segment.gameObject.tag = "Obstacle"; // Ensure tag is set
            _segments.Add(segment);
        }

        transform.position = Vector3.zero; // Reset head position
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Food"))
        {
            Grow();
        }
        else if (other.CompareTag("Obstacle"))
        {
            // Check if collided with a body segment (not head)
            if (other.transform != _segments[0])
            {
                ResetState();
            }
        }
    }
}