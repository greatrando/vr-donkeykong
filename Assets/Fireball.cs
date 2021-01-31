using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Fireball : MonoBehaviour
{


    private const float SPEED = 2f;
    private const int CHANGE_DIRECTION_MILLISECONDS = 500;


    public float Gravity = -9.8f / 10;


    private enum DIRECTION
    {
        LEFT,
        RIGHT,
        UP,
        BACK
    }


    private Rigidbody rigidBody;
    private CollisionHelper _collisionHelper;
    private List<GameObject> _triggered = new List<GameObject>();
    private bool _endOfLife = false;

    private DIRECTION _direction;
    private DateTime _nextDirectionEvaluation;
    private bool _isGrounded;
    private bool _nearRightRail = false;
    private bool _nearLefttRail = false;
    private Vector3 _oneTimeMovement;


    void Start()
    {
        _nextDirectionEvaluation = DateTime.Now.AddMilliseconds(CHANGE_DIRECTION_MILLISECONDS);
        _direction = DIRECTION.RIGHT;

        GameObject GroundCollider = this.transform.Find("GroundCollider").gameObject;
        _collisionHelper = GroundCollider.GetComponent<CollisionHelper>();
        // _collisionHelper.IgnoreObjects.Add(this.gameObject);
        _collisionHelper.OnEnter += OnGroundCollisionEnter;
        _collisionHelper.OnExit += OnGroundCollisionExit;

        rigidBody = gameObject.GetComponent<Rigidbody>();
    }


    void Update()
    {
        CheckForEndOfLife();
        CheckForChangeInDirection();
        UpdateMovement();
    }


    private void SetNextDirectionEvaluation()
    {
        _nextDirectionEvaluation = DateTime.Now.AddMilliseconds(CHANGE_DIRECTION_MILLISECONDS);
    }


    private void CheckForEndOfLife()
    {
        if (_endOfLife)
        {
            Destroy(this.gameObject);
        }
    }


    private void CheckForChangeInDirection()
    {
        if (_direction == DIRECTION.UP || _direction == DIRECTION.BACK || _nextDirectionEvaluation > DateTime.Now) return;

        SetNextDirectionEvaluation();

        int random = UnityEngine.Random.Range(0, 100);
        if (random < 34)
        {
            _direction = DIRECTION.LEFT;
        }
        else if (random < 67)
        {
            _direction = DIRECTION.RIGHT;
        }
    }


    private void UpdateMovement()
    {
        Vector3 movement;

        if (!_isGrounded)
        {
            if (_direction == DIRECTION.LEFT)
            {
                _direction = DIRECTION.RIGHT;
                SetNextDirectionEvaluation();
                _oneTimeMovement = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + 1.0f);
            }
            else if (_direction == DIRECTION.RIGHT)
            {
                _direction = DIRECTION.LEFT;
                SetNextDirectionEvaluation();
                _oneTimeMovement = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - 1.0f);
            }
        }

        switch (_direction)
        {
            case DIRECTION.BACK:
                movement = new Vector3(-SPEED, 0, 0);
                break;
            case DIRECTION.UP:
                movement = new Vector3(0, SPEED, 0);
                break;
            case DIRECTION.RIGHT:
                movement = new Vector3(0, 0, SPEED);
                break;
            default: //DIRECTION.LEFT:
                movement = new Vector3(0, 0, -SPEED);
                break;
        }

        movement *= Time.deltaTime;

        if (_oneTimeMovement != Vector3.zero)
        {
            this.transform.position = _oneTimeMovement;
            _oneTimeMovement = Vector3.zero;
        }

        this.transform.position += movement;
    }


    private void OnGroundCollisionEnter(CollisionHelper sender, GameObject gameObject)
    {
        if (gameObject.name != "Top") return;

        _isGrounded = true;
    }


    private void OnGroundCollisionExit(CollisionHelper sender, GameObject gameObject)
    {
        if (sender.CurrentCollisions.Where(c => c.gameObject.name == "Top").Count() > 0) return;

        _isGrounded = false;
    }


    public void OnTriggerEnter(Collider col)
    {
        if (
                (col.gameObject.transform.parent != null && col.gameObject.transform.parent.name == "Barrels")
            )
        {
            _endOfLife = true;
            return;
        }

        if (col.gameObject.name == "oil-barrel-cylinder" || col.gameObject.name == "oil-barrel")
        {
            _direction = DIRECTION.RIGHT;
            SetNextDirectionEvaluation();
            return;
        }

        if (col.gameObject.name.EndsWith("Rail"))
        {
            if (col.gameObject.transform.parent.parent.gameObject.name != "Floor6")
            {
                if (!_triggered.Contains(col.gameObject))
                {
                    _triggered.Add(col.gameObject);
                }
            }
        }

        if (_direction == DIRECTION.BACK && col.gameObject.name.Contains("Wall"))
        {
            _direction = DIRECTION.LEFT;

            _nextDirectionEvaluation = DateTime.Now;
            CheckForChangeInDirection();
        }
        if (_direction == DIRECTION.BACK)
        {
            return;
        }
        
        _nearRightRail = (_triggered.Where(g => g.name == "RightRail").Count() > 0);
        _nearLefttRail = (_triggered.Where(g => g.name == "LeftRail").Count() > 0);

        if (_nearLefttRail && _nearRightRail)
        {
            int random = UnityEngine.Random.Range(0, 50);
            if (random > 10)
            {
                _direction = DIRECTION.UP;
                SetNextDirectionEvaluation();
                _isGrounded = false;
            }
        }
    }
  

    public void OnTriggerExit(Collider col)
    {
        if (_triggered.Contains(col.gameObject))
        {
            _triggered.Remove(col.gameObject);
        }

        _nearRightRail = (_triggered.Where(g => g.name == "RightRail").Count() > 0);
        _nearLefttRail = (_triggered.Where(g => g.name == "LeftRail").Count() > 0);

        if (!_nearRightRail && !_nearLefttRail && _direction == DIRECTION.UP)
        {
            _direction = DIRECTION.BACK;
            SetNextDirectionEvaluation();
            _oneTimeMovement = new Vector3(this.transform.position.x - 1.0f, this.transform.position.y, this.transform.position.z);
        }
    }


}
