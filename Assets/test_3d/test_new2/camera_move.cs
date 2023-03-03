using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class camera_move : MonoBehaviour
{
    public Transform player_obj;
    public Transform obj_enemy;
    public float follow_speed=10; //�ӵ�
    [Range(100, 800)]public float mouse_sensitivity= 300f; //����
    public float clamp_angle = 70f; // ����

    float rot_x;
    float rot_y;

    public int _num;

    public Transform real_camera;
    public Vector3 dir_normalized; //����/
    public Vector3 final_dir; //���� ��ġ
    public float min_distance; //�ּ� �Ÿ�
    public float max_distance;
    public float final_distance;
    public float smoothness = 10;

    player_move player;
    void Start()
    {
        player = FindObjectOfType<player_move>();
        //�ʱ�ȭ
        rot_x = transform.localRotation.eulerAngles.x;
        rot_y = transform.localRotation.eulerAngles.y;

        dir_normalized = real_camera.localPosition.normalized; // ��ֶ����� ũ�Ⱑ 0�̵Ǽ� ���⸸ �����
        final_distance = real_camera.localPosition.magnitude; // magnitude�� ������ ũ�⸦ �ǹ��� 

        Cursor.lockState = CursorLockMode.Locked; //���콺 ����
        Cursor.visible = false; //���콺 ������
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("x   " + rot_x + "y    "+ rot_y);
        
        if (player.toggle_camera_rotation)
        {
            return;
        }
        
        //���콺 ������ �ݴ�� �޴´�.
        rot_x += -(Input.GetAxis("Mouse Y")) * mouse_sensitivity * Time.deltaTime; //���Ʒ��� �ݴ뿩�� 
        rot_y += Input.GetAxis("Mouse X") * mouse_sensitivity * Time.deltaTime;

        //clamp ������ �����̶�� ���̴�
        rot_x = Mathf.Clamp(rot_x, -clamp_angle, clamp_angle); // x���� ,�ּ�(70��), �ִ�(70��)

        //ȸ������ �Լ�
        Quaternion rot = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rot_x, player.toggle_camera_rotation? 0:rot_y, 0), smoothness *Time.deltaTime);
        
        transform.rotation = rot;
    }
    private void LateUpdate()
    {
        if (player.toggle_camera_rotation)
        {
            Vector3 target_en = Vector3.Scale(obj_enemy.position, new Vector3(1, 0, 1));

            Vector3 direction = obj_enemy.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, smoothness * Time.deltaTime);

            rot_x = -(rotation.eulerAngles.x); //��
            rot_y = rotation.eulerAngles.y;
            
            player.transform.LookAt(target_en);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, player_obj.position, follow_speed * Time.deltaTime);
        }
        transform.position = Vector3.MoveTowards(transform.position, player_obj.position, follow_speed * Time.deltaTime);


        //transformpoint = ���� �����ǿ��� ���� ���������� �ٲ���
        final_dir = transform.TransformPoint(dir_normalized * max_distance);



        //ī�޶� �� ĳ���� �ڿ� ��ü�� ������
        RaycastHit hit;
        //�������� �׸��ٴ� �� (�ּ�, �ִ�Ÿ�)
        if(Physics.Linecast(transform.position, final_dir, out hit)) //���� �ִ�.
        {
            final_distance = Mathf.Clamp(hit.distance, min_distance, max_distance);
        }
        else
        {
            final_distance = max_distance;
        }
        real_camera.localPosition = Vector3.Lerp(real_camera.localPosition, dir_normalized * final_distance, Time.deltaTime * smoothness);
    }
}
