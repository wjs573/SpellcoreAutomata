using Cinemachine;
using UnityEngine;

public class CinemachineFollow : MonoBehaviour
{
    public GameObject followCharacter;
    public GameObject HexMapGrid;
    private CinemachineVirtualCamera virtualCamera;
    public GameObject CM;

    private Vector3 cameraStartPosition; // 用于记录摄像头的初始位置
    public float panSpeed = 20f; // 鼠标拖拽或边缘滚动平移速度
    private Vector3 dragOrigin; // 用于记录鼠标拖拽的起点
    public bool allowDrag = true; // 是否允许拖拽

    public float scrollSpeed = 10f; // 缩放速度
    public float minZoom = 10f; // 最小缩放
    public float maxZoom = 60f; // 最大缩放
    public float edgeScrollThickness = 10f; // 边缘滚动触发的距离

    private bool isCameraLocked = false; // 是否锁定相机移动和缩放
    private void Start()
    {
        virtualCamera = CM.GetComponent<CinemachineVirtualCamera>();
        if (!virtualCamera)
        {
            virtualCamera = CM.AddComponent<CinemachineVirtualCamera>();
        }

        // 记录摄像头的初始位置
        cameraStartPosition = virtualCamera.transform.position;
    }

    public void SetFollowCharacter(GameObject cha)
    {
        followCharacter = cha;
        virtualCamera.Follow = cha.transform; // 设置虚拟摄像机的跟随对象
        if (!cha)
        {
            // 停止跟随并将摄像头位置设置回初始位置
            virtualCamera.Follow = null;
            virtualCamera.transform.position = cameraStartPosition;
            return;
        }
    }

    private void Update()
    {
        // 检查是否按下 L 键来切换锁定/解锁状态
        if (Input.GetKeyDown(KeyCode.L))
        {
            isCameraLocked = !isCameraLocked;
            Debug.Log("Camera Lock Toggled: " + (isCameraLocked ? "Locked" : "Unlocked"));
        }
    }

    private void LateUpdate()
    {
        cameraStartPosition = virtualCamera.transform.position;

        // 如果相机锁定，禁止移动和缩放操作
        if (isCameraLocked)
        {
            return;
        }

        // 如果没有要跟随的角色，启用鼠标拖拽、边缘滚动和缩放
        if (!followCharacter)
        {
            HandleMouseDragMovement(); // 鼠标拖拽移动
            HandleEdgeScroll(); // 边缘滚动
            HandleScrollZoom(); // 滚轮缩放
        }
        // else
        // {
        //     virtualCamera.transform.parent.position = new Vector3(
        //         followCharacter.transform.position.x,
        //         0,
        //         followCharacter.transform.position.z);
        // }
    }

    // 鼠标拖拽相机移动
    private void HandleMouseDragMovement()
    {
        if (Input.GetMouseButtonDown(1)) // 按下鼠标右键
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(1)) return; // 鼠标右键未按下时不移动

        Vector3 difference = Input.mousePosition - dragOrigin;
        Vector3 move = new Vector3(-difference.x * panSpeed * Time.deltaTime, 0, -difference.y * panSpeed * Time.deltaTime);

        // 通过平移相机实现鼠标拖动
        virtualCamera.transform.Translate(move, Space.World);

        // 获取相机当前位置
        Vector3 currentPosition = virtualCamera.transform.position;

        // 限制相机的 X 和 Z 轴范围在 0 到 30 之间
        currentPosition.x = Mathf.Clamp(currentPosition.x, 0, 30);
        currentPosition.z = Mathf.Clamp(currentPosition.z, 0, 30);

        // 更新相机位置
        virtualCamera.transform.position = currentPosition;

        // 更新鼠标拖拽起点
        dragOrigin = Input.mousePosition;
    }

    // 实现屏幕边缘滚动控制相机
    private void HandleEdgeScroll()
    {
        Vector3 move = Vector3.zero;

        if (Input.mousePosition.x >= Screen.width - edgeScrollThickness)
        {
            move.x += panSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.x <= edgeScrollThickness)
        {
            move.x -= panSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.y >= Screen.height - edgeScrollThickness)
        {
            move.z += panSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.y <= edgeScrollThickness)
        {
            move.z -= panSpeed * Time.deltaTime;
        }

        // 先移动相机
        virtualCamera.transform.Translate(move, Space.World);

        // 获取相机当前位置
        Vector3 currentPosition = virtualCamera.transform.position;

        // 限制相机的 X 和 Z 轴范围在 0 到 30 之间
        currentPosition.x = Mathf.Clamp(currentPosition.x, -5, 30);
        currentPosition.z = Mathf.Clamp(currentPosition.z, -5, 30);

        // 更新相机位置
        virtualCamera.transform.position = currentPosition;
    }

    // 实现鼠标滚轮缩放功能
    private void HandleScrollZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            // 控制相机的距离（缩放效果）
            virtualCamera.m_Lens.FieldOfView -= scroll * scrollSpeed;
            virtualCamera.m_Lens.FieldOfView = Mathf.Clamp(virtualCamera.m_Lens.FieldOfView, minZoom, maxZoom);
        }
    }
}