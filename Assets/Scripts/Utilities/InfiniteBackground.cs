using System.Collections;
using UnityEngine;

namespace Vampire
{
    /// <summary>
    /// 用一块 Quad + Shader 实现“无限卷轴”背景。
    /// 功能点：
    /// 1. 根据主摄视口大小自动拉伸到正好覆盖屏幕（Awake）
    /// 2. 每当玩家移动超过 resetDistance，平移背景贴图（ResetBackground 协程）
    /// 3. 支持中心向外扩散的冲击波特效（Shockwave 协程）
    /// 
    /// Shader 需暴露以下属性：
    /// _ResetOffset   : 已累计的偏移量（Vector2）
    /// _TempResetOffset: 本次重置的临时偏移（Vector2）
    /// _ResetBlend    : 临时 → 累计 的插值因子 (0→1)
    /// _Resetting     : 是否处于重置过渡期 (int 0/1)
    /// _Shockwave     : 冲击波距离 (float)
    /// _PlayerPosition: 玩家世界坐标 (Vector3)
    /// </summary>
    [RequireComponent(typeof(MeshRenderer))]
    public class InfiniteBackground : MonoBehaviour
    {
        // 玩家 Transform，用于获取位置
        private Transform playerTransform;

        // 指定的背景材质（使用同一张 Shader）
        [SerializeField] private Material backgroundMaterial;

        /* ========== 卷轴相关状态 ========== */

        // 上一次执行 ResetBackground 时玩家所在的位置
        private Vector2 previousResetPosition = Vector2.zero;
        // 累积的贴图偏移量，最终写回 _ResetOffset
        private Vector2 resetOffset = Vector2.zero;

        // 玩家离上一次重置点的距离超过该值就触发 Reset
        [SerializeField] private float resetDistance = 15f;
        // ResetBackground 过渡耗时
        [SerializeField] private float resetDuration = 5f;

        /* ---------- 生命周期 ---------- */

        private void Awake()
        {
            /* ① 让背景 Quad 充满摄像机视口
             *   Viewport (0,0) = 左下角像素
             *   Viewport (1,1) = 右上角像素
             *   nearClipPlane  用于把 Viewport 点投射到世界坐标
             */
            Vector2 bottomLeft = Camera.main.ViewportToWorldPoint(
                new Vector3(0, 0, Camera.main.nearClipPlane));
            Vector2 topRight = Camera.main.ViewportToWorldPoint(
                new Vector3(1, 1, Camera.main.nearClipPlane));

            Vector3 screenSizeWorldSpace = new Vector3(
                topRight.x - bottomLeft.x,  // 宽
                topRight.y - bottomLeft.y,  // 高
                1);                         // Z 方向保持 1

            transform.localScale = screenSizeWorldSpace;   // Quad 尺寸拉到整屏
            GetComponent<MeshRenderer>().sharedMaterial = backgroundMaterial;
        }

        /// <summary>
        /// 外部注入背景纹理 + 玩家 Transform。
        /// 必须在场景加载后由 GameManager 调用一次。
        /// </summary>
        public void Init(Texture2D backgroundTexture, Transform playerTransform)
        {
            this.playerTransform = playerTransform;

            // 绑定贴图并重置 Shader 关键参数
            backgroundMaterial.mainTexture = backgroundTexture;
            backgroundMaterial.SetFloat("_Shockwave", 0);
            resetOffset = Vector2.zero;
            backgroundMaterial.SetVector("_ResetOffset", resetOffset);
            backgroundMaterial.SetInt("_Resetting", 0);
        }

        /* ========== ① 冲击波效果 ========== */

        /// <summary>
        /// 在 Shader 内部从玩家位置发出冲击波：
        ///   _Shockwave 随时间 d 线性变大，Shader 里用 d 控制 Fresnel/Mask
        /// </summary>
        public IEnumerator Shockwave(float distance)
        {
            float d = 0;
            while (d < distance)
            {
                d += Time.deltaTime * 16;                   // 速度因子可调
                backgroundMaterial.SetFloat("_Shockwave", d);
                backgroundMaterial.SetVector("_PlayerPosition", playerTransform.position);
                yield return null;
            }
            backgroundMaterial.SetFloat("_Shockwave", 0);   // 归零以便下次触发
        }

        /* ========== ② 无限卷轴逻辑 ========== */

        private void Update()
        {
            // 玩家离上一次 Reset 点的位移向量
            Vector2 toReset = previousResetPosition - (Vector2)playerTransform.position;

            // 当距离平方超过阈值²，开始平滑 reset
            if (toReset.sqrMagnitude > resetDistance * resetDistance)
            {
                StartCoroutine(ResetBackground(toReset));
                previousResetPosition = playerTransform.position;   // 新的基准点
            }
        }

        /// <summary>
        /// 平滑更新 resetOffset，以实现“贴图搬家”而背景 Quad 本身不动。
        /// </summary>
        private IEnumerator ResetBackground(Vector2 toReset)
        {
            backgroundMaterial.SetInt("_Resetting", 1);
            backgroundMaterial.SetVector("_TempResetOffset", toReset);

            float t = 0;
            while (t < resetDuration)
            {
                t += Time.deltaTime;
                backgroundMaterial.SetFloat("_ResetBlend", t / resetDuration);
                yield return null;
            }

            // 过渡完成：把本次偏移量累加到总偏移量
            resetOffset += toReset;
            backgroundMaterial.SetVector("_ResetOffset", resetOffset);
            backgroundMaterial.SetInt("_Resetting", 0);
        }
    }
}
