using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BaitPing : MonoBehaviour
{
    public Entity Owner;
    public PhotonView View;

    public List<Sprite> pingImages;

    public SpriteRenderer spriteRenderer;

    public float xPos, yBegin, yEnd;
    public float animationSpeed;

    private int pingCount = 0;
    private bool isCooldown = false;

    private void Start()
    {
        spriteRenderer.enabled = false;
    }

    private void Update()
    {
        transform.position = new Vector3(xPos, transform.position.y, 0f);

        if (Owner.IsClientEntity)
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                Vector3 mousePosition = Input.mousePosition;
                mousePosition.z = 10f;
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

                Ping(worldPosition.x, 0);
            }
        }
    }

    public void Ping(float xPosition, int pingType)
    {
        if (!isCooldown)
        {
            SpawnPing(xPosition, pingType);

            if (pingCount > 2)
            {
                pingCount = 0;
                StartCoroutine(StartCooldown());
            }
        }
        else
        {
            UIManager.Instance.ShowGeneralInfo($"Attendez quelques secondes avant de réutiliser le ping.");
        }
    }

    private void SpawnPing(float xPosition, int pingType)
    {
        pingCount++;

        xPos = xPosition;


        View.RPC("RPC_SpawnPing", RpcTarget.All, xPosition, pingType);

    }

    private IEnumerator StartCooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(3f);
        isCooldown = false;
    }

    public void Animate()
    {
        StopAllCoroutines();
        StartCoroutine(Coroutine_Animate());
    }

    IEnumerator Coroutine_Animate()
    {
        spriteRenderer.enabled = true;

        spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        transform.position = new Vector3(xPos, yBegin, 0f);

        //float time = 0f;

        while (!Mathf.Approximately(transform.position.y, yEnd))
        {
            transform.position = new Vector3(xPos, Mathf.Lerp(transform.position.y, yEnd, Time.deltaTime * animationSpeed), 0f);
            spriteRenderer.color = new Color(1f, 1f, 1f,  (yEnd - transform.position.y) / yEnd);
            yield return null;
        }

        spriteRenderer.enabled = false;
    }

    [PunRPC]
    private void RPC_SpawnPing(float xPosition, int pingType)
    {
        if (ClientManager.Instance.IsEnemy(Owner)) return;

        xPos = xPosition;
        spriteRenderer.sprite = pingImages[pingType];

        Animate();
        AudioManager.PlaySound(Audio.Bait, new Vector3(xPos, 1.5f), 10f);
    }
}
