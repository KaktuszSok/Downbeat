using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;
    public LevelManager levelMgr;
    public PlayerEntity playerEntity; //Data side of the player (us!)

    public SFXCollection walkSFX;
    public SFXCollection turnSFX;

    public KeyCode walkForward = KeyCode.W;
    public KeyCode walkBack = KeyCode.S;
    public KeyCode turnLeft = KeyCode.A;
    public KeyCode turnRight = KeyCode.D;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        levelMgr = LevelManager.instance;
    }

    private void Update()
    {
        if (playerEntity != LevelManager.player)
        {
            playerEntity = LevelManager.player;
        }
        if (playerEntity != null)
        {
            if(Input.anyKeyDown)
            {
                BeatDisplay.instance.AddInputMarker();
            }
            if (Input.GetKeyDown(walkForward))
            {
                bool movedSuccessfully = levelMgr.PlayerTakeAction(new ActionMove(playerEntity, playerEntity.pos + (IntVec)playerEntity.forward)).ToBool();
                if(movedSuccessfully) walkSFX.PlayRandomSoundAtPosition((Vector3)playerEntity.pos, 1, 1);
            }
            if (Input.GetKeyDown(walkBack))
            {
                bool movedSuccessfully = levelMgr.PlayerTakeAction(new ActionMove(playerEntity, playerEntity.pos - (IntVec)playerEntity.forward)).ToBool();
                if (movedSuccessfully) walkSFX.PlayRandomSoundAtPosition((Vector3)playerEntity.pos, 1, 0.7f);
            }
            if (Input.GetKeyDown(turnLeft))
            {
                playerEntity.forward = playerEntity.forward.Turn(-1);
                turnSFX.PlayRandomSoundAtPosition(transform.position);
            }
            if (Input.GetKeyDown(turnRight))
            {
                playerEntity.forward = playerEntity.forward.Turn(1);
                turnSFX.PlayRandomSoundAtPosition(transform.position);
            }
        }
    }

    void LateUpdate()
    {
        if (playerEntity != LevelManager.player)
        {
            playerEntity = LevelManager.player;
        }
        if (playerEntity != null)
        {
            Vector3 pos = transform.position;
            Vector3 fwd = transform.forward;

            Vector3 targetPos = levelMgr.transform.position + (Vector3)playerEntity.pos;
            Vector3 targetFwd = playerEntity.forward.ToVector3();

            if (!playerEntity.justTeleported)
            {
                pos = Vector3.MoveTowards(pos, targetPos, Mathf.Max(10, (targetPos - pos).sqrMagnitude * 10f) * Time.deltaTime);
                fwd = Quaternion.RotateTowards(Quaternion.LookRotation(fwd), Quaternion.LookRotation(targetFwd), 900f * Time.deltaTime) * Vector3.forward;
            }
            else
            {
                playerEntity.justTeleported = false;
                pos = targetPos;
                fwd = targetFwd;
            }

            transform.position = pos;
            transform.forward = fwd;
        }
    }
}
