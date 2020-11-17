using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectable 
{
    void Interact();
    void TakeDamage(float damage);
    void OnDestruct();
    Vector3 GetPosition();
}
