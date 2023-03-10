using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_move : MonoBehaviour
{
    Animator anime;
    Camera camera;
    CharacterController controller;

    public float walk_speed = 5f;
    public float run_speed = 9f;
    public float jump_force =7f;

    float apply_spped;

    public float turnSmoothing = 0.5f;

    public bool toggle_camera_rotation; //배그 알트 기능 또는 다크소울 타겟팅 카메라;
    public float smoothness = 10f;

    //블락 한번만
    int b_num;

    //이동 값
    float f_num;
    float r_num;
    float f_num_look;
    float r_num_look;

    float break_time;

    //애니 시간 값
    float temp = 1;
    float cur_temp = 1;
    float block_temp = 1;

    //중력
    float gravity = 20f;

    bool is_run;
    bool is_atk;

    //이동 백터값
    Vector3 move_dir;
    Vector3 look_dir;
    
    void Start()
    {
        anime = FindObjectOfType<Animator>();
        camera = Camera.main;
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetMouseButtonDown(2))
        {
            toggle_camera_rotation = !toggle_camera_rotation;
            //Debug.Log("dd");
        }
        //p_jump();
        p_move();
        p_attack();
        p_block();
        
    }
    void p_jump()
    {
        if (controller.isGrounded)
        {
            Debug.Log("6666");
            if (Input.GetButton("Jump"))
            {
                move_dir.y = jump_force;
            }
        }
        else
        {
            Debug.Log("213");
        }
    }

    void p_move()
    {
        apply_spped = is_run ? run_speed : walk_speed;
        
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if(controller.velocity == Vector3.zero)
            {
                anime.SetFloat("is_run", 0f);
                is_run = false;
                //return;
            }
            else
            {
                anime.SetFloat("is_run", 1f);
                is_run = true;
            }
        }
        else
        {
            anime.SetFloat("is_run", 0f);
            is_run = false;
        }

        //배그 용
        //tronformdirection = 로컬에서 월드로 방향을 바꿔줌
        //Vector3 forward = transform.TransformDirection(Vector3.forward);
        //Vector3 right = transform.TransformDirection(Vector3.right);

        //다크소울 용
        Vector3 look_forward = new Vector3(camera.transform.forward.x, 0, camera.transform.forward.z).normalized;
        Vector3 look_right = new Vector3(camera.transform.right.x, 0, camera.transform.right.z).normalized;


        f_num = Input.GetAxisRaw("Vertical");
        r_num = Input.GetAxisRaw("Horizontal");

        f_num_look = Input.GetAxisRaw("Vertical");
        r_num_look = Input.GetAxisRaw("Horizontal");

        move_dir = look_forward * f_num + look_right * r_num;
        look_dir = look_forward * f_num_look + look_right * r_num_look;

        move_dir.y -= gravity * Time.deltaTime;
        //움직임
        controller.Move(move_dir.normalized * apply_spped * Time.deltaTime);


        if (!toggle_camera_rotation)
        {
            if (f_num != 0 || r_num != 0)
            {
                break_time = 0;

                // 부드럽게 돌기(바라보기)
                Vector3 player_rotate = Vector3.Scale(look_dir, new Vector3(1, 0, 1));
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(player_rotate), Time.deltaTime * smoothness); 

                anime.SetBool("is_walk", true);
                anime.SetBool("is_break", false);
                anime.SetInteger("is_walk_int", 1);
            }
            else
            {
                anime.SetBool("is_walk", false);
                if (break_time <= 4)
                {
                    break_time += Time.deltaTime;
                }
                else if( break_time >4 && break_time <= 7)
                {
                    break_time += Time.deltaTime;
                    anime.SetBool("is_break", true);   
                }
                else
                {
                    anime.SetBool("is_break", false);
                    break_time = 0;
                }
            }
        }
        else if (f_num != 0 || r_num != 0)
        {
            break_time = 0;
            anime.SetBool("is_break", false);
            anime.SetBool("is_walk", true);
            if (f_num == 1)
            {
                anime.SetInteger("is_walk_int", 1);
            }
            else if(f_num==-1)
            {
                anime.SetInteger("is_walk_int", 4);
            }

            if(r_num == 1)
            {
                anime.SetInteger("is_walk_int", 3);
            }
            else if(r_num == -1)
            {
                anime.SetInteger("is_walk_int", 2);
            }
        }
        else
        {
            anime.SetBool("is_walk", false);
            if (break_time <= 4)
            {
                break_time += Time.deltaTime;
            }
            else if (break_time > 4 && break_time <= 7)
            {
                break_time += Time.deltaTime;
                anime.SetBool("is_break", true);
            }
            else
            {
                anime.SetBool("is_break", false);
                break_time = 0;
            }
        }
    }

    void p_attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            temp = 1f;
            cur_temp = 1.4f;
            is_atk = true;
            break_time = 0;
            b_num = 1;
            anime.SetLayerWeight(1, 1);
            anime.SetTrigger("do_atk");
            anime.SetBool("is_break", false);
            
        }
        if (is_atk)
        {
            if (temp > 0)
            {
                temp -= Time.deltaTime; //딜레이
            }
            else
            {
                if(cur_temp > 0)
                {
                    cur_temp -= Time.deltaTime;
                }
                else
                {
                    is_atk = false;
                }
                anime.SetLayerWeight(1, cur_temp);
                
            }

        }
    }
    
    void p_block()
    {
        if (Input.GetMouseButton(1))
        {
            //is_block = true;
            anime.SetBool("is_blocking", true);
            anime.SetLayerWeight(1, 1);
            b_num = 0;
            block_temp = 1;
        }
        else
        {
            if(b_num < 1)
            {
                anime.SetBool("is_blocking", false);
                if (block_temp > 0)
                {
                    block_temp -= Time.deltaTime;
                }
                else
                {
                    b_num++;
                }
                anime.SetLayerWeight(1, block_temp);
            }
        }
    }

    
    
    
}
