using UnityEngine;

/// <summary>
/// 玩家
/// </summary>
public class Player : MonoBehaviour
{
    /// <summary>
    /// 角色控制器
    /// </summary>
    private CharacterController controller;

    /// <summary>
    /// 玩家移动速度
    /// </summary>
    public float speed = 5f;

    /// <summary>
    /// 重力
    /// </summary>
    public float gravity = -15f;

    /// <summary>
    /// 速度方向
    /// </summary>
    Vector3 velocity;


    private void Awake()
    {
        controller = transform.GetComponent<CharacterController>();
    }

    private float _timer;
    // Update is called once per frame
    void Update()
    {
        //获取玩家键盘的输入 wsad
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //玩家移动方向
        Vector3 move = transform.right * x + transform.forward * z;

        //玩家进行移动
        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space) && _timer <= 0)
        {
            velocity.y = 5;
            _timer = 1;
        }

        if (_timer > 0)
            _timer -= Time.deltaTime;
        if (_timer < 0)
            _timer = 0;
        //重力
        velocity.y += gravity * Time.deltaTime;
        //重力方向移动
        controller.Move(velocity * Time.deltaTime);


        //射击
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("shut");
            //从屏幕中心方向发射一条射线
            var ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;
            //射线如果碰撞到物体
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.collider.gameObject.name);
            }
        }
    }


    /// <summary>
    /// 触发碰撞体
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        //如果碰撞到的物体是钥匙
        if (other.name == "Key")
        {
            Debug.Log("获取钥匙");
        }
    }
}