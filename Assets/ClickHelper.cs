using System;
using System.Collections.Generic;
using UnityEngine;


public class ClickHelper : MonoBehaviour
{


    public delegate void MouseDownEvent(ClickHelper sender, string name, GameObject gameObject);
    public delegate void MouseUpEvent(ClickHelper sender, string name, GameObject gameObject);
    public delegate void ClickEvent(ClickHelper sender, string name, GameObject gameObject);
    public delegate void MouseDownMoveventEvent(ClickHelper sender, Vector3 delta);


    public List<string> ClickObjectNames { get; set; } = new List<string>();
    public MouseDownEvent OnMouseDown = null;
    public MouseUpEvent OnMouseUp = null;
    public ClickEvent OnClick = null;
    public MouseDownMoveventEvent OnMouseDownMove = null;


    private bool isDown = false;
    private DateTime _clickTimeout = DateTime.MinValue;
    private List<GameObject> _selectedObjects;
    private Vector3 _mouseDownPosition;


    void Start()
    {
        _selectedObjects = new List<GameObject>();        
    }


    void Update()
    {
        if (Input.GetMouseButton(0))
        { 
            HandleMouseDown();
        }        
        else if (isDown)
        {
            HandleMouseUp();
        }
    }


    private void HandleMouseDown()
    {
        if (!isDown)
        {
            HandleNewMouseDown();
        }
        else
        {
            HandleExistingMouseDown();
        }
    }


    private void HandleNewMouseDown()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 

        isDown = true;
        _clickTimeout = DateTime.Now.AddMilliseconds(500);

        RaycastHit[] hits = Physics.RaycastAll(ray, 5f);
        foreach(RaycastHit hit in hits)
        {
            if (ClickObjectNames.Contains(hit.transform.gameObject.name))
            {
                _selectedObjects.Add(hit.transform.gameObject);
                OnMouseDown?.Invoke(this, hit.transform.gameObject.name, hit.transform.gameObject);
                _mouseDownPosition = hit.point;
            }
        }
    }


    private void HandleExistingMouseDown()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 5f))
        {
            Vector3 delta = _mouseDownPosition - hit.point;
            if (delta != Vector3.zero)
            {
                OnMouseDownMove?.Invoke(this, delta);
            }
        }
    }


    private void HandleMouseUp()
    {
        foreach (GameObject gameObject in _selectedObjects.ToArray())
        {
            if (_clickTimeout > DateTime.Now)
            {
                OnClick?.Invoke(this, gameObject.name, gameObject);
            }
            OnMouseUp?.Invoke(this, gameObject.name, gameObject);

            _selectedObjects.Remove(gameObject);
        }

        isDown = false;
    }


}
