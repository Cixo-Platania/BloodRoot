using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public  class EnemyController : MonoBehaviour
{
    //creo un float che mi servià a delimitare il range massimo del nemico 
    public float lookRadius = 10f;
    //creo un transform e lo chiamo target
    Transform target;
    // creo una navmesh
    NavMeshAgent agent;
    //creo un animator
    public Animator anim;
    // creo un bool che controlla quando il nemico è attivo o no 
    public static bool isActive = true;
    
    // Start is called before the first frame update
    void Start()
    {
        // cerco dentro il mio game Object una componente di tipo animator e la assegno calla componente  anim
        anim = GetComponent<Animator>();
       
        gameObject.SetActive(isActive);
        //cerco dentro il payer manager la transform del player e la imposto gome targer
        target = PlayerManager.instance.player.transform;
        //cerco dentro il mio game Object una componente di tipo navMes e la assegno alla compopnente agent
        agent = GetComponent<NavMeshAgent>();

        
    }

    // Update is called once per frame
    void Update()
    {
        // creo un float distance che inzializzo come una distanza fra il targer e la posizione del gameObject
        float distance = Vector3.Distance(target.position, transform.position);

        //se la distanza è minore del lookRadius 
        if (distance<=lookRadius)
        {
            //imposto la destinazione del nemico al giocatore
            agent.SetDestination(target.position);
            // imposto l'animazione di camminata
            anim.SetBool("SeePlayer", true);
            //se la distanza e maggiore della stopping distance della navmesh
            if (distance <= agent.stoppingDistance)
            {
                //disattivo l'animazione di camminata
                anim.SetBool("SeePlayer", false);
                //chiamo la funzione face target
                FaceTarget();
            }
           
        }
       
    }
    //creo un void on collision Enter che contollerà quando il giocatore collide con qualcosa
    private void OnCollisionEnter(Collision collision)
    {
        // se collide con i player
        if (collision.gameObject.tag == "Player")
        {
            // imposto lo stato di battaglia nel game manager
            GameManager.instace.gameState = GameManager.GameState.BattleState;
        }
    }

    //creo una funzione che mi permettera di guardare il player senza far muovere il nemico 
    void FaceTarget()
    {
        //imposto la direzione al target positon meno la posizione del nemico 
        Vector3 direction = (target.position - transform.position).normalized;
        //creo una variabile quaternion e la chiamo lookrotation, la inizializzo a la rotazione del player ma solo nelle 
        //assi x e z per non farlo ruotare verso l'alto 
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        //imposto la rotazione del nemico alla look rotation con uno slerp che mi farà ritardare il tutto di 5 secondi 
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

    }
    private void OnDrawGizmosSelected()
    {
        // imposto il colore in rosso
        Gizmos.color = Color.red;
        //e imposto la posizione del cerchio al centro del nemico e il raggio come lookRadius
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
