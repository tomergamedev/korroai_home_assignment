using UnityEngine;

public interface ICharacterCollidable
{
    void OnCharacterCollisionEnter(ControllerColliderHit hit);
}
