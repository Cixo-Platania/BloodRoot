using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    //creo un riferimento dalla classe BattleStateMachine (vedi lo script BattleStateMachine per capire meglio)
    private BatleStateMachine Bsm;
    // creo un riferimento alla classe baseEnemy (vedi lo script baseEnemy per capire meglio)
    public baseEnemy enemy;
    // creo un game object che mi farà vedere il nemico selezionato 
    public GameObject Selector;
    //creo un riferimento alla classe HealtBar e lo chiamo healtBar
    public HealthBar healtBar;
    //creo un riferimento all'animation
    public Animator anim;
    //creo un float e lo chiamo timeAnimation
    public float TimeAnimation;
    //creo un enumerator che avrà al suo interno tutti gli stati possibili del nostro nemico in battaglia
    public enum TurnState
    {
        //caricamento della barra fittizia
        Processing,
        //scelgo la mossa randomicamente
        ChoseAction,
        //aspetto
        Waiting,
        //eseguo l'azione
        Action,
        //muoio
        Dead

    }
    //creo un riferimento al mio enumerator
    public TurnState CurrentState;
    //creo un float che farà muovere la mia progres bar fittizia
    private float curCoolDown = 0f;
    // creo un float che stabilirà il tempo massimo di attesa della mia progress bar fittizia
    private float maxCoolDown = 10f;
    //creo un vector tre che controlla la posizione iniziale dell' giocatore (per farlo tornare in posizione dopo l'attacco)
    private Vector3 startPosition;
    // creo un bool che controlla quando inizia l'azione che inizializzo a false
    private bool actionIsStarted = false;
    //creo un riferiemnto al giocatore da attaccare
    public GameObject heroToAttack;
    // creo un float che mi servirà a decidere quando deve andare veloce il nemico
    private float animSpeed = 15f;
    //creo un bool che controllerà quando l'eroe è vivo e lo inizializzo a true
    private bool isAlive = true;
    // Start is called before the first frame update
    void Start()
    {
        //assegno tramite la funzione getComponent l'animetor all'interno del gameObject ad anim
        anim = GetComponent<Animator>();
        // inposto lo stato iniziale a processing
        CurrentState = TurnState.Processing;
        //assegno al battle manager un GameObject di nome BatleManager tramite la funzione find e tramite la funzuione get component
        // cerco in batleStateMachine che ho creato in gioco 
        Bsm = GameObject.Find("BatleManager").GetComponent<BatleStateMachine>();
        //assegno a start position la posizione dell nemico
        startPosition = transform.position;
        // inposto il selector a false
        Selector.SetActive(false);
        //inposto la setMaxHealt della HealtBar del nemico
        healtBar.setMaxHealth(enemy.currHP);

    }

    // Update is called once per frame
    void Update()
    {
        // creo uno switch che mi permette di cambiare fra i vari turnState
        switch (CurrentState)
        {
            //creo il primo caso
            case (TurnState.Processing):
                //dentro processing chiamo la funzione UpgradeProgressBar
                UpgradeProgressBar();
                //creo un break per fermare il caso 1
                break;

            //creo il secondo caso 
            case (TurnState.ChoseAction):
                //chiamo la funzione ChoseAction
                choseAction();
                //inposto lo stato a waiting
                CurrentState = TurnState.Waiting;
                break;

            case (TurnState.Waiting):
                

                break;

            

            case (TurnState.Action):
                //faccio iniziare la couroutine e inserisco l'enumerator TimeForAction 
                StartCoroutine(TimeForAction());
                break;

            case (TurnState.Dead):
                //controllo se non è isAlive
                if (!isAlive)
                {
                    return;
                }
                // se il personaggio è morto 
                else
                {
                    //cambio il tag
                    this.gameObject.tag = "DeadEnemy";
                    //elimino il gameObject dalla lista EnemyInGame cosi da non farlo attaccare dai nemici 
                    Bsm.EnemyInGame.Remove(this.gameObject);
                    //disabilito il selectorn 
                    Selector.SetActive(false);
                    //creo un if che controlla se gli EnemyInGame sono maggiori di 0
                    if (Bsm.EnemyInGame.Count>0)
                    {
                        //se lo sono rimuovioamo gli ogetti dalla performList
                        for (int i = 0; i < Bsm.performList.Count; i++)
                        {
                            //controllo se i è diverso da 0
                            if (i!=0)
                            {
                                //controlliamo se in questo momento stiamo attaccando con quel personaggio
                                if (Bsm.performList[i].AttacksGameObject == this.gameObject)
                                {
                                    //rimuovo il personaggio dalla Perform list così attaccheremo con l'altro 
                                    Bsm.performList.Remove(Bsm.performList[i]);
                                }
                                // controlle se il nostro nemico appena morto era stato selezionato come target di un attacco
                                if (Bsm.performList[i].AttacksTarget == this.gameObject)
                                {
                                    //reindirizzo il nemico ad un altro giocatore a caso
                                    Bsm.performList[i].AttacksTarget = Bsm.EnemyInGame[Random.Range(0, Bsm.EnemyInGame.Count)];

                                }
                            }
                          
                        }

                    }
           
                    //cambio colere del nemico 
                    this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(105, 105, 105, 255);
                    //imposto isAlive a false
                    isAlive = false;
                    //resetto i buttons dei nemici 
                    Bsm.EnemyButtons();
                    //controllo se la partita è vinta o persa
                    Bsm.BattleState = BatleStateMachine.PerformAction.CheckAlive;
                }
                break;

        }

    }

    // creo una funzione che chiamo UpgradeProgressBar
    void UpgradeProgressBar()
    {
        //incremento curCoolDown con il passare dei secondi 
        curCoolDown = curCoolDown + Time.deltaTime;
        //controllo se il coolDown ha raggiunto il suo limite
        if (curCoolDown >= maxCoolDown)
        {
            //cambio il current state in ChoseAction
            CurrentState = TurnState.ChoseAction;
        }

    }
    // creo una funzione void che chiamo choseAction
    void choseAction()
    {
        //assegno un nuovo HandleTurn a myAttack
        HandleTurn myAttack = new HandleTurn();
        //imposto il nome dell'attaccante come il nome del nemico
        myAttack.Attacker = enemy.Thename;
        // cambio il type di myAttack a enemy
        myAttack.Type = "Enemy";
        //assegno questo GameObject a AttacksGameObject
        myAttack.AttacksGameObject = this.gameObject;
        // randomizzo il player da attaccare
        myAttack.AttacksTarget = Bsm.HerosInGame[Random.Range(0, Bsm.HerosInGame.Count)];
        //randomizzo il tipo di attacco dalla lista
        int num = Random.Range(0, enemy.attacks.Count);
        //assengo l'attacco appena randomizzato a chosenAttack
        myAttack.chosenAttack = enemy.attacks[num];
        // faccio un debug.log che controlla quale attacco e stato eseguito 
        Debug.Log(this.gameObject.name + " has chosen " + myAttack.chosenAttack.attackName +
            " and do "+ myAttack.chosenAttack.attackDamage + " damage");
        Bsm.collectAction(myAttack);
    }
    //creo un enumerator privato e lo chiamo TimeForAction 
    private IEnumerator TimeForAction()
    {
        //controlle se l'azione è iniziata
        if (actionIsStarted)
        {
            //se lo è fermo tutto
            yield break;
        }
        //imposto actionIsStarted a true
        actionIsStarted = true;
        // imposto l'obbiettivo del Nemico sul Player da attaccare
        Vector3 heroPosition = new Vector3 (heroToAttack.transform.position.x -10f, heroToAttack.transform.position.y, heroToAttack.transform.position.z-5f);
        //imposto la boleana SeePlayer a true
        anim.SetBool("SeePlayer", true);
        //mi muovo verso il nemico
        while (MoveTowardEnemy(heroPosition)) {yield return null;}
        //imposto la boleana SeePlayer a false
        anim.SetBool("SeePlayer", false);
        //imposto la boleana IsAttack a true
        anim.SetBool("IsAttack", true);
        //aspetto un poco 
        yield return new WaitForSeconds(TimeAnimation);
        // faccio danno 
        DoDamage();
        //imposto la boleana IsAttack a true
        anim.SetBool("IsAttack", false);
        // setto la posizione del nemico alla posizione originale
        Vector3 firtPosition = startPosition;
        //imposto la boleana SeePlayer a true
        anim.SetBool("SeePlayer", true);
        // mi muovo verso la posizione originale
        while (MoveTowardStart(firtPosition)) { yield return null; }
        //imposto la boleana SeePlayer a false
        anim.SetBool("SeePlayer", false);
        // rimuovo questo performer dalla lista BSM
        Bsm.performList.RemoveAt(0);
        //assegno il il PerformActionWait a Bsm.BattleState
        Bsm.BattleState = BatleStateMachine.PerformAction.Wait;
        //finisco la coroutine
        actionIsStarted = false;
        //resetto lo stato del nemico 
        curCoolDown = 0f;
        //cambio il currentState a Processing
        CurrentState = TurnState.Processing;
    }

    //creo una funzione di tipo bool che chiederà un vector 3 e la chiamo MoveTowardEnemy
    private bool MoveTowardEnemy(Vector3 goal)
    {
        //ritorno goal che imposto diverso da transform.posittion che imposto uguale ad il vector3 moveTowards
        //a cui passo i seguenti valori transform.position, goal, animSpeed e moltiprico anim speed per Time.deltaTime
        return goal != (transform.position = Vector3.MoveTowards(transform.position,goal, animSpeed * Time.deltaTime));
    }

    //creo una funzione di tipo bool che chiederà un vector 3 e la chiamo MoveTowardStart
    private bool MoveTowardStart(Vector3 goal)
    {
        //ritorno goal che imposto diverso da transform.posittion che imposto uguale ad il vector3 moveTowards
        //a cui passo i seguenti valori transform.position, goal, animSpeed e moltiprico anim speed per Time.deltaTime
        return goal != (transform.position = Vector3.MoveTowards(transform.position, goal, animSpeed * Time.deltaTime));
    }

    //creo una funzioe che chiamo DoDamage
    void DoDamage() 
    {
        //creo un int che chuiamo Calc_damage e lo inizializzo all'attacco del player + l'attacco scelto
        float calc_damage = enemy.currATK + Bsm.performList[0].chosenAttack.attackDamage;
        //cerco la funzione TakeDamage di HeroStateM tramite la funzione getComponent e passo il calc_damage
        heroToAttack.GetComponent<HeroStateM>().TakeDamage(calc_damage);
    }

    //creo una variabile void che richiedera un float e la chiamo TakeDamage
    public void TakeDamage(int getDamageAmount)
    {
        //diminuisco la hero.curHp con il damageAmmaunt
        enemy.currHP -= getDamageAmount;
        //faccio avvenire il suono EnemyHit
        FindObjectOfType<AudioManagerù>().Play("EnemyHit");
        //creo un if che controlla quando la curHp scende sotto lo 0
        if (enemy.currHP<=0)
        {
            //imposto la curHp a zero
            enemy.currHP = 0;
            // cambio in current state a TurnState.Dead
            CurrentState = TurnState.Dead;
            // attivo il trigger IsDead
            anim.SetTrigger("IsDead");
        }
        //chiamo la funzione SetHealth i gli passo l'enemy.curHp
        healtBar.SetHealth(enemy.currHP);
    }
}
