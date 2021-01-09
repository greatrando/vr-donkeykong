using System.Collections.Generic;
using UnityEngine;

public class GameBarrel : MonoBehaviour
{


    public delegate void ShouldDestroyEvent(GameObject barrel);


    private const float CONSTANT_VELOCITY = 8f;
    private const float SHORTCUT_CONSTANT_VELOCITY = 1.5f;


    private enum BARREL_DIRECTION
    {
        RIGHT,
        FALL_RIGHT,
        RIGHT_FORWARD,
        SHORTCUT_RIGHT_FORWARD,
        LEFT,
        FALL_LEFT,
        LEFT_FORWARD,
        SHORTCUT_LEFT_FORWARD
    }


    public ShouldDestroyEvent OnShouldDestroy = null;


    private Rigidbody rigidBody;
    private List<GameObject> currentCollisions = new List<GameObject>();
    private BARREL_DIRECTION _direction;
    private float _clampY = 0;
    private float _clampZ = 90f;


    void Start()
    {
        _direction = BARREL_DIRECTION.RIGHT;
        rigidBody = gameObject.GetComponent<Rigidbody>();
    }


    void Update()
    {
        Prevent_Wobble();
        Maitain_Velocity();
    }

     
    void OnCollisionEnter(Collision col) 
    {
        CheckForEndOfLife(col.gameObject);
        CheckForDirectionChange(col.gameObject);

        if (IsWall(col.gameObject)) return;
        currentCollisions.Add(col.gameObject);
    }


    void OnCollisionExit(Collision col) 
    {
        if (IsWall(col.gameObject)) return;
        currentCollisions.Remove(col.gameObject);
    }


    public void PerformShortcut()
    {
        if (_direction == BARREL_DIRECTION.RIGHT)
        {
            _direction = BARREL_DIRECTION.SHORTCUT_RIGHT_FORWARD;
        }
        if (_direction == BARREL_DIRECTION.LEFT)
        {
            _direction = BARREL_DIRECTION.SHORTCUT_LEFT_FORWARD;
        }
    }


    public void Explode()
    {
        this.GetComponent<MeshRenderer>().enabled = false;  //don't render
        this.GetComponent<Rigidbody>().detectCollisions = false; //stop collisions

        //play explosion        
        AudioSource[] sources = this.GetComponents<AudioSource>();
        AudioSource explode = sources[0];
        explode.Play();

        //destroy after playing audio
        Destroy(this.gameObject, 0.7f);
    }


    private void CheckForDirectionChange(GameObject collisionGameObject)
    {
        if (_direction == BARREL_DIRECTION.RIGHT && collisionGameObject.name == "RightWall")
        {
            _direction = BARREL_DIRECTION.FALL_RIGHT;
        }
        else if (_direction == BARREL_DIRECTION.FALL_RIGHT && collisionGameObject.name == "Top")
        {
            _direction = BARREL_DIRECTION.RIGHT_FORWARD;
        }
        else if ( (_direction == BARREL_DIRECTION.RIGHT_FORWARD || _direction == BARREL_DIRECTION.SHORTCUT_RIGHT_FORWARD) && collisionGameObject.name == "FrontWall")
        {
            _direction = BARREL_DIRECTION.LEFT;
        }
        else if (_direction == BARREL_DIRECTION.LEFT && collisionGameObject.name == "LeftWall")
        {
            _direction = BARREL_DIRECTION.FALL_LEFT;
        }
        else if (_direction == BARREL_DIRECTION.FALL_LEFT && collisionGameObject.name == "Top")
        {
            _direction = BARREL_DIRECTION.LEFT_FORWARD;
        }
        else if ( (_direction == BARREL_DIRECTION.LEFT_FORWARD || _direction == BARREL_DIRECTION.SHORTCUT_LEFT_FORWARD) && collisionGameObject.name == "FrontWall")
        {
            _direction = BARREL_DIRECTION.RIGHT;
        }
    }


    private void CheckForEndOfLife(GameObject collisionGameObject)
    {
        if (
                collisionGameObject.name == "oil-barrel-cylinder" ||
                collisionGameObject.name == "fireball" ||
                collisionGameObject.name.StartsWith("Hammer")
            )
        {
            OnShouldDestroy?.Invoke(this.gameObject);
        }
    }


    private bool IsWall(GameObject collisionGameObject)
    {
        return (
            collisionGameObject.name == "FrontWall" ||
            collisionGameObject.name == "BackWall" ||
            collisionGameObject.name == "LeftWall" ||
            collisionGameObject.name == "RightWall"
        );
    }


    private void Prevent_Wobble()
    {
        if (
                _direction == BARREL_DIRECTION.FALL_RIGHT ||
                _direction == BARREL_DIRECTION.RIGHT_FORWARD ||
                _direction == BARREL_DIRECTION.SHORTCUT_RIGHT_FORWARD ||
                _direction == BARREL_DIRECTION.FALL_LEFT ||
                _direction == BARREL_DIRECTION.LEFT_FORWARD ||
                _direction == BARREL_DIRECTION.SHORTCUT_LEFT_FORWARD
            )
        {
            if (_clampY == 0)
            {
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - 1);
                _clampY = 90;
            }
        }
        else
        {
            if (_clampY == 90)
            {
                this.transform.position = new Vector3(this.transform.position.x + 0.5f, this.transform.position.y, this.transform.position.z);
            }
            _clampY = 0;
        }
        
        Vector3 currentRotation = this.transform.eulerAngles;
        float y = _clampY - currentRotation.y;
        float z = _clampZ - currentRotation.z;
        currentRotation += new Vector3(0, y, z);
        this.transform.eulerAngles = currentRotation;
    }


    private void Maitain_Velocity()
    {
        if (_direction == BARREL_DIRECTION.FALL_RIGHT || _direction == BARREL_DIRECTION.FALL_LEFT)
        {
            return;
        }

        if (_direction == BARREL_DIRECTION.RIGHT)
        {
            rigidBody.velocity = new Vector3(0, rigidBody.velocity.y, CONSTANT_VELOCITY);
        }
        else if (_direction == BARREL_DIRECTION.RIGHT_FORWARD || _direction == BARREL_DIRECTION.LEFT_FORWARD)
        {
            rigidBody.velocity = new Vector3(CONSTANT_VELOCITY, rigidBody.velocity.y, 0);
        }
        else if (_direction == BARREL_DIRECTION.SHORTCUT_RIGHT_FORWARD || _direction == BARREL_DIRECTION.SHORTCUT_LEFT_FORWARD)
        {
            rigidBody.velocity = new Vector3(SHORTCUT_CONSTANT_VELOCITY, rigidBody.velocity.y, 0);
        }
        else if (_direction == BARREL_DIRECTION.LEFT)
        {
            rigidBody.velocity = new Vector3(0, rigidBody.velocity.y, -CONSTANT_VELOCITY);
        }
        else
        {
            rigidBody.velocity = new Vector3(0, 0, 0);
        }
    }


}
