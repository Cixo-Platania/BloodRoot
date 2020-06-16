using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonPlayer : MonoBehaviour
{
    // creo un float publica e la chiamo speed
    public float speed;
    // creo due vector 3 che salveranno in memoria la posizione corrente 
    //del giocatore e l'ultima posizione prima di entrare in battaglia
    public Vector3 curPos, lastPos;
    //creo un animator e lo chiamo anim 
    Animator anim;
    // creo un float e lo chiamo gravity e lo inizializzo a -9.81(gravità della terra)
    public float gravity = -9.81f;
    //creo un riferimento al rigid body e lo chiamo rb
    public Rigidbody rb;
    
    // Start is called before the first frame update
    void Start()
    {
        // cerco dentro il mio game Object una componente di tipo RigidBody e la assegno alla componente tìrb
        rb = GetComponent<Rigidbody>();
        // cerco dentro il mio game Object una componente di tipo animator e la assegno alla componente  anim
        anim = GetComponent<Animator> ();
        //blocco il cursore in una posizione
        Cursor.lockState = CursorLockMode.Locked;
        // rendo invisibile i cursore
        Cursor.visible = false;
        //controllo e la lastHeroPosition del gameManager e diversa dal vector3.zero
        if (GameManager.instace.LastHeroPosition!=Vector3.zero)
        {
            //imposto la posizione del player alla last hero position del gameManager
            transform.position = GameManager.instace.LastHeroPosition;
            //imposto la last hero positon al vector3.zero
            GameManager.instace.LastHeroPosition = Vector3.zero;

        }
    }
    
    // Update is called once per frame
    void Update()
    {
        //chiamo la funzione player movment
        PlayerMovment();
        
    }
    private void FixedUpdate()
    {
        //creo un vector 3 che chiamo new position e lo inizializzo alla posizione del giocatore nelle tre assi ma 
        //nella asse y sommo il float gravity che moltiplico nel tempo
        Vector3 newPositon = new Vector3(transform.position.x,transform.position.y+gravity*Time.deltaTime,transform.position.z);
        //chiamo la funzione move position del rigid body e passo la new position come valore
        rb.MovePosition(newPositon);
        //imposto la curent position alla transform.position
        curPos = transform.position;
        //controllo se la current positon e la last positon sono uguali 
        if (curPos==lastPos)
        {
            // se lo sono imposto la boleana is walking del game manager a false
            GameManager.instace.isWalking = false;
            
        }
        else
        {
            //se non lo sono imposto la boleana is walking del game manager a true
            GameManager.instace.isWalking = true;
            
        }
        //controllo quando il giocatore preme W oppure D oppure A
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            //imposto la boleana is walking a true
            anim.SetBool("IsWalking", true);
            //imposto la boleana is walkingBack a false 
            anim.SetBool("IsWakkingBack", false);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            //imposto la boleana is walking a false
            anim.SetBool("IsWalking", false);
            //imposto la boleana is walkingBack a true
            anim.SetBool("IsWakkingBack", true);
        }
        else
        {
            //imposto la boleana is walking a false
            anim.SetBool("IsWakkingBack", false);
            //imposto la boleana is walkingBack a false 
            anim.SetBool("IsWalking", false);
        }
        //controllo quando premo il tasto z
        if (Input.GetKeyDown(KeyCode.Z))
        {
            //attivo il trigger chiamatto isAttaking
            anim.SetTrigger("IsAttacking");
        }
        //imposto la last pos alla cur pos
        lastPos = curPos;

    }
    void PlayerMovment()
    {
        //creo due float che conterrano le assi verticali e orizontali 
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");
        //creo un vector 3 chiamato player movment e lo inizializzo ad un nuovo vector3 a cui passo le due variabili create sopra
        // per le azzi x e z e per l'asse y passo 0 e moltiplico tutto per speed e per time.deltaTime
        Vector3 playerMovment = new Vector3(hor, 0, ver) * speed * Time.deltaTime;
        //richiamo la funzione translate del nostro game object a cui passo due valori uno player movment e l'altro space.self
        transform.Translate(playerMovment, Space.Self);
    }
}
