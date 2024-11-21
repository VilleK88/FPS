using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyManager : MonoBehaviour
{
    #region Singleton
    public static EnemyManager Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
    #endregion
    public StatePatternEnemy[] enemies;
    public Image indicatorImage;
    public Sprite patrolImage;
    public Sprite alertImage;
    public Sprite trackingImage;
    public Sprite combatImage;
    Player player;
    public LayerMask enemyLayer;
    public float enemyDataRadius = 300f;
    private void Start()
    {
        player = PlayerManager.instance.GetPlayer().GetComponent<Player>();
        enemies = FindObjectsOfType<StatePatternEnemy>();
        if (GameManager.instance.loadEnemiesData)
            LoadEnemiesData();
    }
    public void SaveEnemiesData()
    {
        GameManager.instance.nearbyEnemies.Clear();
        Vector3 playerPosition = player.transform.position;
        Collider[] colliders = Physics.OverlapSphere(playerPosition, enemyDataRadius, enemyLayer);
        foreach(Collider collider in colliders)
        {
            StatePatternEnemy enemy = collider.GetComponent<StatePatternEnemy>();
            if (enemy != null)
            {
                EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                EnemyData enemyData = new EnemyData
                {
                    enemyDataID = enemy.enemyID,
                    enemyPositionX = enemy.transform.position.x,
                    enemyPositionY = enemy.transform.position.y,
                    enemyPositionZ = enemy.transform.position.z,
                    enemyRotationX = enemy.transform.eulerAngles.x,
                    enemyRotationY = enemy.transform.eulerAngles.y,
                    enemyRotationZ = enemy.transform.eulerAngles.z,
                    waypointIndexData = enemy.patrolState.waypointIndex,
                    dead = enemyHealth.dead,
                    alreadyFoundDead = enemyHealth.alreadyFoundDead,
                    ragdollData = SaveRagdollTransforms(enemy.transform) 
                };
                GameManager.instance.nearbyEnemies.Add(enemyData);
            }
        }
    }
    public void LoadEnemiesData()
    {
        Vector3 playerPosition = player.transform.position;
        Collider[] colliders = Physics.OverlapSphere(playerPosition, enemyDataRadius, enemyLayer);
        foreach (Collider collider in colliders)
        {
            StatePatternEnemy enemy = collider.GetComponent<StatePatternEnemy>();
            if (enemy != null)
            {
                EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                EnemyData enemyData = GameManager.instance.nearbyEnemies.Find(enemyData => enemyData.enemyDataID == enemy.enemyID);
                if(enemyData != null)
                {
                    enemy.transform.position = new Vector3(enemyData.enemyPositionX, enemyData.enemyPositionY, enemyData.enemyPositionZ);
                    enemy.transform.eulerAngles = new Vector3(enemyData.enemyRotationX, enemyData.enemyRotationY, enemyData.enemyRotationZ);
                    enemy.patrolState.waypointIndex = enemyData.waypointIndexData;
                    enemyHealth.dead = enemyData.dead;
                    enemyHealth.alreadyFoundDead = enemyData.alreadyFoundDead;
                    if (enemyHealth.dead)
                    {
                        if (enemyHealth.rigidBodies == null) enemyHealth.rigidBodies = enemyHealth.GetComponentsInChildren<Rigidbody>();
                        enemyHealth.ActivateRagdoll();
                        enemyHealth.DieData();
                        LoadRagdollTransforms(enemy.transform, enemyData.ragdollData);
                    }
                }
            }
        }
        GameManager.instance.loadEnemiesData = false;
    }
    public RagdollData SaveRagdollTransforms(Transform rootBone)
    {
        RagdollData ragdollData = new RagdollData();
        SaveBoneTransforms(rootBone, ragdollData);
        return ragdollData;
    }
    void SaveBoneTransforms(Transform bone, RagdollData ragdollData)
    {
        BoneData boneData = new BoneData
        {
            boneName = bone.name,
            bonePositionX = bone.localPosition.x,
            bonePositionY = bone.localPosition.y,
            bonePositionZ = bone.localPosition.z,
            boneRotationX = bone.localEulerAngles.x,
            boneRotationY = bone.localEulerAngles.y,
            boneRotationZ = bone.localEulerAngles.z
        };
        ragdollData.bones.Add(boneData);
        foreach (Transform child in bone)
            SaveBoneTransforms(child, ragdollData);
    }
    public void LoadRagdollTransforms(Transform rootBone, RagdollData ragdollData)
    {
        foreach(BoneData boneData in ragdollData.bones)
        {
            Transform bone = FindBoneByName(rootBone, boneData.boneName);
            if(bone != null)
            {
                bone.localPosition = new Vector3(boneData.bonePositionX, boneData.bonePositionY, boneData.bonePositionZ);
                bone.localEulerAngles = new Vector3(boneData.boneRotationX, boneData.boneRotationY, boneData.boneRotationZ);
            }
        }
    }
    Transform FindBoneByName(Transform parent, string name)
    {
        if (parent.name == name)
            return parent;
        foreach(Transform child in parent)
        {
            Transform found = FindBoneByName(child, name);
            if (found != null)
                return found;
        }
        return null;
    }
    public IEnumerator BackToPatrol()
    {
        indicatorImage.sprite = patrolImage;
        yield return new WaitForSeconds(3);
        indicatorImage.enabled = false;
    }
    public void CloseEnemyHealthbars()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                EnemyHealth enemyHealth = enemies[i].GetComponent<EnemyHealth>();
                if (enemyHealth != null && enemyHealth.healthbar.activeSelf)
                    enemyHealth.HideHealth();
            }
        }
    }
    public bool CanAnyoneSeeThePlayer()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].canSeePlayer == true)
                return true;
        }
        return false;
    }
    public bool CloseIndicatorImage()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].currentState != enemies[i].patrolState)
                return false;
        }
        return true;
    }
}