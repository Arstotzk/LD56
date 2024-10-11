using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class PlayerInput : MonoBehaviour
{
    private GameControl gameControl;

    public Menu menu;
    public Hook hookFirst;
    public Hook hookSecond;
    public Spawn spawn;
    public UIController uiController;
    public SpriteRenderer spriteRenderer;
    public Sprite spriteIdle;
    public Sprite spriteRun;
    public Sprite spriteRun2;
    public PlayerSounds playerSounds;
    public GameObject groundChecker;
    public ParticleSystem runEffect;
    public ParticleSystem deadEffect;
    public BotsManager botsManager;

    public float speed = 4f;
    public float speedClimb = 4f;
    public float speedMoveOnRope = 1f;
    public float forceSwing = 4f;
    public float jumpHeight = 2f;
    public bool isOnGround = true;
    public float ropeDistance = 0f;
    public float ropeConst = 0.5f;
    public int maxAvailableBots = 1;
    public int availableBots = 0;
    public bool isDestroing = false;

    private int frame = 0;
    private int maxFrame = 20;
    private bool _isTurnRight;
    public bool isTurnRight 
    { 
        get 
        {
            return _isTurnRight;
        } 
        set 
        {
            if (_isTurnRight != value)
            {
                if (value == true)
                    spriteRenderer.flipX = false;
                else
                    spriteRenderer.flipX = true;
                _isTurnRight = value;
            }
        }
    }

    void Start()
    {
        isTurnRight = true;
        botsManager = GameObject.FindFirstObjectByType<BotsManager>();
        botsManager.AddBot(this.gameObject);
        uiController.SetAvailableBots(botsManager.availableBots);
        uiController.SetMaxAvailableBots(botsManager.maxAvailableBots);
    }
    void Update()
    {
        if (gameControl.Gameplay.enabled) 
        {
            Move();
        }
        if (gameControl.OnOneHook.enabled)
        {
            Climb();
            Swing();
        }
        if (gameControl.OnTwoHook.enabled)
        {
            MoveOnRope();
            Tense();
        }
    }
    public void Awake()
    {
        gameControl = new GameControl();
        gameControl.Enable();
        gameControl.Menu.Disable();
        gameControl.OnOneHook.Disable();
        gameControl.OnTwoHook.Disable();
        gameControl.Gameplay.Disable();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
    public void OnEnable()
    {
        botsManager = GameObject.FindFirstObjectByType<BotsManager>();
        gameControl.Gameplay.Jamp.performed += Jump;

        gameControl.Gameplay.FirstHook.performed += SetFirstHook;
        gameControl.OnOneHook.FirstHook.performed += SetFirstHook;
        gameControl.OnTwoHook.FirstHook.performed += SetFirstHook;

        gameControl.Gameplay.SecondHook.performed += SetSecondHook;
        gameControl.OnOneHook.SecondHook.performed += SetSecondHook;
        gameControl.OnTwoHook.SecondHook.performed += SetSecondHook;

        gameControl.Gameplay.Destroy.performed += Destroy;
        gameControl.OnOneHook.Destroy.performed += Destroy;
        gameControl.OnTwoHook.Destroy.performed += Destroy;

        gameControl.Gameplay.NewBot.performed += NewBot;
        gameControl.OnOneHook.NewBot.performed += NewBot;
        gameControl.OnTwoHook.NewBot.performed += NewBot;

        gameControl.Gameplay.ShowMenu.performed += ShowMenu;
        gameControl.Menu.UnshowMenu.performed += UnshowMenu;
    }
    public void OnDisable()
    {
        gameControl.Gameplay.Jamp.performed -= Jump;

        gameControl.Gameplay.FirstHook.performed -= SetFirstHook;
        gameControl.OnOneHook.FirstHook.performed -= SetFirstHook;
        gameControl.OnTwoHook.FirstHook.performed -= SetFirstHook;

        gameControl.Gameplay.SecondHook.performed -= SetSecondHook;
        gameControl.OnOneHook.SecondHook.performed -= SetSecondHook;
        gameControl.OnTwoHook.SecondHook.performed -= SetSecondHook;

        gameControl.Gameplay.Destroy.performed -= Destroy;
        gameControl.OnOneHook.Destroy.performed -= Destroy;
        gameControl.OnTwoHook.Destroy.performed -= Destroy;

        gameControl.Gameplay.NewBot.performed -= NewBot;
        gameControl.OnOneHook.NewBot.performed -= NewBot;
        gameControl.OnTwoHook.NewBot.performed -= NewBot;

        gameControl.Gameplay.ShowMenu.performed -= ShowMenu;
        gameControl.Menu.UnshowMenu.performed -= UnshowMenu;
    }
    private Sprite GetSpriteRun() 
    {
        if (frame > maxFrame)
            frame = 0;
        frame++;
        if (frame < maxFrame/2)
            return spriteRun;
        else
            return spriteRun2;
    }
    private void Move()
    {
        var direction = gameControl.Gameplay.Move.ReadValue<float>() * speed * Time.deltaTime;
        if (direction > 0)
        {
            isTurnRight = true;
            spriteRenderer.sprite = GetSpriteRun();
            if (isOnGround)
            {
                runEffect.gameObject.SetActive(true);
                var velocity = runEffect.velocityOverLifetime;
                velocity.xMultiplier = -0.4f;
            }
            if (!playerSounds.run.isPlaying)
                playerSounds.run.Play();
        }
        else if (direction < 0)
        {
            isTurnRight = false;
            spriteRenderer.sprite = GetSpriteRun();
            if (isOnGround)
            {
                runEffect.gameObject.SetActive(true);
                var velocity = runEffect.velocityOverLifetime;
                velocity.xMultiplier = 0.4f;
            }
            if (!playerSounds.run.isPlaying)
                playerSounds.run.Play();
        }
        else
        {
            runEffect.gameObject.SetActive(false);
            spriteRenderer.sprite = spriteIdle;
            playerSounds.run.Stop();
        }
        Vector3 movement = transform.right * direction;
        transform.position += movement; 
    }
    private void Jump(InputAction.CallbackContext ctx)
    {
        if (isOnGround)
        {
            Vector3 movement = transform.up * jumpHeight;
            GetComponent<Rigidbody2D>().AddForceY(movement.y);
            //isOnGround = false;
            playerSounds.jump.Play();
            
        }
    }
    private void Climb()
    {
        var valueClimb = gameControl.OnOneHook.Tension.ReadValue<float>() * speedClimb * Time.deltaTime;
        if (valueClimb > 0 && isOnGround)
        {
            playerSounds.moveOnRope.Stop();
            return;
        }
        else if (valueClimb != 0)
        {
            if (!playerSounds.moveOnRope.isPlaying)
                playerSounds.moveOnRope.Play();
        }
        else
        {
            playerSounds.moveOnRope.Stop();
        }
        if (hookFirst.isEnabled)
            hookFirst.Climb(valueClimb);
        if (hookSecond.isEnabled)
            hookSecond.Climb(valueClimb);
    }

    private void Swing()
    {
        var valueSwing = gameControl.OnOneHook.Move.ReadValue<float>() * forceSwing * Time.deltaTime;
        GetComponent<Rigidbody2D>().AddForceX(valueSwing);
    }

    private void MoveOnRope() 
    {
        var valueMove = gameControl.OnTwoHook.Move.ReadValue<float>() * speedMoveOnRope * Time.deltaTime;
        if (hookFirst.joint.connectedBody.transform.position.x > hookSecond.joint.connectedBody.transform.position.x)
            valueMove = -valueMove;

        if (hookFirst.joint.distance + valueMove > ropeDistance || hookFirst.joint.distance + valueMove < hookFirst.minDistance ||
            hookSecond.joint.distance - valueMove > ropeDistance || hookSecond.joint.distance - valueMove < hookSecond.minDistance)
        {
            return;
        }

        if (valueMove != 0)
        {
            if (!playerSounds.moveOnRope.isPlaying)
            {
                playerSounds.moveOnRope.Play();
                playerSounds.tense.Stop();
            }
        }
        else
        {
            playerSounds.moveOnRope.Stop();
        }

        hookFirst.Climb(valueMove);
        hookSecond.Climb(-valueMove);
    }
    private void Tense() 
    {
        var valueMove = gameControl.OnTwoHook.Tension.ReadValue<float>() * speedMoveOnRope * Time.deltaTime;
        if (hookFirst.joint.distance + hookSecond.joint.distance + valueMove < ropeDistance && (hookFirst.joint.connectedBody.gameObject.tag != "Box" && hookSecond.joint.connectedBody.gameObject.tag != "Box"))
        {
            playerSounds.tense.Stop();
            return;
        }
        if (valueMove > 0 && isOnGround)
        {
            return;
        }

        if (valueMove != 0)
        {
            if (!playerSounds.tense.isPlaying)
            {
                playerSounds.tense.Play();
                playerSounds.moveOnRope.Stop();
            }
        }
        else 
        {
            playerSounds.tense.Stop();
        }

        hookFirst.Climb(valueMove);
        hookSecond.Climb(valueMove);
    }
    private void SetFirstHook(InputAction.CallbackContext ctx)
    {
        SetHook(hookFirst);
    }

    private void SetSecondHook(InputAction.CallbackContext ctx)
    {
        SetHook(hookSecond);
    }

    private void SetHook(Hook hook)
    {
        if (gameControl.Gameplay.enabled)
        {
            var isSetted = hook.SetHook();
            if (isSetted)
            {
                gameControl.Gameplay.Disable();
                gameControl.OnOneHook.Enable();
                playerSounds.run.Stop();
                playerSounds.hookSet.Play();
            }
            return;
        }
        if (gameControl.OnOneHook.enabled)
        {
            if (hook.isEnabled)
            {
                gameControl.Gameplay.Enable();
                gameControl.OnOneHook.Disable();
                playerSounds.hookUnSet.Play();
                hook.UnsetHook();
                return;
            }
            else
            {
                var isSetted = hook.SetHook();
                if (isSetted)
                {
                    gameControl.OnOneHook.Disable();
                    gameControl.OnTwoHook.Enable();
                    ropeDistance = Vector2.Distance(hookFirst.joint.connectedBody.transform.position, hookSecond.joint.connectedBody.transform.position) - ropeConst;
                    playerSounds.hookSet.Play();
                    return;
                }
            }
        }
        if (gameControl.OnTwoHook.enabled)
        {
            gameControl.OnTwoHook.Disable();
            gameControl.OnOneHook.Enable();
            playerSounds.hookUnSet.Play();
            hook.UnsetHook();
            return;
        }
    }

    private void NewBot(InputAction.CallbackContext ctx) 
    {
        if (botsManager.availableBots > 0)
        {
            spawn.SpawnBot(this.gameObject, false);
            this.enabled = false;
            playerSounds.spawn.Play();
        }
    }
    private void Destroy(InputAction.CallbackContext ctx)
    {
        Destroy();
    }
    public void Destroy()
    {
        var previousBot = botsManager.GetPreviosBot();
        botsManager.DeleteBot(this.gameObject);
        var objects = GameObject.FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);
        if (this.enabled == false)
        {
            StartCoroutine(RealDestroy());
            playerSounds.died.Play();
            deadEffect.Play();
            isDestroing = true;
            foreach (var player in objects)
            {
                if (player.enabled == true)
                {
                    player.botsManager.availableBots += 1;
                    player.SetAvailableBots();
                }
            }
            return;
        }
        if (isDestroing == true)
            return;
        if (previousBot == null || previousBot.GetComponent<PlayerInput>().isDestroing)
        {
            //if (objects.Where(p => p.isDestroing).Count() >= 1)
            //    return;
            spawn.SpawnBot(this.gameObject, true);
            playerSounds.spawn.Play();
        }
        else
        {
            botsManager.availableBots += 1;
            previousBot.GetComponent<PlayerInput>().enabled = true;
            int LayerIgnoreRaycast = LayerMask.NameToLayer("Default");
            previousBot.gameObject.layer = LayerIgnoreRaycast;
            groundChecker.SetActive(true);
            previousBot.GetComponent<PlayerInput>().SetAvailableBots();
        }
        gameControl.Gameplay.Disable();
        StartCoroutine(RealDestroy());
        playerSounds.died.Play();
        deadEffect.Play();
        isDestroing = true;
    }
    public IEnumerator RealDestroy()
    {
        yield return new WaitForSeconds(0.5f);
        deadEffect.Stop();
        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);
    }
    public void SetAvailableBots() 
    {
        uiController.SetAvailableBots(botsManager.availableBots);
    }

    public void SetIsOnGround(bool value)
    {
        if (value == true && isOnGround != value)
            playerSounds.onGround.Play();
        isOnGround = value;
        if(value == false)
            runEffect.gameObject.SetActive(false);
    }

    private void ShowMenu(InputAction.CallbackContext ctx)
    {
        menu.UI.SetActive(true);
        gameControl.Gameplay.Disable();
        gameControl.Menu.Enable();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
    private void UnshowMenu(InputAction.CallbackContext ctx)
    {
        menu.UI.SetActive(false);
        gameControl.Gameplay.Enable();
        gameControl.Menu.Disable();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
    public void DialogUnshow()
    {
        gameControl.Dialog.Disable();
        gameControl.Gameplay.Enable();
        uiController.dialogUI.SetActive(false);
    }
}
