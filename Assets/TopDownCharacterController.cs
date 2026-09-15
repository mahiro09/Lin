using UnityEngine;

namespace Cainos.PixelArtTopDown_Basic
{
    public class TopDownCharacterController : MonoBehaviour
    {
        [Header("移動速度")]
        public float speed = 3f;

        [Header("風の壁判定")]
        public float windWallCheckDistance = 0.6f;
        public float windWallDistance = 0.3f;

        private Animator animator;
        private Rigidbody2D rb;

        // 風による速度
        private Vector2 windVelocity = Vector2.zero;

        private void Start()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            // ========================================
            // チュートリアル中の操作制限
            // ========================================

            if (TutorialManager.IsTutorialActive)
            {
                bool canMove = false;

                // 移動しよう
                if (TutorialManager.CurrentStep == 1)
                {
                    canMove = true;
                }

                // 線を描こう
                if (TutorialManager.CurrentStep == 2)
                {
                    canMove = true;
                }

                // 橋を渡ろう
                if (TutorialManager.CurrentStep == 4)
                {
                    canMove = true;
                }

                // 移動できないステップ
                if (!canMove)
                {
                    rb.velocity = Vector2.zero;

                    animator.SetBool(
                        "IsMoving",
                        false
                    );

                    return;
                }

                // 移動入力をチュートリアル側へ通知
                if (TutorialManager.CurrentStep == 1 ||
                    TutorialManager.CurrentStep == 4)
                {
                    TutorialManager.Instance.CheckMoveInput();
                }
            }

            // ========================================
            // WASD入力
            // ========================================

            Vector2 dir = Vector2.zero;

            if (Input.GetKey(KeyCode.A))
            {
                dir.x = -1;

                animator.SetInteger(
                    "Direction",
                    3
                );
            }
            else if (Input.GetKey(KeyCode.D))
            {
                dir.x = 1;

                animator.SetInteger(
                    "Direction",
                    2
                );
            }

            if (Input.GetKey(KeyCode.W))
            {
                dir.y = 1;

                animator.SetInteger(
                    "Direction",
                    1
                );
            }
            else if (Input.GetKey(KeyCode.S))
            {
                dir.y = -1;

                animator.SetInteger(
                    "Direction",
                    0
                );
            }

            // 斜め移動が速くならないようにする
            dir.Normalize();

            // ========================================
            // アニメーション
            // ========================================

            animator.SetBool(
                "IsMoving",
                dir.magnitude > 0
            );

            // ========================================
            // プレイヤーの通常移動
            // ========================================

            Vector2 normalVelocity =
                speed * dir;

            // ========================================
            // 風の移動
            // ========================================

            Vector2 wind = windVelocity;

            // 風の先に描いた線があるか確認
            if (IsWindBlocked())
            {
                // 壁に当たったら風だけ止める
                wind = Vector2.zero;
            }

            // ========================================
            // 通常移動 + 風
            // ========================================

            rb.velocity =
                normalVelocity + wind;
        }

        // ========================================
        // 風を設定
        // ========================================

        public void SetWindVelocity(
            Vector2 velocity)
        {
            windVelocity = velocity;
        }

        // ========================================
        // 風を解除
        // ========================================

        public void ClearWindVelocity()
        {
            windVelocity = Vector2.zero;
        }

        // ========================================
        // 風の進行方向に壁があるか
        // ========================================

        private bool IsWindBlocked()
        {
            // 風がなければ壁判定しない
            if (windVelocity == Vector2.zero)
            {
                return false;
            }

            Vector2 playerPosition =
                transform.position;

            Vector2 windDirection =
                windVelocity.normalized;

            // 風が進む方向の少し先を調べる
            Vector2 checkPosition =
                playerPosition +
                windDirection *
                windWallCheckDistance;

            // シーン内の描いた線を取得
            BridgeLine[] bridgeLines =
                FindObjectsOfType<BridgeLine>();

            foreach (BridgeLine bridge
                     in bridgeLines)
            {
                if (bridge == null)
                {
                    continue;
                }

                // 線とチェック位置の距離
                float distance =
                    bridge.GetDistanceToLine(
                        checkPosition
                    );

                // 線が風の進行方向にある
                if (distance <= windWallDistance)
                {
                    return true;
                }
            }

            return false;
        }
    }
}