using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BatleStateMachine : MonoBehaviour
{
    //creo una Boleanae la chiamo isBoss e la inizializzo a falsa
    public bool isBoss = false;
    //creo un gameObject che servirà per controllare quando siamo stati curati o no 
    public GameObject testoCura;
    //creo un enumerator che conterra tutte le fasi di gioco 
    public enum PerformAction
    {
        //aspetta
        Wait,
        //pendo il comando 
        TakeAction,
        //faccio avvenire l'azione
        PerformAction,
        // controllo chi è vivo 
        CheckAlive,
        //condizione di vittoria
        Win,
        //condizione di sconfitta
        Lose
    }
    //creo un riferimento al mio enumerator
    public PerformAction BattleState;
    // creo una lista di  HandleTurn (vedi lo script handelTurn per vedere di cosa si tratta)
    public List<HandleTurn> performList = new List<HandleTurn>();
    // creo una lista di gameObject che conterrà tutti gli eroi in gioco
    public List<GameObject> HerosInGame = new List<GameObject>();
    // creo una lista di gamenObject che conterra tutti i nemici in gioco 
    public List<GameObject> EnemyInGame = new List<GameObject>();

    //creo un emnu che chiamo HeroGui che conterrà tutte le fasi della Gui dell'eroe
    public enum HeroGUI
    {
        //attivazione
        Activate,
        //attesa
        Waiting,
        //chiamata funzione Input1
        Input1,
        //chiamata funzione Input2
        Input2,
        //fine
        Done
    }
    //creo un riferimento al mio enumerator
    public HeroGUI HeroInput;
    //creo una lista publica di gameObject che chiamo HeroToManage e la inizializzo ad una nuova lista di GameObject
    public List<GameObject> HeroToManage = new List<GameObject>();
    // creo un private HndleTurn che chiamo HeroChoiche
    private HandleTurn HeroChoice;
    //creo un riferimento all'enemyButton
    public GameObject enemyButton;
    //Creo un riferimento all'enemyButton Spacer di tipo transofrm 
    public Transform Spacer;
    //creo un riferimento all'AttackPanel
    public GameObject AttackPanel;
    //creo un riferimento all'EnemySelectPanel
    public GameObject EnemySelectPanel;
    //creo un riferimento all'MagicsPanel
    public GameObject MagicsPanel;

    //attachi del player
    //Creo un riferimento all'actionSpacer di tipo transofrm 
    public Transform actionSpacer;
    //Creo un riferimento all'magicSpacer di tipo transofrm
    public Transform magicSpacer;
    //creo un riferimento all'actionButton
    public GameObject actionButton;
    //creo un riferimento ai MagicButtons
    public GameObject MagicButtons;
    //creo una lista di GameObject che chiamo AtkBtns (quindi tutti gli attakButton misti fra magic e action)
    private List<GameObject> AtkBtns = new List<GameObject>();

    //creo una lista di GameObject che chiamo AtkBtns
    private List<GameObject> EnemyBtns = new List<GameObject>();
    //creo una lista di transform che chiamo spawnPoint
    public List<Transform> SpawnPoints = new List<Transform>();
    //creo un gameObject che chiamo Testo 
    public GameObject testo;
    //creo una bool che chiamo is active e inizializzo a false 
    public bool isactive = false;
    // creo un float che chiamo timer
    public float timer;
    //creo un float che chiamo startTimer e inizializzo a 0.5f
    public float startTimer = 0.5f;
    void Awake()
    {
           //per ogni enemyAmmounti in GameManager.instace
            for (int i = 0; i < GameManager.instace.enemyAmmount; i++)
            {
                // creo un GameObject che chiamo newEnemy e lo instanzio con questi tre valori GameManager.instace.EnemysToBattle[i], SpawnPoints[i].position, Quaternion.Euler(0, 60, 0) come game Object
                GameObject NewEnemy = Instantiate(GameManager.instace.EnemysToBattle[i], SpawnPoints[i].position, Quaternion.Euler(0, 60, 0)) as GameObject;
                //assegno il nome  dell'enemy + 1 a newEnemy.name
                NewEnemy.name = NewEnemy.GetComponent<EnemyStateMachine>().enemy.Thename + " " + (i + 1);
                //assegno al Thename in EnemyStateMachine il newEnemy.name
                NewEnemy.GetComponent<EnemyStateMachine>().enemy.Thename = NewEnemy.name;
                // uso la funzione add per aggiungere i new enemy in game 
                EnemyInGame.Add(NewEnemy);
            //controllo se il nuovo enemy ha come nome il nome del boss
            if (NewEnemy==GameObject.Find("Boss Alce 1"))
            {
                //imposto la variabile isBoss a true
                isBoss = true;
            }
            }
    }
    // Start is called before the first frame update
    void Start()
    {
        //uso la funzione set active Per impostare TestoCura a false
        testoCura.SetActive(false);
        // uso la funzione addRange per aggiungere tutti i player alla lista Heros in game 
        HerosInGame.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        //uso la funzione set active Per impostare Testo a false
        testo.SetActive(false);
        //setto il timer a 0
        timer = 0;
        //rendo libero il cursore
        Cursor.lockState = CursorLockMode.None;
        // rendo visibile il cursore
        Cursor.visible = true;
        //imposto il battleState a wait
        BattleState = PerformAction.Wait;
        // imposto L'hero input a activate
        HeroInput = HeroGUI.Activate;
        //uso la funzione set active Per impostare AttackPanel a false
        AttackPanel.SetActive(false);
        //uso la funzione set active Per impostare EnemySelectPanel a false
        EnemySelectPanel.SetActive(false);
        //uso la funzione set active Per impostare MagicsPanel a false
        MagicsPanel.SetActive(false);
        // chiamo la funzione enemyButtons
        EnemyButtons();
        
    }

    // Update is called once per frame
    void Update()
    {
        // controlle se isActive è uguale a true
        if (isactive)
        {
            // sommo il tempo al timer
            timer += Time.deltaTime;
            // controllo se il timer è maggiore di start timer
            if (timer>=startTimer)
            {
                //uso la funzione set active Per impostare testo a false
                testo.SetActive(false);
                //imposto il timer a 0
                timer = 0;
                //imposto isActive a false
                isactive = false;
            }
        }
       // creo uno switch fra battle state
        switch (BattleState)
        {
            //creo il primo caso 
            case (PerformAction.Wait):
                //controllo se la performlist.conunt è maggiore di 0 
                if (performList.Count>0)
                {
                    //cambio lo stato il take action 
                    BattleState = PerformAction.TakeAction;
                }

                break;
                //caso 2
            case (PerformAction.TakeAction):
                //assegno il performer all al primo numero degli attakers della performList
                GameObject performer = GameObject.Find(performList[0].Attacker);
                //controllo se il tipo della perform list è uguale a Enemy
                if (performList[0].Type == "Enemy")
                {
                    //assegno a ESM il lo script EnemyStateMachine del performer
                    EnemyStateMachine ESM = performer.GetComponent<EnemyStateMachine>();
                    //per ogni eroe in gioco 
                    for (int i = 0; i < HerosInGame.Count; i++)
                    {
                        //controllo quando l'eroe in gioco e attacksTarget combaciano 
                        if (performList[0].AttacksTarget==HerosInGame[i])
                        {
                            // assegno a ESM.heroToAttack l'eroe giusto tramite il controllo di prima
                            ESM.heroToAttack = performList[0].AttacksTarget;
                            // cambio il currentState di ESM in action 
                            ESM.CurrentState = EnemyStateMachine.TurnState.Action;
                            break;
                        }
                        else
                        {
                            //randomizzo il player da attaccare
                            performList[0].AttacksTarget = HerosInGame[Random.Range(0, HerosInGame.Count)];
                            // assegno a ESM.heroToAttack l'eroe randomizzato
                            ESM.heroToAttack = performList[0].AttacksTarget;
                            // cambio il currentState di ESM in action 
                            ESM.CurrentState = EnemyStateMachine.TurnState.Action;
                        }
                    }
                }
                // controllo se il tipo della perform list è uguale a Player
                if (performList[0].Type == "Player")
                {
                    //assegno a HSM  lo script HeroStateM del performer
                    HeroStateM HSM = performer.GetComponent<HeroStateM>();
                    // assegno a HSM.enemyToAttack il nemico scelto 
                    HSM.enemyToAttack = performList[0].AttacksTarget;
                    // cambio il currentState di HSM in action 
                    HSM.CurrentState = HeroStateM.TurnState.Action;
                }
                //cambio il BattleState a PerformAction
                BattleState = PerformAction.PerformAction;
                break;
                //caso3
            case (PerformAction.PerformAction):
                //idle
                break;
                //caso4
            case (PerformAction.CheckAlive):
                //se gli eroi in game sono minori di 1
                if (HerosInGame.Count<1)
                {
                    // cambio il battle state a lose
                    BattleState = PerformAction.Lose;
                    //lose the battle
                }
                // se i nemici in gioco sono minori di 1
                else if (EnemyInGame.Count<1)
                {
                    // cambio il battle state a 1
                    BattleState = PerformAction.Win;
                    //win the battle
                   
                }
                else
                {
                    //chiamo la funzione clearAttackPanel
                    clearAttackPanel();
                    //cambio l'HeroInput in activate
                    HeroInput = HeroGUI.Activate;
                }
                break;
                //caso5
            case (PerformAction.Lose):
                {
                    //vado nella scena di gameOver
                    SceneManager.LoadScene("GameOver");
                }
                break;
                //caso6
            case (PerformAction.Win):
                {
                    //per ogni eroe in game 
                    for (int i = 0; i < HerosInGame.Count; i++)
                    {
                        //cambio lo stato in wating
                        HerosInGame[i].GetComponent<HeroStateM>().CurrentState = HeroStateM.TurnState.Waiting;
                    }
                    //attivo il testo cura
                    testoCura.SetActive(true);
                    //sommo il timer al tempo 
                    timer += Time.deltaTime;
                    //controllo se il timer è maggiore dello start timer e se isBoss e uguale a false
                    if (timer >= startTimer && isBoss == false)
                    {
                        //disattivo il testo cura
                        testoCura.SetActive(false);
                        // imposto timer a 0
                        timer = 0;
                        //chiamo la funzione LoadSceneAfterBattle
                        GameManager.instace.LoadSceneAfterBattle();
                        //cambio il game state a WorldState
                        GameManager.instace.gameState = GameManager.GameState.WorldState;
                        //pulisco gli enemyToBattle
                        GameManager.instace.EnemysToBattle.Clear();

                    }
                    //se isBoss e uguale a true
                    else if (isBoss == true) 
                    {
                        //cambio scena in vittoria
                        SceneManager.LoadScene("Win");
                    }

                }
                break;
        }

        // creo uno switch  per l'hero imput 
        switch (HeroInput)
        {
            //caso1
            case (HeroGUI.Activate):
                //se hero to manage è maggiore di 0
                if (HeroToManage.Count>0)
                {
                    //cerco il selecter del heroToManage e lo attivo
                    HeroToManage[0].transform.Find("Selecter").gameObject.SetActive(true);
                    //assengo a HeroChoice un nuovo HandleTurn
                    HeroChoice = new HandleTurn();
                    //attivo l'AttackPanel
                    AttackPanel.SetActive(true);
                    //chiamo la funzione cretataAttackButtons
                    createAttackButtons();
                    //Cambio l'HeroInput a wating
                    HeroInput = HeroGUI.Waiting;
                }
                break;
                //caso3
            case (HeroGUI.Waiting):
                  //idle
                break;
                //caso4
            case (HeroGUI.Done):
                //chaimo la funzione HeroInputDone
                HeroInputDone();
                break;
          

        }
    }
    //creo una  funzione void collectAction che chiederà un HandleTurn
    public void collectAction(HandleTurn input)
    {
        // aggiungo input alla performList
        performList.Add(input);
    }
    //creo una funzione void EnemyButtons 
    public void EnemyButtons()
    {
        //per ogni bottone del nemico nella lista dei bottoni dei nemici  
        foreach (GameObject enemyBtn in EnemyBtns)
        {
            // distrucci il bottore
            Destroy(enemyBtn);
        }
        //pulisco la lista EnemyBtns
        EnemyBtns.Clear();
        //per ogni nemico nella lista EnemyInGame 
        foreach(GameObject enemy in EnemyInGame)
        {
            //instanzio un enemyButton come gameObject dentro newButton
            GameObject newButton = Instantiate(enemyButton) as GameObject;
            //assegno a button lo script EnemySelectButton del newButton
            EnemySelectButton button = newButton.GetComponent<EnemySelectButton>();
            //assegno a curr_nemy lo script EnemyStateMachine di enemy 
            EnemyStateMachine cur_enemy = enemy.GetComponent<EnemyStateMachine>();
            //cerco dentro a button un gameObject chiamato Text e assegno lo srcipt Text di Text a ButtonText
            Text ButtonText = newButton.transform.Find("Text").gameObject.GetComponent<Text>();
            //assegno a ButtonText.text  il nome del nemico corrente
            ButtonText.text = cur_enemy.enemy.Thename;
            //assegno a button.EnemyPrefab il nemuico 
            button.EnemyPrefab = enemy;
            //uso la funzione set parent di new button e passo i parametri spacer a false
            newButton.transform.SetParent(Spacer,false);
            // aggiungo il nuovo bottone alla lista
            EnemyBtns.Add(newButton);
        }
    }
    // creo una funzione void che chiamo input 1
    public void Input1()
    {
        //assegno a HeroChoice.Attacker il nome dell'eroe
        HeroChoice.Attacker = HeroToManage[0].name;
        //assegno a HeroChoice.AttacksGameObject il game Object dell'eroe
        HeroChoice.AttacksGameObject = HeroToManage[0];
        //cambio il tipo di heroChoice a Player
        HeroChoice.Type = "Player";
        //assengo al chosenAttack l'attacco che sta per eseguire l'eroe
        HeroChoice.chosenAttack = HeroToManage[0].GetComponent<HeroStateM>().hero.attacks[0];
        //distattivo l'attack panel
        AttackPanel.SetActive(false);
        //attivo EnemySelectPanel
        EnemySelectPanel.SetActive(true);

    }
    // creo una funzione void  che chiamo input 2  e chiederà un gameObject
    public void Input2(GameObject choseEnemy)
    {
        //imposto l'attack targer al chosenEnemy
        HeroChoice.AttacksTarget = choseEnemy;
        //imposto l'HeroInput a done
        HeroInput = HeroGUI.Done;
    }
    //creo una funzione void che chiamo HeroInputDone
    void HeroInputDone()
    {
        //aggiungo l'HeroChoice alla performList
        performList.Add(HeroChoice);
        // chiamo la funzione clearAttackPanel
        clearAttackPanel();
        //disattivo il selecter dell'eroe che ha eseguito l'azione
        HeroToManage[0].transform.Find("Selecter").gameObject.SetActive(false);
        //rimuovo l'eroe dall'HeroToManage
        HeroToManage.RemoveAt(0);
        //inposto l'heroInput ad activate
        HeroInput = HeroGUI.Activate;
    }
    //creo la funzione chiamata clearAttackPanel
    void clearAttackPanel()
    {
        //disattivo EnemySelectPanel
        EnemySelectPanel.SetActive(false);
        //disattivo AttackPanel
        AttackPanel.SetActive(false);
        //disattivo MagicsPanel
        MagicsPanel.SetActive(false);
        //per ogni bottone di attacco nella lista di AtkBtns
        foreach (GameObject Attackbutton in AtkBtns)
        {
            //distruggo i bottoni 
            Destroy(Attackbutton);
        }
        // pulisco la lista
        AtkBtns.Clear();
    }
    //creo una funzione che chiamo createAttackButtons
    void createAttackButtons()
    {
        //instazio actionButton come gameObject e lo assengo a AttackButton
        GameObject AttackButton = Instantiate(actionButton) as GameObject;
        //cerco dentro attackButton unGameObject chiamato text e cerco la componente text 
        //dopo di che lo assegno ad attackButtonText
        Text attackButtonText = AttackButton.transform.Find("Text").gameObject.GetComponent<Text>();
        //cambio il text in attack
        attackButtonText.text = "Attack";
        //Utilizzo la funzione addListener per Aggiungere Input1 come funzuione ogni volta che clicco il bottone
        AttackButton.GetComponent<Button>().onClick.AddListener(() => Input1());
        //uso la funzione set parent di AttackButton e passo i parametri actionSpacer a false
        AttackButton.transform.SetParent(actionSpacer, false);
        //aggiungo l'attack button alla lista
        AtkBtns.Add(AttackButton);

        //instazio actionButton come gameObject e lo assengo a MagicAttackButton
        GameObject MagicAttackButton = Instantiate(actionButton) as GameObject;
        //cerco dentro MagicAttackButton unGameObject chiamato text e cerco la componente text 
        //dopo di che lo assegno ad MagicButtonText
        Text MagicButtonText = MagicAttackButton.transform.Find("Text").gameObject.GetComponent<Text>();
        //cambio il text in Magic
        MagicButtonText.text = "Magic";
        //Utilizzo la funzione addListener per Aggiungere Input3 come funzuione ogni volta che clicco il bottone
        MagicAttackButton.GetComponent<Button>().onClick.AddListener(() => Input3());
        //uso la funzione set parent di AttackButton e passo i parametri actionSpacer a false
        MagicAttackButton.transform.SetParent(actionSpacer, false);
        //aggiungo MagicAttackButton alla lista
        AtkBtns.Add(MagicAttackButton);
        //se l'eroe non ha magie disponibili 
        if (HeroToManage[0].GetComponent<HeroStateM>().hero.MagicAttack.Count > 0)
        {
            //per ogni spellAttack all interno della lista delle magie del player scelto 
            foreach(BaseAttack SpellAtk in HeroToManage[0].GetComponent<HeroStateM>().hero.MagicAttack)
            {
                //instazia un magicButton
                GameObject SpellButton = Instantiate(MagicButtons);
                //assegna a SpellButtonText la componente text di MagicButton
                Text SpellButtonText = MagicButtons.transform.Find("Text").gameObject.GetComponent<Text>();
                //assegna il text di SpellButtonText al nome della spell
                SpellButtonText.text = SpellAtk.attackName;
                //assegna a ATB la componente AttackButton di MagicButton
                AttackButton ATB = MagicButtons.GetComponent<AttackButton>();
                //assegnaa ATB.magicAttackToPerform  la SpellAtk
                ATB.magicAttackToPerform = SpellAtk;
                //uso la funzione set parent di SpellButton e passo i parametri actionSpacer a false
                SpellButton.transform.SetParent(magicSpacer, false);
                //aggiungo SpellButton alla lista
                AtkBtns.Add(SpellButton);
            }
        }
        else
        {
            //imposto il componente Button di MagicAttackButton non interagibile
            MagicAttackButton.GetComponent<Button>().interactable = false;
        }

    }
    //creo una funzione void che chiamo Input4 Che richiederà un BaseAttack
    public void Input4(BaseAttack choosenMagic)//magic attack
    {
        //assegno a HeroChoice.Attacker il nome dell'eroe
        HeroChoice.Attacker = HeroToManage[0].name;
        //assegno a HeroChoice.AttacksGameObject il game Object dell'eroe
        HeroChoice.AttacksGameObject = HeroToManage[0];
        //cambio il tipo di heroChoice a Player
        HeroChoice.Type = "Player";
        //controllo se l'attackCost è minore dell'Mp corrente dell'Eroe selezionato
        if (choosenMagic.attackCost<=HeroToManage[0].GetComponent<HeroStateM>().hero.currMP)
        {
            //diminuisco l'MP dell'giocatore in base al costo della spell
            HeroToManage[0].GetComponent<HeroStateM>().hero.currMP -= choosenMagic.attackCost;
            //assengo al chosenAttack la magia scelta da player
            HeroChoice.chosenAttack = choosenMagic;
            //disattivo MagicsPanel
            MagicsPanel.SetActive(false);
            //attivo l'enemySelectPanel
            EnemySelectPanel.SetActive(true);
            //chiamo la funzione UpdateHeroPanel per cambiare il numero di mp
            HeroToManage[0].GetComponent<HeroStateM>().UpdateHeroPanel();
        }
        else
        {
            //disattivo MagicsPanel
            MagicsPanel.SetActive(false);
            //attivo AttackPanel
            AttackPanel.SetActive(true);
            //attivo testo
            testo.SetActive(true);
            //imposto in active a true
            isactive = true;
        }
    }
    //creo una funzione void che chiamo input 3
    public void Input3()//cambio pannello nelle magie
    {
        //disattivo AttackPanel
        AttackPanel.SetActive(false);
        //attivo MagicsPanel
        MagicsPanel.SetActive(true);
    }
}
