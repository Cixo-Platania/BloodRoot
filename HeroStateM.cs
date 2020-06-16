using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class HeroStateM : MonoBehaviour
{
    //creo un riferimento dalla classe BattleStateMachine (vedi lo script BattleStateMachine per capire meglio)
    private BatleStateMachine Bsm;
    // creo un riferimento alla classe baseHero (vedi lo script baseHero per capire meglio)
    public baseHero hero;
    //creo un float e lo chiamo timeOfAnimation
    public float timeOfAnimation;
    //creo un riferimento all'animation
    public Animator anim;
    //creo un enumerator che avrà al suo interno tutti gli stati possibili del nostro eroe in battaglia
    public enum TurnState
    {
        //caricamento della barra
        Processing,
        //aggiungo tutti i giocatori alla lista HeroToManage
        AddToList,
        //aspetto
        Waiting,
        //seleziono l'azione
        Selecting,
        //faccio l'azione
        Action,
        //muoio 
        Dead

    }
    //creo un float che mi farà variare la velocità di caricamento della ProcesBar dei vari eroi 
    public float agility = 0.3f;
    //creo un riferimento al mio enumerator
    public TurnState CurrentState;
    //creo un float che farà muovere la mia progres bar
    private float curCoolDown = 0f;
    // creo un float che stabilirà il tempo massimo di attesa della mia progress bar
    private float maxCoolDown = 5f;
    //creo una immagine che conterrà la mia progress bar
    private Image ProgressBar;
    // creo un game object che mi farà vedere il giocatore selezionato 
    public GameObject Selector;
    //IeNumerator
    //creo un riferiemnto al nemico da attaccare
    public GameObject enemyToAttack;
    // creo un bool che controlla quando inizia l'azione che inizializzo a false
    private bool actionIsStarted = false;
    //creo un vector tre che controlla la posizione iniziale dell' giocatore (per farlo tornare in posizione dopo l'attacco)
    private Vector3 startPosition;
    // creo un float che mi servirà a decidere quando deve andare veloce il mio eroe
    private float animSpeed = 15f;
    //creo un bool che controllerà quando l'eroe è vivo e lo inizializzo a true
    private bool IsAlive = true;
    //creo un riferimento alla classe heroPannelStats (vedi lo script heroPannelStats per capire meglio)
    private HeroPanleStats stats;
    //creo un game object che contiene tutte le scritte e bottoni riguardanti l'eroe e lo chiamo hero panel
    public GameObject heroPanel;
    //creo un transform che mi permettera di decidere dove posizionare gli hero panel
    public Transform HeroPanleSpacer;
   
    // Start is called before the first frame update
    void Start()
    {
        //assegno tramite la funzione getComponent l'animetor all'interno del gameObject ad anim
        anim = GetComponent<Animator>();
        //creo un pannello eroe e lo riempio di informazioni (per maggiori informazioni vedi la funzione crete hero panel)
        createHeroPanel();
        //assegno a start position la posizione dell player
        startPosition = transform.position;
        // tolgo dal limite massimo la agiliti 
        maxCoolDown -= agility;
        // inposto lo stato iniziale a processing
        CurrentState = TurnState.Processing;
        //assegno al battle manager un GameObject di nome BatleManager tramite la funzione find e tramite la funzuione get component
        // cerco in batleStateMachine che ho creato in gioco 
        Bsm = GameObject.Find("BatleManager").GetComponent<BatleStateMachine>();
        // inposto il selector a false
        Selector.SetActive(false);
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
            case (TurnState.AddToList):
                //aggiungo il gameObject a cui è assegnato lo script a heroToManage della classe BatleStateMachine
                Bsm.HeroToManage.Add(this.gameObject);
                //inposto lo stato a waiting
                CurrentState = TurnState.Waiting;
                break;

            case (TurnState.Waiting):
                //idle
                break;

            case (TurnState.Action):
                //faccio iniziare la couroutine e inserisco l'enumerator TimeForAction 
                StartCoroutine(TimeForAction());
                break;

            case (TurnState.Dead):
                //controllo se non è isAlive
                if (!IsAlive)
                {
                    return;
                }
                // se il personaggio è morto 
                else
                {
                    //cambia tag 
                    this.gameObject.tag = "DeadHero";
                    //elimino il gameObject dalla lista herosInGame cosi da non farlo attaccare dai nemici 
                    Bsm.HerosInGame.Remove(this.gameObject);
                    //elimino il gameObject dalla lista HeroToManage cosi da non poterlo più utilizzare
                    Bsm.HeroToManage.Remove(this.gameObject);
                    // disativazione del selector
                    Selector.SetActive(false);
                    //resetto hud inpostando l'attacPanel a false e il selector a false 
                    Bsm.AttackPanel.SetActive(false);
                    Bsm.EnemySelectPanel.SetActive(false);
                    //creo un if che controlla se gli HerosInGame sono maggiori di 0
                    if (Bsm.HerosInGame.Count > 0)
                    {
                        //se lo sono rimuovioamo gli ogetti dalla performList
                        for (int i = 0; i < Bsm.performList.Count; i++)
                        {
                            //controllo se i è diverso da 0
                            if (i != 0)
                            {
                                //controlliamo se in questo momento stiamo attaccando con quel personaggio
                                if (Bsm.performList[i].AttacksGameObject == this.gameObject)
                                {
                                    //rimuovo il personaggio dalla Perform list così attaccheremo con l'altro 
                                    Bsm.performList.Remove(Bsm.performList[i]);
                                }
                                // controlle se il nostro personaggio appena morto era stato selezionato come target di un attacco
                                if (Bsm.performList[i].AttacksTarget == this.gameObject)
                                {
                                    //reindirizzo il nemico ad un altro giocatore a caso 
                                    Bsm.performList[i].AttacksTarget = Bsm.HerosInGame[Random.Range(0, Bsm.HerosInGame.Count)];
                                }
                            }

                        }
                    }

                    //attiva animazione di morte
                    this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(105, 105, 105, 255);
                    //reste heroInput
                    Bsm.BattleState = BatleStateMachine.PerformAction.CheckAlive;
                    //inposto isAlive a false
                    IsAlive = false;

                }
                //finisco il caso con un break 
                break;

        }
        
    }

    // creo una funzione che chiamo UpgradeProgressBar
    void UpgradeProgressBar()
    {
        //incremento curCoolDown con il passare dei secondi  
        curCoolDown = curCoolDown + Time.deltaTime;
        //creo un float che chiamo calcCoolDown e lo inizializzo come la seguente frazione: curCoolDown / maxCoolDown
        float calcCoolDown = curCoolDown / maxCoolDown;
        //cambio la local scale della progressbar e la imposto ad un nuovo vector 3 che sra uguale nell asse x a la funzione clamp di mathf 
        //che avra come valori calcCoolDown, 0, 1 e nelle assi y e z sarà uguale a alla local scale della ProgresBar
        ProgressBar.transform.localScale = new Vector3(Mathf.Clamp(calcCoolDown, 0, 1), ProgressBar.transform.localScale.y, ProgressBar.transform.localScale.z);
        //controllo se il coolDown ha raggiunto il suo limite
        if (curCoolDown>=maxCoolDown)
        {
            //cambio il current state in addToList
            CurrentState = TurnState.AddToList;
        }

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
        // imposto l'obbiettivo del Player sul Nemico da attaccare
        Vector3 EnemyPosition = new Vector3(enemyToAttack.transform.position.x + 10.5f, enemyToAttack.transform.position.y, enemyToAttack.transform.position.z+5f);
        //imposto la boleana IsWalking a true
        anim.SetBool("IsWalking", true);
        //mi muovo verso il nemico 
        while (MoveTowardEnemy(EnemyPosition)) { yield return null; }
        //imposto la boleana IsWalking a false
        anim.SetBool("IsWalking", false);
        // attivo il trigger IsAttacking
        anim.SetTrigger("IsAttacking");
        //aspetto un poco 
        yield return new WaitForSeconds(timeOfAnimation);
        // faccio danno 
        DoDamage();
        // setto la posizioned del player alla posizione originale
        Vector3 firtPosition = startPosition;
        //imposto la boleana IsWalkingBack a true
        anim.SetBool("IsWakkingBack", true);
        // mi muovo verso la posizione originale
        while (MoveTowardStart(firtPosition)) { yield return null; }
        //imposto la boleana IsWalkingBack a false
        anim.SetBool("IsWakkingBack", false);
        // rimuovo questo performer dalla lista BSM
        Bsm.performList.RemoveAt(0);
        // controllo quando il bsm è diverso sia dalla perform action win che dalla perform action lose
        if (Bsm.BattleState != BatleStateMachine.PerformAction.Win&&Bsm.BattleState!=BatleStateMachine.PerformAction.Lose)
        {
            //resetto BSM in wait
            Bsm.BattleState = BatleStateMachine.PerformAction.Wait;
            //resetto lo stato del nemico 
            curCoolDown = 0f;
            //cambio il currentState a Processing
            CurrentState = TurnState.Processing;
        }
        else
        {
            // cambio il current state a waiting
            CurrentState = TurnState.Waiting;
        }
      
        //finisco la coroutine
        actionIsStarted = false;
    }
    //creo una funzione di tipo bool che chiederà un vector 3 e la chiamo MoveTowardEnemy
    private bool MoveTowardEnemy(Vector3 goal)
    {
        //ritorno goal che imposto diverso da transform.posittion che imposto uguale ad il vector3 moveTowards
        //a cui passo i seguenti valori transform.position, goal, animSpeed e moltiprico anim speed per Time.deltaTime
        return goal != (transform.position = Vector3.MoveTowards(transform.position, goal, animSpeed * Time.deltaTime));
    }
    //creo una funzione di tipo bool che chiederà un vector 3 e la chiamo MoveTowardStart
    private bool MoveTowardStart(Vector3 goal)
    {
        //ritorno goal che imposto diverso da transform.posittion che imposto uguale ad il vector3 moveTowards
        //a cui passo i seguenti valori transform.position, goal, animSpeed e moltiprico anim speed per Time.deltaTime
        return goal != (transform.position = Vector3.MoveTowards(transform.position, goal, animSpeed * Time.deltaTime));
    }
    //creo una variabile void che richiedera un float e la chiamo TakeDamage
    public void TakeDamage(float damageAmount)
    {
        //diminuisco la hero.curHp con il damageAmmaunt
        hero.currHP -= damageAmount;
        //faccio avvenire il suono PlayerHit
        FindObjectOfType<AudioManagerù>().Play("PlayerHit");
        //creo un if che controlla quando la curHp scende sotto lo 0 
        if (hero.currHP <= 0)
        {
            //imposto la curHp a zero
            hero.currHP = 0;
            // cambio in current state a TurnState.Dead
            CurrentState = TurnState.Dead;
            // attivo il trigger IsDead
            anim.SetTrigger("IsDead");
            
        }
        //chiamo la funzione UpdateHeroPanel
        UpdateHeroPanel();

    }
    //creo una funzioe che chiamo DoDamage
    void DoDamage()
    {
        //creo un int che chuiamo Calc_damage e lo inizializzo all'attacco del player + l'attacco scelto
        int calc_damage = hero.currATK + Bsm.performList[0].chosenAttack.attackDamage;
        //cerco la funzione TakeDamage di EnemyStateMachine tramite la funzione getComponent e passo il calc_damage
        enemyToAttack.GetComponent<EnemyStateMachine>().TakeDamage(calc_damage);
    }
    //creo una funzuione che chiamo createHeroPanel
    void createHeroPanel()
    {
        //impsto l'heroPanel ad una instanziazione dell' heroPanel come GameObject
        heroPanel = Instantiate(heroPanel) as GameObject;
        //tramite la funzione GetComponent cenrco l'HeroPanelStats dentro hero panel e lo essegno a stats
        stats = heroPanel.GetComponent<HeroPanleStats>();
        //assegno il nome del giocatore al suo rispettivo Pannello 
        stats.HeroName.text = hero.Thename;
        //assegno gli Hp ed i maxHp del giocatore al suo rispettivo Pannello 
        stats.HeroHp.text = "HP: " + hero.currHP + "/" + hero.baseHP;
        //assegno gli Mp ed i MaxMp del giocatore al suo rispettivo Pannello 
        stats.HeroMp.text = "MP: " + hero.currMP + "/" + hero.baseMP;
        //assegno stats.progressBar alla progressBar
        ProgressBar = stats.ProgressBar;
        //uso la funzione setParent e passo al suo interno HeroPanelSpacer e false
        heroPanel.transform.SetParent(HeroPanleSpacer, false);
    }
    // creo una funzuone che chiamo UpdateHeroPanel
    public void UpdateHeroPanel()
    {
        //cambio gli Hp ed i maxHp del giocatore al suo rispettivo Pannello 
        stats.HeroHp.text = "HP: " + hero.currHP + "/" + hero.baseHP;
        //cambio gli Mp ed i MaxMp del giocatore al suo rispettivo Pannello 
        stats.HeroMp.text = "MP: " + hero.currMP + "/" + hero.baseMP;

    }
  
}
