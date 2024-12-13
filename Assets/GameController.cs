using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    // Объявление различных переменных, используемых в игре
    [SerializeField] private Image attackTimerClock; // Изображение для отображения таймера атак
    [SerializeField] private Image harvestTimerClock; // Изображение для отображения таймера сбора урожая
    [SerializeField] private Text cycleText; // Текст для отображения количества циклов
    [SerializeField] private Text grainText; // Текст для отображения количества зерна
    [SerializeField] private Text farmersText; // Текст для отображения количества фермеров
    [SerializeField] private Text warriorsText; // Текст для отображения количества воинов
    [SerializeField] private Text nextEnemiesText; // Текст для отображения количества следующих врагов
    [SerializeField] private Text surviveStatsText; // Текст для отображения статистики выживших атак
    [SerializeField] private Text generatedGrainText; // Текст для отображения сгенерированного зерна
    [SerializeField] private Text spentGrainText; // Текст для отображения потраченного зерна
    [SerializeField] private Text hiredWarriorsText; // Текст для отображения количества нанятых воинов
    [SerializeField] private Button hireFarmerButton; // Кнопка для найма фермера
    [SerializeField] private Button hireWarriorButton; // Кнопка для найма воина
    [SerializeField] private Button pauseButton; // Кнопка для паузы игры
    [SerializeField] private GameObject gameOverScreen; // Экран конца игры
    [SerializeField] private GameObject winScreen; // Экран победы
    [SerializeField] private GameObject pauseScreen; // Экран паузы
    [SerializeField] private Button restartButtonWin; // Кнопка для перезапуска игры после победы
    [SerializeField] private Button restartButtonLose; // Кнопка для перезапуска игры после поражения
    [SerializeField] private Button continueButton; // Кнопка для продолжения игры после паузы
    [SerializeField] private Button toggleSoundButton; // Кнопка для включения/выключения звука
    [SerializeField] private Button restartButtonPause; // Кнопка для перезапуска игры из экрана паузы

    [SerializeField] private AudioSource buttonClickSound;
    [SerializeField] private AudioSource grainHarvestSound;
    [SerializeField] private AudioSource hireWarriorSound;
    [SerializeField] private AudioSource hireFarmerSound;
    [SerializeField] private AudioSource attackSound;
    [SerializeField] private AudioSource backgroundMusic;

    // Объявление приватных переменных для хранения состояния игры
    [SerializeField] private int cycle = 0; // Количество циклов
    [SerializeField] private int grain = 10; // Количество зерна
    [SerializeField] private int farmers = 1; // Количество фермеров
    [SerializeField] private int warriors = 0; // Количество воинов
    [SerializeField] private int enemies = 2; // Количество врагов в следующей атаке
    [SerializeField] private int survivedAttacks = 0; // Количество выживших атак
    [SerializeField] private int generatedGrain = 0; // Количество сгенерированного зерна
    [SerializeField] private int spentGrain = 0; // Количество потраченного зерна
    [SerializeField] private int hiredWarriors = 0; // Количество нанятых воинов
    [SerializeField] private bool isPaused = false; // Флаг, указывающий, находится ли игра на паузе
    [SerializeField] private bool soundEnabled = true; // Флаг, указывающий, включен ли звук
    [SerializeField] private float attackTimer = 20f; // Таймер для атаки
    [SerializeField] private float harvestTimer = 4f; // Таймер для сбора урожая

    // Сохранение начальных значений таймеров
    private float initialAttackTimer;
    private float initialHarvestTimer;
    private bool isMusicPlaying = true;

    // Метод, вызываемый при старте игры
    private void Start()
    {
        initialAttackTimer = attackTimer;
        initialHarvestTimer = harvestTimer;

        // Обновление интерфейса
        UpdateUI();

        // Привязка функций к кнопкам
        hireFarmerButton.onClick.AddListener(() => { HireFarmer(); PlayButtonClickSound(); });
        hireWarriorButton.onClick.AddListener(() => { HireWarrior(); PlayButtonClickSound(); });
        pauseButton.onClick.AddListener(() => { TogglePause(); PlayButtonClickSound(); });
        restartButtonWin.onClick.AddListener(() => { RestartGame(); PlayButtonClickSound(); });
        restartButtonLose.onClick.AddListener(() => { RestartGame(); PlayButtonClickSound(); });
        continueButton.onClick.AddListener(() => { TogglePause(); PlayButtonClickSound(); });
        restartButtonPause.onClick.AddListener(() => { RestartGame(); PlayButtonClickSound(); });
        toggleSoundButton.onClick.AddListener(() => { ToggleSound(); PlayButtonClickSound(); });

        // Воспроизведение фоновой музыки
        backgroundMusic.Play();
    }

    // Метод, вызываемый каждый кадр
    private void Update()
    {
        // Если игра на паузе, прекращаем обновление
        if (isPaused)
            return;

        // Уменьшение таймера атаки и сбора урожая
        attackTimer -= Time.deltaTime;
        harvestTimer -= Time.deltaTime;

        // Обновление заполненности изображений таймеров
        attackTimerClock.fillAmount = attackTimer / 20f;
        harvestTimerClock.fillAmount = harvestTimer / 4f;

        // Когда таймер атаки достигает нуля
        if (attackTimer <= 0)
        {
            AttackVillage();
            attackTimer = 20f;
        }

        // Когда таймер сбора урожая достигает нуля
        if (harvestTimer <= 0)
        {
            HarvestGrain();
            harvestTimer = 4f;
        }

        // Обновление интерфейса
        UpdateUI();
    }

    // Метод для найма фермера
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

    // Метод для найма воина
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

    // Метод для сбора урожая
    private void HarvestGrain()
    {
        grain += farmers * 4;
        generatedGrain += farmers * 4;
        PlayGrainHarvestSound();
        UpdateUI();
    }

    // Метод для атаки деревни
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

    // Метод для обновления интерфейса
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

    // Метод для обработки конца игры
    private void GameOver()
    {
        gameOverScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    // Метод для перезапуска игры
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

    // Метод для проверки условия победы
    private void CheckWin()
    {
        if (farmers >= 100 && grain >= 500)
        {
            winScreen.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    // Метод для включения/выключения паузы
    private void TogglePause()
    {
        isPaused = !isPaused;
        pauseScreen.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    // Метод для включения/выключения звука
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
