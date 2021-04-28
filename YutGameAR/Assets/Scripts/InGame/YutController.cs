using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YutController : MonoBehaviour
{
    
    #region public & private instance variables / Init method
    
    public int yid;
    public int result;

    private YutManager _yutMgr;
    private Rigidbody _rigidbody;
    private Vector3 _initPos;
    private bool _onBump;
    
    private void Init()
    {
        _yutMgr = GetComponentInParent<YutManager>();
        _rigidbody = GetComponent<Rigidbody>();
        _initPos = transform.position;
    }
    
    #endregion
    
    
    
    #region Unity Event functions
    void Start()
    {
        Init();
    }
    void Update()
    {
        if (_onBump)
        {
            if (_rigidbody.velocity.Equals(Vector3.zero) && _rigidbody.angularVelocity.Equals(Vector3.zero))
            {
                float zAngle = transform.rotation.eulerAngles.z;
                result = (zAngle >= 0 && zAngle <= 90) || (zAngle >= 270 && zAngle <= 360) ? 0 : 1;     // 0 = front, 1 = back
                _yutMgr.resultQueue.Enqueue(result);
                _onBump = false;
            }
        }
    }
    
    #endregion

    
    
    #region Yut controller methods
    public void Reset()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.useGravity = false;

        transform.position = _initPos;
        transform.eulerAngles = Vector3.zero;
        result = -1;
        _onBump = false;
    }

    public void Throw(float xTorque, float yTorque, float zTorque, float yForce, ForceMode fMode)
    {
        Reset();
        _rigidbody.useGravity = true;
        _rigidbody.AddForce(0, yForce, 0, fMode);
        _rigidbody.AddTorque(xTorque, yTorque,zTorque, fMode);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name.Equals("YutPlate"))
        {
            _onBump = true;
        }
    }
    
    #endregion
}
