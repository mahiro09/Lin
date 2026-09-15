using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("チュートリアルUI")]
    public GameObject tutorialPanel;

    [Header("説明画像")]
    public Image tutorialImage;

    [Header("次へボタン")]
    public GameObject nextButton;

    [Header("チュートリアル画像")]
    public Sprite[] tutorialImages;

    [Header("移動開始から次へ進むまでの時間")]
    public float moveWaitTime = 2f;

    private int currentStep = 0;

    public static bool IsTutorialActive { get; private set; }

    public static int CurrentStep { get; private set; }

    private bool operationStarted = false;

    // WASDを押し始めたか
    private bool moveStarted = false;

    // WASDを押し始めてからの時間
    private float moveInputTimer = 0f;

    public static bool IsOperationStarted
    {
        get
        {
            if (Instance == null)
            {
                return false;
            }

            return Instance.operationStarted;
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentStep = 0;
        CurrentStep = 0;

        IsTutorialActive = true;

        operationStarted = false;

        moveStarted = false;
        moveInputTimer = 0f;

        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
        }

        ShowStep();
    }

    private void Update()
    {
        // チュートリアルが終了していたら何もしない
        if (!IsTutorialActive)
        {
            return;
        }

        // =====================================
        // Step 1
        // 「移動しよう！」
        // =====================================

        if (currentStep == 1 &&
            operationStarted)
        {
            CheckMoveInput();
        }
    }

    // =====================================
    // 次へボタン
    // =====================================

    public void NextStep()
    {
        // -------------------------------------
        // Step 1
        // 「移動しよう！」
        // ↓
        // 操作開始
        // -------------------------------------

        if (currentStep == 1)
        {
            StartOperation();
            return;
        }

        // -------------------------------------
        // Step 2
        // 「線を描こう！」
        // ↓
        // 操作開始
        // -------------------------------------

        if (currentStep == 2)
        {
            StartOperation();
            return;
        }

        // -------------------------------------
        // Step 4
        // 「橋を渡ろう！」
        // ↓
        // 操作開始
        // -------------------------------------

        if (currentStep == 4)
        {
            StartOperation();
            return;
        }

        // -------------------------------------
        // Step 5
        // 「インクは大切に！」
        // ↓
        // チュートリアル終了
        // -------------------------------------

        if (currentStep == 5)
        {
            FinishTutorial();
            return;
        }

        // -------------------------------------
        // その他
        // 次のStepへ
        // -------------------------------------

        currentStep++;

        CurrentStep = currentStep;

        operationStarted = false;

        moveStarted = false;
        moveInputTimer = 0f;

        if (currentStep >= tutorialImages.Length)
        {
            FinishTutorial();
            return;
        }

        ShowStep();
    }

    // =====================================
    // 操作開始
    // =====================================

    private void StartOperation()
    {
        operationStarted = true;

        moveStarted = false;
        moveInputTimer = 0f;

        // 次へボタンを消す
        if (nextButton != null)
        {
            nextButton.SetActive(false);
        }

        // 説明画像を消す
        if (tutorialImage != null)
        {
            tutorialImage.gameObject.SetActive(false);
        }

        Debug.Log(
            "操作開始 Step : " + currentStep
        );
    }

    // =====================================
    // Step 1
    // WASD操作確認
    // =====================================

    public void CheckMoveInput()
    {
        if (!IsTutorialActive)
        {
            return;
        }

        if (!operationStarted)
        {
            return;
        }

        if (currentStep != 1)
        {
            return;
        }

        // WASDを押しているか
        bool isMoving =
            Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.D);

        // -------------------------------------
        // WASDを押し始めた
        // -------------------------------------

        if (isMoving && !moveStarted)
        {
            moveStarted = true;
            moveInputTimer = 0f;

            Debug.Log(
                "WASD操作開始！"
            );
        }

        // -------------------------------------
        // WASDを押し始めてから時間を計測
        // -------------------------------------

        if (moveStarted)
        {
            moveInputTimer += Time.deltaTime;

            Debug.Log(
                "移動時間 : "
                + moveInputTimer.ToString("F1")
                + "秒"
            );

            // ---------------------------------
            // 2秒経過
            // ---------------------------------

            if (moveInputTimer >= moveWaitTime)
            {
                Debug.Log(
                    "2秒経過！次のチュートリアルへ"
                );

                currentStep = 2;

                CurrentStep = currentStep;

                operationStarted = false;

                moveStarted = false;
                moveInputTimer = 0f;

                ShowStep();
            }
        }
    }

    // =====================================
    // Step 2
    // 線を描いた
    // =====================================

    public void OnLineDrawn()
    {
        if (!IsTutorialActive)
        {
            return;
        }

        if (!operationStarted)
        {
            return;
        }

        if (currentStep != 2)
        {
            return;
        }

        Debug.Log(
            "チュートリアル：線を描きました！"
        );

        currentStep = 3;

        CurrentStep = currentStep;

        operationStarted = false;

        ShowStep();
    }

    // =====================================
    // Step 4
    // ゴール到達
    // =====================================

    public void OnGoalReached()
    {
        if (!IsTutorialActive)
        {
            return;
        }

        if (!operationStarted)
        {
            return;
        }

        if (currentStep != 4)
        {
            return;
        }

        Debug.Log(
            "チュートリアル：橋を渡ってゴールしました！"
        );

        currentStep = 5;

        CurrentStep = currentStep;

        operationStarted = false;

        ShowStep();
    }

    // =====================================
    // チュートリアル画像表示
    // =====================================

    private void ShowStep()
    {
        if (tutorialImage == null)
        {
            return;
        }

        if (tutorialImages == null ||
            tutorialImages.Length == 0)
        {
            return;
        }

        if (currentStep < 0 ||
            currentStep >= tutorialImages.Length)
        {
            FinishTutorial();
            return;
        }

        // 画像を表示
        tutorialImage.gameObject.SetActive(true);

        tutorialImage.sprite =
            tutorialImages[currentStep];

        // 次へボタンを表示
        if (nextButton != null)
        {
            nextButton.SetActive(true);
        }

        Debug.Log(
            "チュートリアル Step : "
            + currentStep
        );
    }

    // =====================================
    // チュートリアル終了
    // =====================================

    private void FinishTutorial()
    {
        IsTutorialActive = false;

        operationStarted = false;

        moveStarted = false;
        moveInputTimer = 0f;

        // チュートリアルパネルを消す
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }

        // 画像を消す
        if (tutorialImage != null)
        {
            tutorialImage.gameObject.SetActive(false);
        }

        // 次へボタンを消す
        if (nextButton != null)
        {
            nextButton.SetActive(false);
        }

        Debug.Log(
            "チュートリアル終了！"
        );

        // CLEAR画面を表示
        if (ClearManager.Instance != null)
        {
            ClearManager.Instance.Clear();
        }
    }
}