using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public int enemyScore;
    public float speed;
    public int health;
    public Sprite[] sprites;

    public float maxShotDelay;
    public float curShotDelay;

    public GameObject bulletObjA;
    public GameObject bulletObjB;

    public GameObject itemCoin;
    public GameObject itemPower;
    public GameObject itemBoom;

    public GameObject player;
    public GameManager gameManager;
    public ObjectManager objectManager;

    SpriteRenderer spriteRenderer;
    Animator anim;

    public int patternIndex;
    public int curPatternCount;
    public int[] maxPatternCount;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (enemyName == "B")
            anim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        switch(enemyName)
        {
            case "B":
                health = 3000;
                Invoke("Stop", 2.0f);
                break;

            case "L":
                health = 40;
                break;

            case "M":
                health = 10;
                break;

            case "S":
                health = 3;
                break;
        }
    }

    void Stop()
    {
        if (!gameObject.activeSelf)
            return;

        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.zero;

        Invoke("Think", 2.0f);
    }

    void Think()
    {
        patternIndex = patternIndex == 3 ? 0 : patternIndex + 1;

        curPatternCount = 0;

        switch (patternIndex)
        {
            case 0:
                FireForward();
                break;
            case 1:
                FireShot();
                break;
            case 2:
                FireArc();
                break;
            case 3:
                FireFAround();
                break;
        }
    }

    void FireForward()
    {
        //Debug.Log("앞으로 4발 발사");

        // #.Fire 4 Bullet Forward
        GameObject bulletR = objectManager.MakeObj("BulletBossA");
        bulletR.transform.position = transform.position + Vector3.right * 0.3f;

        GameObject bulletRR = objectManager.MakeObj("BulletBossA");
        bulletRR.transform.position = transform.position + Vector3.right * 0.45f;

        GameObject bulletL = objectManager.MakeObj("BulletBossA");
        bulletL.transform.position = transform.position + Vector3.left * 0.3f;

        GameObject bulletLL = objectManager.MakeObj("BulletBossA");
        bulletLL.transform.position = transform.position + Vector3.left * 0.45f;

        Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();

        Vector3 dirVecR = player.transform.position - (transform.position + Vector3.right * 0.3f);
        Vector3 dirVecRR = player.transform.position - (transform.position + Vector3.right * 0.45f);
        Vector3 dirVecL = player.transform.position - (transform.position + Vector3.left * 0.3f);
        Vector3 dirVecLL = player.transform.position - (transform.position + Vector3.left * 0.45f);

        rigidR.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
        rigidRR.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
        rigidL.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
        rigidLL.AddForce(Vector2.down * 8, ForceMode2D.Impulse);

        // #.Pattern Counting
        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireForward", 2.0f);
        else
            Invoke("Think", 3.0f);
    }

    void FireShot()
    {
        //Debug.Log("플레이어 방향으로 샷건");
        
        // #.Fire 5 Random Shotgun Bullet to Player
        for(int index = 0; index < 5; index++)
        {
            GameObject bullet = objectManager.MakeObj("BulletEnemyB");
            bullet.transform.position = transform.position;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector2 dirVec = player.transform.position - transform.position;
            Vector2 ranVec = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(0.0f, 2.0f));
            dirVec += ranVec;
            rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
        }

        // #.Pattern Counting
        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireShot", 3.5f);
        else
            Invoke("Think", 3.0f);
    }

    void FireArc()
    {
        //Debug.Log("부채 모양으로 발사");

        // #.Fire Arc Continue Fire
        GameObject bullet = objectManager.MakeObj("BulletEnemyA");
        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.identity;

        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        Vector2 dirVec = new Vector2( Mathf.Cos(Mathf.PI * 10 * curPatternCount / maxPatternCount[patternIndex]) , Mathf.Sin(Mathf.PI * 10 * curPatternCount / maxPatternCount[patternIndex]) );
        rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);

        // #.Pattern Counting
        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireArc", 0.15f);
        else
            Invoke("Think", 3.0f);
    }

    void FireFAround()
    {
        //Debug.Log("원 형태로 전체 공격");

        // #.Fire Around
        int roundNumA = 50;
        int roundNumB = 40;
        int roundNum = (curPatternCount % 2 == 0) ? roundNumA : roundNumB;

        for (int index = 0; index < roundNum; index++)
        {
            GameObject bullet = objectManager.MakeObj("BulletBossB");
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * index / roundNum), Mathf.Sin(Mathf.PI * 2 * index / roundNum));
            rigid.AddForce(dirVec.normalized * 2, ForceMode2D.Impulse);

            Vector3 rotVec = Vector3.forward * 360 * index / roundNum + Vector3.forward * 90;
            bullet.transform.Rotate(rotVec);
        }

        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireFAround", 0.7f);
        else
            Invoke("Think", 3.0f);
    }

    void Update()
    {
        if (enemyName == "B")
            return;

        Fire();
        Reload();
    }

    void Fire()
    {
        if (curShotDelay < maxShotDelay)
            return;

        if(enemyName == "S")
        {
            GameObject bullet = objectManager.MakeObj("BulletEnemyA");
            bullet.transform.position = transform.position;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector3 dirVec = player.transform.position - transform.position;
            rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
        }
        else if(enemyName == "L")
        {
            GameObject bulletR = objectManager.MakeObj("BulletEnemyB");
            bulletR.transform.position = transform.position + Vector3.right * 0.3f;

            GameObject bulletL = objectManager.MakeObj("BulletEnemyB");
            bulletL.transform.position = transform.position + Vector3.left * 0.3f;

            Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
            Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();

            Vector3 dirVecR = player.transform.position - (transform.position + Vector3.right * 0.3f);
            Vector3 dirVecL = player.transform.position - (transform.position + Vector3.left * 0.3f);

            rigidR.AddForce(dirVecR.normalized * 4, ForceMode2D.Impulse);
            rigidL.AddForce(dirVecL.normalized * 4, ForceMode2D.Impulse);
        }

        curShotDelay = 0;
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    public void OnHit(int dmg)
    {
        if (health < 0)
            return;

        health -= dmg;

        if(enemyName == "B")
        {
            anim.SetTrigger("OnHit");
        }
        else
        {
            spriteRenderer.sprite = sprites[1];
            Invoke("ReturnSprite", 0.1f);
        }


        if (health <= 0)
        {
            Player playerLogic = player.GetComponent<Player>();
            playerLogic.score += enemyScore;

            // #.Random Ratio Item Drop
            int ran = enemyName == "B" ? 0 : Random.Range(0, 10);

            if(ran < 5) // 50%
            {
                //Debug.Log("Not Item");
            }
            else if(ran < 8)    // 30%  // Coin
            {
                GameObject itemCoin = objectManager.MakeObj("ItemCoin");
                itemCoin.transform.position = transform.position;
            }
            else if (ran < 9)   // 10%  // Power
            {
                GameObject itemPower = objectManager.MakeObj("ItemPower");
                itemPower.transform.position = transform.position;
            }
            else if (ran < 10)  // 10%  // Boom
            {
                GameObject itemBoom = objectManager.MakeObj("ItemBoom");
                itemBoom.transform.position = transform.position;
            }

            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;

            gameManager.CallExplosion(transform.position, enemyName);

            // #.Boss Kill
            if (enemyName == "B")
                gameManager.StageEnd();
        }

        
    }

    void ReturnSprite()
    {
        spriteRenderer.sprite = sprites[0];
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BorderBullet" && enemyName != "B")
        {
            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;
        }
        else if (collision.gameObject.tag == "PlayerBullet")
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();

            OnHit(bullet.dmg);

            collision.gameObject.SetActive(false);
        }
    }
}
