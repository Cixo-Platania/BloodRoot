using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPresonCamera : MonoBehaviour
{
    // creo un float che chiamo rotation speed e lo inizializzo a 1
    public float RotationSpeed = 1f;
    // creo due transorm che mi serviranno come riferimento del target(dove guardare) e del player(chi seguire)
    public Transform Target, Player;
    //creo due float che chiamo mouseX e mouseY
    float mouseX, mouseY;
    //creo una variabile transform che chiamo obstructor
    public Transform Obstruction;
 
    void Start()
    {
        //assegno targer a obstructor
        Obstruction = Target;
        //rendo il cursore invisibile
        Cursor.visible = false;
        //blocco il cursore
        Cursor.lockState = CursorLockMode.Locked;
    }
    void LateUpdate()
    {
        //chiamo la funzione camControll
        camControll();
       
    }

    //creo una funzione void che chiamo camControll
    void camControll()
    {
        //sommo a mouseX l'asse x del mouse per la rotation speed
        mouseX += Input.GetAxis("Mouse X") * RotationSpeed;
        //diminuisto a mouseY l'asse y del mouse per la rotation speed
        mouseY -= Input.GetAxis("Mouse Y") * RotationSpeed;
        //richiamo la classe Mathf e utilizzo la funzione clamp(che bloccherà le assi delle y fra due valori) in questo caso -35 e 60
        mouseY = Mathf.Clamp(mouseY, -35, 60);
        //utilizzo la funzione lookAt e gli passo il target
        transform.LookAt(Target);
        // imposto il target. rotation a Quaternion.Euler passandogli i valorri mouseY, mouseX, 0
        Target.rotation = Quaternion.Euler(mouseY, mouseX, 0);
        // imposto il player.rotation a Quaternion.Euler passandogli i valorri 0, mouseX, 0
        Player.rotation = Quaternion.Euler(0, mouseX, 0);
        
    }


}
