using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class playerController : MonoBehaviour
{
    [Header("---- Componets ----")]
    [SerializeField] CharacterController controller;
    [SerializeField] Camera playerCamera; 

    [Header("---- Player Stats ----")]
    [SerializeField] float playerSpeed;
    [SerializeField] float jumpHeight;
    [SerializeField] float gravityValue;
    [SerializeField] int jumpCount;
    [SerializeField] float sprintMod;
    [SerializeField] int dashMod;
    [SerializeField] float dashTime;
    

    [Header("---- Weapon Stats ----")]
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;
    [SerializeField] int shootDmg;
    [SerializeField] GameObject gunModel;
    public List<gunStats> gunList = new List<gunStats>();
    [SerializeField] GameObject hitEffect;


    private Vector3 playerVelocity;
    int jumpTimes;
    Vector3 moveDir;
    bool isSprinting = false;
    bool isShooting;
    bool isDashing = false;
    float speedOrig;
    float counter = 0;
    int HPorignal;
    int selectedGun;
    

    private void Start()
    {
        speedOrig = playerSpeed;
    }

    void Update()
    {
        movement();
        sprint();
        if (playerVelocity.y > 0f && Input.GetButtonDown("Dash"))//Dash is set to F for now
        {
            StartCoroutine(dash());
            counter = 0;
        }
        StartCoroutine(shoot());
    }


    void movement()
    {
        if (controller.isGrounded && playerVelocity.y < 0)
        {
            jumpTimes = 0;
            playerVelocity.y = 0f;
        }

        moveDir = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
        controller.Move(moveDir * Time.deltaTime * playerSpeed);

        if (Input.GetButtonDown("Jump") && jumpTimes < jumpCount)
        {
            jumpTimes++;
            playerVelocity.y = jumpHeight;
        }

        playerVelocity.y -= gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
    void sprint()
    {
        if(!isSprinting && Input.GetButtonDown("Sprint") && playerVelocity.y <= 0)
        {
            playerSpeed *= sprintMod;
            isSprinting = true;
        }
        else if(isSprinting && Input.GetButtonUp("Sprint"))
        {
            playerSpeed = speedOrig;
            isSprinting = false;
        }
    }
    IEnumerator dash()
    {
        float gravOrig = gravityValue;
        float prevSpeed = playerSpeed;
        float startTime = Time.time; 

        while (Time.time - startTime <= dashTime) 
        {
            if (!isDashing)
            {
                playerSpeed *= dashMod;
                playerVelocity.y = 0;
                gravityValue = 0f;
                isDashing = true;
            }
            yield return null;
        }

        playerSpeed = prevSpeed;
        gravityValue = gravOrig;
        isDashing = false;
    }
    public void gunPickup(gunStats gunStat)
    {
        shootRate = gunStat.shootRate;
        shootDist = gunStat.shootDist;
        shootDmg = gunStat.shootDmg;
        hitEffect = gunStat.hitEffect;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunStat.gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunStat.gunModel.GetComponent<MeshRenderer>().sharedMaterial;

        gunList.Add(gunStat);
    }

    public void gunDrop()
    {
        shootRate = 0;
        shootDist = 0;
        shootDmg = 0;
        hitEffect = null;

        gunModel.GetComponent<MeshFilter>().sharedMesh = null;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = null;

        gunList.Clear();
    }

    IEnumerator shoot()
    {
        if (gunList.Count > 0 && isShooting == false && Input.GetButton("Shoot"))
        {
            isShooting = true;

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDist))
            {
                if (hit.collider.GetComponent<IDamage>() != null)
                {
                    hit.collider.GetComponent<IDamage>().takeDamage(shootDmg);
                }

                Instantiate(hitEffect, hit.point, hitEffect.transform.rotation);
            }

            yield return new WaitForSeconds(shootRate);
            isShooting = false;
        }
    }

    public void respawn()
    {
        controller.enabled = false;
        transform.position = gameManager.instance.spawnPos.transform.position;
        gameManager.instance.playerDeadMenu.SetActive(false);
        controller.enabled = true;
    }
}
