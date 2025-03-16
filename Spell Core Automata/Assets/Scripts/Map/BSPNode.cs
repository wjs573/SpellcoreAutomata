using UnityEngine;

public class BSPNode
{
    public BSPNode leftChild;  // 左子节点
    public BSPNode rightChild; // 右子节点
    public RectInt space;      // 当前节点代表的3D区域（XZ平面）
    public Room room;          // 生成的房间数据
    public int depth;          // 节点深度

    public bool IsLeaf => leftChild == null && rightChild == null;
}