using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    // ���������� ��������� ����������, ������������ � ����
    [SerializeField] private Image attackTimerClock; // ����������� ��� ����������� ������� ����
    [SerializeField] private Image harvestTimerClock; // ����������� ��� ����������� ������� ����� ������
    [SerializeField] private Text cycleText; // ����� ��� ����������� ���������� ������
    [SerializeField] private Text grainText; // ����� ��� ����������� ���������� �����
    [SerializeField] private Text farmersText; // ����� ��� ����������� ���������� ��������
    [SerializeField] private Text warriorsText; // ����� ��� ����������� ���������� ������
    [SerializeField] private Text nextEnemiesText; // ����� ��� ����������� ���������� ��������� ������
    [SerializeField] private Text surviveStatsText; // ����� ��� ����������� ���������� �������� ����
    [SerializeField] private Text generatedGrainText; // ����� ��� ����������� ���������������� �����
    [SerializeField] private Text spentGrainText; // ����� ��� ����������� ������������ �����
    [SerializeField] private Text hiredWarriorsText; // ����� ��� ����������� ���������� ������� ������
    [SerializeField] private Button hireFarmerButton; // ������ ��� ����� �������
    [SerializeField] private Button hireWarriorButton; // ������ ��� ����� �����
    [SerializeField] private Button pauseButton; // ������ ��� ����� ����
    [SerializeField] private GameObject gameOverScreen; // ����� ����� ����
    [SerializeField] private GameObject winScreen; // ����� ������
    [SerializeField] private GameObject pauseScreen; // ����� �����
    [SerializeField] private Button restartButtonWin; // ������ ��� ����������� ���� ����� ������
    [SerializeField] private Button restartButtonLose; // ������ ��� ����������� ���� ����� ���������
    [SerializeField] private Button continueButton; // ������ ��� ����������� ���� ����� �����
    [SerializeField] private Button toggleSoundButton; // ������ ��� ���������/���������� �����
    [SerializeField] private Button restartButtonPause; // ������ ��� ����������� ���� �� ������ �����

    [SerializeField] private AudioSource buttonClickSound;
    [SerializeField] private AudioSource grainHarvestSound;
    [SerializeField] private AudioSource hireWarriorSound;
    [SerializeField] private AudioSource hireFarmerSound;
    [SerializeField] private AudioSource attackSound;
    [SerializeField] private AudioSource backgroundMusic;

    // ���������� ��������� ���������� ��� �������� ��������� ����
    [SerializeField] private int cycle = 0; // ���������� ������
    [SerializeField] private int grain = 10; // ���������� �����
    [SerializeField] private int farmers = 1; // ���������� ��������
    [SerializeField] private int warriors = 0; // ���������� ������
    [SerializeField] private int enemies = 2; // ���������� ������ � ��������� �����
    [SerializeField] private int survivedAttacks = 0; // ���������� �������� ����
    [SerializeField] private int generatedGrain = 0; // ���������� ���������������� �����
    [SerializeField] private int spentGrain = 0; // ���������� ������������ �����
    [SerializeField] private int hiredWarriors = 0; // ���������� ������� ������
    [SerializeField] private bool isPaused = false; // ����, �����������, ��������� �� ���� �� �����
    [SerializeField] private bool soundEnabled = true; // ����, �����������, ������� �� ����
    [SerializeField] private float attackTimer = 20f; // ������ ��� �����
    [SerializeField] private float harvestTimer = 4f; // ������ ��� ����� ������

    // ���������� ��������� �������� ��������
    private float initialAttackTimer;
    private float initialHarvestTimer;
    private bool isMusicPlaying = true;

    // �����, ���������� ��� ������ ����
    private void Start()
    {
        initialAttackTimer = attackTimer;
        initialHarvestTimer = harvestTimer;

        // ���������� ����������
        UpdateUI();

        // �������� ������� � �������
        hireFarmerButton.onClick.AddListener(() => { HireFarmer(); PlayButtonClickSound(); });
        hireWarriorButton.onClick.AddListener(() => { HireWarrior(); PlayButtonClickSound(); });
        pauseButton.onClick.AddListener(() => { TogglePause(); PlayButtonClickSound(); });
        restartButtonWin.onClick.AddListener(() => { RestartGame(); PlayButtonClickSound(); });
        restartButtonLose.onClick.AddListener(() => { RestartGame(); PlayButtonClickSound(); });
        continueButton.onClick.AddListener(() => { TogglePause(); PlayButtonClickSound(); });
        restartButtonPause.onClick.AddListener(() => { RestartGame(); PlayButtonClickSound(); });
        toggleSoundButton.onClick.AddListener(() => { ToggleSound(); PlayButtonClickSound(); });

        // ��������������� ������� ������
        backgroundMusic.Play();
    }

    // �����, ���������� ������ ����
    private void Update()
    {
        // ���� ���� �� �����, ���������� ����������
        if (isPaused)
            return;

        // ���������� ������� ����� � ����� ������
        attackTimer -= Time.deltaTime;
        harvestTimer -= Time.deltaTime;

        // ���������� ������������� ����������� ��������
        attackTimerClock.fillAmount = attackTimer / 20f;
        harvestTimerClock.fillAmount = harvestTimer / 4f;

        // ����� ������ ����� ��������� ����
        if (attackTimer <= 0)
        {
            AttackVillage();
            attackTimer = 20f;
        }

        // ����� ������ ����� ������ ��������� ����
        if (harvestTimer <= 0)
        {
            HarvestGrain();
            harvestTimer = 4f;
        }

        // ���������� ����������
        UpdateUI();
    }

    // ����� ��� ����� �������
    private void HireFarmer()
    {
        if (grain >= 3)
        {
            grain -= 3;
            farmers++;
            spentGrain += 3;
            PlayHireFarmerSound();
            UpdateUI();
            CheckWin();
        }
    }

    // ����� ��� ����� �����
    private void HireWarrior()
    {
        if (grain >= 5)
        {
            grain -= 5;
            warriors++;
            spentGrain += 5;
            hiredWarriors++;
            PlayHireWarriorSound();
            UpdateUI();
        }
    }

    // ����� ��� ����� ������
    private void HarvestGrain()
    {
        grain += farmers * 4;
        generatedGrain += farmers * 4;
        PlayGrainHarvestSound();
        UpdateUI();
    }

    // ����� ��� ����� �������
    private void AttackVillage()
    {
        PlayAttackSound();
        if (warriors >= enemies)
        {
            survivedAttacks++;
            warriors -= enemies;
            enemies += 2;
        }
        else
        {
            GameOver();
        }
        UpdateUI();
    }

    // ����� ��� ���������� ����������
    private void UpdateUI()
    {
        cycleText.text = cycle.ToString();
        grainText.text = grain.ToString();
        farmersText.text = farmers.ToString();
        warriorsText.text = warriors.ToString();
        nextEnemiesText.text = enemies.ToString();
        surviveStatsText.text = survivedAttacks.ToString();
        generatedGrainText.text = generatedGrain.ToString();
        spentGrainText.text = spentGrain.ToString();
        hiredWarriorsText.text = hiredWarriors.ToString();

        hireFarmerButton.interactable = grain >= 3;
        hireWarriorButton.interactable = grain >= 5;
    }

    // ����� ��� ��������� ����� ����
    private void GameOver()
    {
        gameOverScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    // ����� ��� ����������� ����
    private void RestartGame()
    {
        cycle = 0;
        grain = 10;
        farmers = 1;
        warriors = 0;
        enemies = 2;
        survivedAttacks = 0;
        generatedGrain = 0;
        spentGrain = 0;
        hiredWarriors = 0;

        attackTimer = initialAttackTimer;
        harvestTimer = initialHarvestTimer;

        Time.timeScale = 1f;
        isPaused = false;
        soundEnabled = true;

        gameOverScreen.SetActive(false);
        winScreen.SetActive(false);
        pauseScreen.SetActive(false);

        UpdateUI();
    }

    // ����� ��� �������� ������� ������
    private void CheckWin()
    {
        if (farmers >= 100 && grain >= 500)
        {
            winScreen.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    // ����� ��� ���������/���������� �����
    private void TogglePause()
    {
        isPaused = !isPaused;
        pauseScreen.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    // ����� ��� ���������/���������� �����
    private void ToggleSound()
    {
        if (isMusicPlaying)
        {
            backgroundMusic.Pause();
        }
        else
        {
            backgroundMusic.Play();
        }
        isMusicPlaying = !isMusicPlaying;
    }

    private void PlayButtonClickSound()
    {
        if (soundEnabled)
        {
            buttonClickSound.Play();
        }
    }

    private void PlayGrainHarvestSound()
    {
        if (soundEnabled)
        {
            grainHarvestSound.Play();
        }
    }

    private void PlayHireWarriorSound()
    {
        if (soundEnabled)
        {
            hireWarriorSound.Play();
        }
    }

    private void PlayHireFarmerSound()
    {
        if (soundEnabled)
        {
            hireFarmerSound.Play();
        }
    }

    private void PlayAttackSound()
    {
        if (soundEnabled)
        {
            attackSound.Play();
        }
    }
}
