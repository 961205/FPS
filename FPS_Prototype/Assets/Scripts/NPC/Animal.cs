using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour
{
    [SerializeField] protected string animalName; //������ �̸�
    [SerializeField] protected int hp; //������ ü��

    [SerializeField] protected float walkSpeed; //�ȱ� ���ǵ�
    [SerializeField] protected float runSpeed; //�ٱ� ���ǵ�

    protected Vector3 destination; //������

    //���� ����
    protected bool isAction; //�ൿ �� ����
    protected bool isWalking; //�ȴ� �� ����
    protected bool isRunning; //�ٴ� �� ����
    protected bool isDead; //�׾����� ����

    [SerializeField] protected float walkTime; //�ȱ� �ð�
    [SerializeField] protected float waitTime; //��� �ð�
    [SerializeField] protected float runTime; //�ٱ� �ð�
    protected float currentTime;

    //�ʿ��� ������Ʈ
    [SerializeField] protected Animator anim;
    [SerializeField] protected Rigidbody rigid;
    [SerializeField] protected BoxCollider boxCol;
    protected AudioSource audioSource;
    protected NavMeshAgent nav;

    [SerializeField] protected AudioClip[] normalSound;
    [SerializeField] protected AudioClip hurtSound;
    [SerializeField] protected AudioClip deadSound;

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        currentTime = waitTime;
        isAction = true;
    }

    void Update()
    {
        if (!isDead)
        {
            Move();
            ElapseTime();
        }
    }

    protected void Move()
    {
        if (isWalking || isRunning)
            //rigid.MovePosition(transform.position + (transform.forward * applySpeed * Time.deltaTime));
            nav.SetDestination(transform.position + destination * 5f);
    }

    protected void ElapseTime()
    {
        if (isAction)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
                ReSet();
        }
    }
    protected virtual void ReSet()
    {
        isWalking = false; isRunning = false; isAction = true;
        nav.speed = walkSpeed;
        nav.ResetPath();
        anim.SetBool("Walking", isWalking); anim.SetBool("Running", isRunning);
        destination.Set(Random.Range(-0.2f, 0.2f), 0f, Random.Range(0.5f, 1f));
    }

    protected void TryWalk()
    {
        isWalking = true;
        anim.SetBool("Walking", isWalking);
        currentTime = walkTime;
        nav.speed = walkSpeed;
        Debug.Log("�ȱ�");
    }


    public virtual void Damage(int _dmg, Vector3 _targetPos)
    {
        if (!isDead)
        {
            hp -= _dmg;

            if (hp <= 0)
            {
                Dead();
                return;
            }
            PlaySE(hurtSound);
            anim.SetTrigger("Hurt");
        }
    }

    protected void Dead()
    {
        PlaySE(deadSound);
        isWalking = false;
        isRunning = false;
        isDead = true;
        anim.SetTrigger("Dead");
    }

    protected void RandomSound()
    {
        int _random = Random.Range(0, 3); //�ϻ� ���� 3��
        PlaySE(normalSound[_random]);
    }

    protected void PlaySE(AudioClip _clip)
    {
        audioSource.clip = _clip;
        audioSource.Play();
    }
}