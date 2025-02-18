using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody playerRigidbody; // Rigidbody 组件
    public float moveSpeed = 5f;      // 移动速度
    public float jumpForce = 5f;      // 跳跃力度
    public float dashMultiplier = 3f; // 冲刺速度倍数
    public float rayLength = 0.6f;    // 射线检测长度
    public Transform cameraTransform; // 摄像机的 Transform

    private bool isGrounded = false;  // 是否在地面
    private bool isDashing = false;   // 是否正在冲刺
    private int jumpCount = 0;        // 跳跃次数

    public int score = 0; // 记录得分
    public ScoreManager scoreManager;


    void Start()
    {
        // 如果没有手动赋值 Rigidbody，尝试自动获取
        if (playerRigidbody == null)
        {
            playerRigidbody = GetComponent<Rigidbody>();
        }

        // 如果没有手动赋值摄像机，使用主摄像机
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        scoreManager = FindAnyObjectByType<ScoreManager>();
    }

    void Update()
    {

        //float mouseX = Input.GetAxis("Mouse X");
        //transform.Rotate(0, mouseX, 0); // 控制角色左右旋转，旋转速度可以调整

        // 让角色面向摄像机的反方向
        Vector3 cameraPosition = cameraTransform.position;
        Vector3 playerPosition = transform.position;

        // 计算面朝摄像机的方向
        Vector3 directionToCamera = (cameraPosition - playerPosition).normalized;

        // 计算背对摄像机的方向（朝相反方向）
        Vector3 lookDirection = -new Vector3(directionToCamera.x, 0, directionToCamera.z); // 忽略 y 轴，防止角色倾斜

        // 使角色旋转到背对摄像机的方向
        if (lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }

        // 获取输入
        Vector2 inputVector = Vector2.zero;
        if (Input.GetKey(KeyCode.W))
        {
            inputVector += Vector2.up;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputVector += Vector2.down;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputVector += Vector2.right;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputVector += Vector2.left;
        }

        // 将输入转换为 3D 平面移动方向（基于摄像机方向）
        Vector3 moveDirection = cameraTransform.forward * inputVector.y + cameraTransform.right * inputVector.x;
        moveDirection.y = 0; // 确保角色不会在垂直方向上移动
        moveDirection.Normalize(); // 归一化方向，防止斜向移动速度过快

        // 冲刺逻辑
        if (Input.GetKeyDown(KeyCode.E))
        {
            isDashing = true;
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            isDashing = false;
        }

        // 计算移动速度
        float currentSpeed = isDashing ? moveSpeed * dashMultiplier : moveSpeed;
        playerRigidbody.linearVelocity = new Vector3(moveDirection.x * currentSpeed, playerRigidbody.linearVelocity.y, moveDirection.z * currentSpeed);

        // 地面检测
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.down * 0.5f; // 确保射线从角色的脚底发射
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, out hit, rayLength);

        // 如果角色在地面上，重置跳跃次数
        if (isGrounded)
        {
            jumpCount = 0;
        }

        // 调试信息：显示地面检测结果
        Debug.DrawLine(rayOrigin, rayOrigin + Vector3.down * rayLength, isGrounded ? Color.green : Color.red);
        Debug.Log("Is Grounded: " + isGrounded);

        // 跳跃控制
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (jumpCount < 2) // 允许二段跳
            {
                playerRigidbody.linearVelocity = new Vector3(playerRigidbody.linearVelocity.x, 0, playerRigidbody.linearVelocity.z); // 清除 Y 轴速度，确保跳跃高度一致
                playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                jumpCount++;
                Debug.Log("Jump! Jump Count: " + jumpCount);
            }
            else
            {
                Debug.Log("Max Jump Reached");
            }
        }
    }

    void OnDrawGizmos()
    {
        // 绘制地面检测射线
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * rayLength);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin")) // 检查是否碰到硬币
        {

            scoreManager.AddScore(1);
            score++; // 分数+1
            Debug.Log("Score: " + score); // 输出分数到控制台

            Destroy(other.gameObject); // 销毁硬币
        }
    }
}

