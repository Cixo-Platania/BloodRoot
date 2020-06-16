using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapPlayer : MonoBehaviour
{
    // creo una variabile transform che mi servirà da riferimento al player
    public Transform Player;
    private void LateUpdate()
    {
        //creo un nuovo vector3 che sarà uguale alla player.position
        Vector3 newPosition = Player.position;
        //imposto il valore del nuovo vector3(solo dell'asse y) al transform.position.y
        newPosition.y = transform.position.y;
        //imposto la transform.positon del gameObject alla newPosition
        transform.position = newPosition;
    }
}
