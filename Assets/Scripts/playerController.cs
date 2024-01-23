using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class playerController : MonoBehaviour
{
    [Header("---- Componets ----")]
    [SerializeField] CharacterController controller;

    [Header("---- Player Stats ----")]
    [SerializeField] float playerSpeed;
    [SerializeField] float jumpHeight;
    [SerializeField] float gravityValue;
    [SerializeField] int jumpCount;
    [SerializeField] int sprintMod;

    [Header("---- Weapon Stats ----")]
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;
    [SerializeField] int shootDmg;
    [SerializeField] GameObject gunModel;
    public List<gunStats> gunList = new List<gunStats>();
    [SerializeField] GameObject hitEffect;


    private Vector3 playerVelocity;
    int jumpTimes;
    Vector3 move;
    bool isSprinting;
    bool isShooting;
    float speedOrig;
    int HPorignal;
    int selectedGun;

    private void Start()
    {

    }

    void Update()
    {
        movement();
        sprint();
        StartCoroutine(shoot());
    }
    void movement()
    {
        if (controller.isGrounded && playerVelocity.y < 0)
        {
            jumpTimes = 0;
            playerVelocity.y = 0f;
        }

        move = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
        controller.Move(move * Time.deltaTime * playerSpeed);

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
        if(Input.GetButtonDown("Sprint"))
        {
            playerSpeed *= sprintMod;
            isSprinting = true;
        }
        else if(Input.GetButtonUp("Sprint"))
        {
            playerSpeed /= sprintMod;
            isSprinting = false;
        }
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
