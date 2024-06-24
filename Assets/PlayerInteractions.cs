using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    //Unfortunatly, Unity's CharacterController collision system is quite bad. I need to implement a workaround
    HashSet<ICharacterCollidable> _activeCollisions;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        ICharacterCollidable[] collidables = hit.gameObject.GetComponents<ICharacterCollidable>();
        foreach (var collidable in collidables)
        {
            if (!_activeCollisions.Contains(collidable))
            {
                collidable.OnCharacterCollisionEnter(hit);
                _activeCollisions.Add(collidable);
            }
            _activeCollisions.IntersectWith(collidables);
        }
    }
}
