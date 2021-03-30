﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pig : MonoBehaviour
{
    [SerializeField] private string animalName; // 동물이름
    [SerializeField] private int hp; // 동물의 체력

    [SerializeField] private float walkSpeed; // 걷기속도
    [SerializeField] private float runSpeed; // 뛰기속도
    private float applySpeed;

    private Vector3 direction; // 방향

    // 상태변수
    private bool isAction; // 행동중인지 아닌지 판별
    private bool isWalking; // 걷는지 안걷는지 판별
    private bool isRunning; // 뛰는지 판별
    private bool isDead; // 죽었는지 판별

    [SerializeField] private float walkTime; // 걷기 시간
    [SerializeField] private float waitTime; // 대기 시간
    [SerializeField] private float runTime; // 뛰기 시간
    private float currentTime;

    // 필요한 컴포넌트
    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private BoxCollider boxCol;
    private AudioSource theAudio;

    [SerializeField] private AudioClip[] sound_pig_Normal;
    [SerializeField] private AudioClip sound_pig_Hurt;
    [SerializeField] private AudioClip sound_pig_Dead;

    // Start is called before the first frame update
    void Start()
    {
        theAudio = GetComponent<AudioSource>();
        currentTime = waitTime;
        isAction = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            Move();
            Rotation();
            ElapseTime();
        }   
    }

    private void Move() // 움직임
    {
        if (isWalking || isRunning)
            rigid.MovePosition(transform.position + (transform.forward * applySpeed * Time.deltaTime));
    }

    private void Rotation() // 위치조정
    {
        if (isWalking || isRunning)
        {
            Vector3 _rotation = Vector3.Lerp(transform.eulerAngles, new Vector3(0f, direction.y, 0f), 0.01f);
            rigid.MoveRotation(Quaternion.Euler(_rotation));
        }
    }

    private void ElapseTime() // 경과시간
    {
        if (isAction)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
                ReSet();
        }
    }

    private void ReSet() // 리셋
    {
        isWalking = false; isRunning = false; isAction = true;
        applySpeed = walkSpeed;
        anim.SetBool("Walking", isWalking); anim.SetBool("Running", isRunning);
        direction.Set(0f, Random.Range(0f, 360f), 0f);
        RandomAction();
    }
    private void RandomAction() // 랜덤으로 행동실행
    {
        RandomSound();

        int _random = Random.Range(0, 4); // 대기, 풀뜯기, 두리번, 걷기

        if (_random == 0)
            Wait();
        else if (_random == 1)
            Eat();
        else if (_random == 2)
            Peek();
        else if (_random == 3)
            TryWalk();
    }

    private void Wait() // 대기
    {
        currentTime = waitTime;
        Debug.Log("대기");
    }
    private void Eat() // 풀뜯기
    {
        currentTime = waitTime;
        anim.SetTrigger("Eat");
        Debug.Log("풀뜯기");
    }
    private void Peek() // 두리번
    {
        currentTime = waitTime;
        anim.SetTrigger("Peek");
        Debug.Log("두리번");
    }
    private void TryWalk() // 걷기
    {
        isWalking = true;
        anim.SetBool("Walking", isWalking);
        currentTime = walkTime;
        applySpeed = walkSpeed;
        Debug.Log("걷기");
    }

    public void Run(Vector3 _targetPos) // 달리기
    {
        direction = Quaternion.LookRotation(transform.position - _targetPos).eulerAngles;

        currentTime = runTime;
        isWalking = false;
        isRunning = true;
        applySpeed = runSpeed;
        anim.SetBool("Running", isRunning);
    }

    public void Damage(int _dmg, Vector3 _targetPos) // 데미지
    {
        if (!isDead)
        {
            hp -= _dmg;

            if (hp <= 0)
            {
                Dead();
                return;
            }

            PlaySE(sound_pig_Hurt);
            anim.SetTrigger("Hurt");
            Run(_targetPos);
        }     
    }

    private void Dead() // 사망
    {
        PlaySE(sound_pig_Dead);
        isWalking = false;
        isRunning = false;
        isDead = true;
        anim.SetTrigger("Dead");
    }

    private void RandomSound() // 랜덤 사운드 재생
    {
        int _random = Random.Range(0, 3); // 일상 사운드 3개
        PlaySE(sound_pig_Normal[_random]);
    }

    private void PlaySE(AudioClip _clip) // 오디오 재생
    {
        theAudio.clip = _clip;
        theAudio.Play();
    }
}
