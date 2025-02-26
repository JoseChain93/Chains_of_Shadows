using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public CharacterStats stats; // Referencia a las estadísticas del jugador
    
    private Enemy targetEnemy; // Enemigo seleccionado
    public Enemy[] enemies; // Lista de enemigos
    public CombatManager2 combatManager2; // Referencia al CombatManager

    // Referencia al botón de ataque
    public Button attackButton;

    public Button disparoDeFeButton; // Referencia al botón de Disparo de Fe

    public Button defenseButton; // Referencia al botón de defensa
    public bool isDefending = false; // Bandera para saber si el jugador está defendiendo
    public Button healButton; // Botón para curarse

    private float healPercentage = 0.3f; // 30% de la vida máxima
    private int healManaCost = 20; // Coste de maná para usar la curación
    public Button escapeButton;  // El botón de escape

    // Marcadores para los enemigos
    public GameObject marcadorEnemy1; // Marcador para el enemigo 1
    public GameObject marcadorEnemy2; // Marcador para el enemigo 2

    public GameObject gameOver; //Pantalla Game Over

    private bool isGameOver = false; //Booleano para marcar estado game over
    private float attackReduction = 1.0f;  // Valor por defecto sin reducción
    private int attackReductionTurns = 0;  // Número de turnos restantes con reducción de ataque                                   
    private int bleedDamage = 0;  // Daño de sangrado por turno
    private int bleedTurns = 0;   // Número de turnos restantes con sangrado

    public bool isNecrosisActive = false;  // Cambiado a public
    public bool canHeal = true;  // Cambiado a public
    public int necrosisDamage = 5;
    public int necrosisTurnsLeft = 2;

    public AudioSource audioSource; // Fuente de audio para reproducir sonidos
    public AudioClip disparoDeFeSound; //Sonido de Disparo de Fé
    public AudioClip healSound;

    private void Start()
    {
        // Obtener el componente PlayerController desde el mismo objeto


        // Configurar el botón de ataque
        if (attackButton != null)
        {
            attackButton.onClick.AddListener(OnAttackButtonPressed); // Asignar la función al botón
        }

        enemies = FindObjectsOfType<Enemy>(); // Obtener todos los enemigos en la escena

        if (enemies.Length > 0)
        {
            targetEnemy = enemies[0];

            MoveMarkerToTarget(); // Mover el marcador al primer enemigo
        }

        if (PlayerStats.Instance != null)
        {
            stats = PlayerStats.Instance.stats;
        }
        else
        {
            Debug.LogError("No se encontró PlayerStats en la escena.");
            return;
        }

        // Configurar el botón de Disparo de Fe
        if (disparoDeFeButton != null)
        {
            disparoDeFeButton.onClick.AddListener(DisparoDeFe); // Asignar la función al botón
        }

        enemies = FindObjectsOfType<Enemy>(); // Obtener todos los enemigos en la escena

        if (enemies.Length > 0)
        {
            targetEnemy = enemies[0];

            MoveMarkerToTarget(); // Mover el marcador al primer enemigo
        }

        if (PlayerStats.Instance != null)
        {
            stats = PlayerStats.Instance.stats;
        }
        else
        {
            Debug.LogError("No se encontró PlayerStats en la escena.");
            return;
        }

        // Configurar la función al botón de defensa
        if (defenseButton != null)
        {
            defenseButton.onClick.AddListener(OnDefendButtonPressed); // Asignar la función al botón de defensa
        }

        // Configurar la función al botón curar
        if (healButton != null)
        {
            healButton.onClick.AddListener(Heal); // Asignar la función al botón
        }

        // Configurar el botón de huida
        if (escapeButton != null)
        {
            escapeButton.onClick.AddListener(TryToEscape);
        }


    }





    private void Update()
    {
        // Filtrar enemigos vivos
        enemies = FilterAliveEnemies(enemies);

        // Seleccionar el primer enemigo si se presiona la tecla 1
        if (Input.GetKeyDown(KeyCode.Alpha1) && enemies.Length > 0)
        {
            targetEnemy = enemies[0]; // Seleccionar el primer enemigo vivo
            Debug.Log("Objetivo cambiado a: " + targetEnemy.name);
            MoveMarkerToTarget(); // Mover el marcador
        }
        // Seleccionar el segundo enemigo si se presiona la tecla 2
        else if (Input.GetKeyDown(KeyCode.Alpha2) && enemies.Length > 1)
        {
            targetEnemy = enemies[1]; // Seleccionar el segundo enemigo vivo
            Debug.Log("Objetivo cambiado a: " + targetEnemy.name);
            MoveMarkerToTarget(); // Mover el marcador
        }

        // Si el objetivo está muerto, seleccionar automáticamente al siguiente enemigo
        if (targetEnemy != null && targetEnemy.health <= 0)
        {
            targetEnemy = GetNextAliveEnemy();
            if (targetEnemy != null)
            {
                Debug.Log("Objetivo cambiado automáticamente a: " + targetEnemy.name);
                MoveMarkerToTarget(); // Mover el marcador
            }
            else
            {
                // Si no hay enemigos vivos, desactiva ambos marcadores
                marcadorEnemy1.SetActive(false);
                marcadorEnemy2.SetActive(false);
            }

          
        }

        if (isGameOver && Input.GetKeyDown(KeyCode.Space))
        {
            // Reiniciar las estadísticas del jugador antes de cargar el menú
            if (stats != null)
            {
                stats.ResetStats();
            }

            // Forzar la última posición a ser la de respawn fijo
            ScenePositionEnterCombatData.ultimaPosicion = ScenePositionEnterCombatData.posicionRespawn;

            // Cargar el menú principal
            SceneManager.LoadScene(0);

            StartCoroutine(SetPlayerPositionAfterLoad());
        }
    }

    public void TakeDamage(int damage)
    {
        if (stats != null)
        {
            // Si está defendiendo, reducimos el daño a la mitad
            if (isDefending)
            {
                Debug.Log("¡Defensa activa! El daño recibido se ha reducido a " + damage);
                damage = Mathf.FloorToInt(damage * 0.5f); // Reducir el daño a la mitad

            }

            // Aplicamos el daño al jugador
            Debug.Log("¡El jugador ha recibido " + damage + " de daño! Salud restante: " + stats.currentHealth);
            stats.TakeDamage(damage);


            // Desactivamos la defensa después de recibir el daño
            isDefending = false;

            // Verificamos si el jugador ha muerto
            if (stats.currentHealth <= 0)
            {
                Die();
            }
        }
    }

    public void Attack()
    {
        targetEnemy = GetCurrentMarkedEnemy();

        if (targetEnemy != null && stats != null)
        {
            int reducedAttackPower = Mathf.FloorToInt(stats.attackPower * attackReduction);  // Aplica la reducción de ataque

            Debug.Log("¡El jugador ataca a " + targetEnemy.name + " e inflige " + stats.attackPower + " de daño!");
            targetEnemy.TakeDamage(stats.attackPower);
        }
    }


    // Método para lanzar la habilidad Disparo de Fe
    public void DisparoDeFe()
    {
        targetEnemy = GetCurrentMarkedEnemy();

        if (stats.currentMana >= 35) // Verificar si tiene suficiente maná
        {
            // Calcular el daño basado en la inteligencia del jugador
            int dañoBase = 30;
            int dañoEscalado = dañoBase + (stats.intelligence * 2); // Escalar daño con inteligencia

            // Asegurarse de que el daño no sea negativo
            dañoEscalado = Mathf.Max(dañoEscalado, 1);

            // Aplicar el daño al enemigo seleccionado
            if (targetEnemy != null)
            {
                audioSource.PlayOneShot(disparoDeFeSound);
                targetEnemy.TakeDamage(dañoEscalado);
                Debug.Log("¡Disparo de Fe inflige " + dañoEscalado + " de daño a " + targetEnemy.name + "!");
            }

            // Reducir maná
            stats.currentMana -= 35;
            Debug.Log("¡Disparo de Fe usado! Maná restante: " + stats.currentMana);

            // Actualizar la UI para mostrar el nuevo valor de maná
            PlayerStats.Instance.UpdateUI();
        }
        else
        {
            Debug.Log("¡No tienes suficiente maná para usar Disparo de Fe!");
        }
    }

    public bool IsDefending()
    {

        return isDefending;
    }

    public void Heal()
    {
    {   if (!canHeal)  // Si no puedes curarte debido a necrosis, no se puede curar
        {
            Debug.Log("¡No puedes curarte debido a necrosis!");
            return;
        }
        
        if (stats.currentMana >= healManaCost) // Verificar si tiene suficiente maná
        {
            int healAmount = Mathf.FloorToInt(stats.maxHealth * healPercentage); // 30% de la vida máxima

            // Restaurar salud sin exceder la vida máxima
            stats.currentHealth = Mathf.Min(stats.currentHealth + healAmount, stats.maxHealth);

            // Restar maná
            stats.currentMana -= healManaCost;

            Debug.Log("¡El jugador se ha curado " + healAmount + " puntos de salud! Salud actual: " + stats.currentHealth);
            audioSource.PlayOneShot(healSound);
            Debug.Log("Maná restante: " + stats.currentMana);

            // Actualizar la UI de vida y maná
            PlayerStats.Instance.UpdateUI();

            if (combatManager2 != null)
            {
                combatManager2.EnemyTurn();
            }
        }

    }
    
        
 }

   public void TryToEscape()
{
    // Obtener el nombre de la escena actual
    string currentScene = SceneManager.GetActiveScene().name;

    // Verificar si la escena actual contiene "BossCombat"
    if (currentScene.Contains("BossCombat"))
    {
        Debug.Log("No puedes escapar durante un combate contra el jefe.");
        return; // No permite escapar si es un combate contra el jefe
    }

    // Obtener la agilidad del jugador (usando PlayerStats.Instance para acceder a la instancia global de stats)
    int agilidadJugador = PlayerStats.Instance.stats.agility;

    // Calcular la probabilidad de escape, donde 0.5f es la probabilidad base
    // y se escala en función de la agilidad del jugador. Ajustamos para que una agilidad de 20
    // dé un 100% de probabilidad de escape.
    float probabilidadEscape = 0.5f + (agilidadJugador / 40f); // 40 es el factor de normalización para 100% a 20 de agilidad

    // Asegurarnos de que la probabilidad no se pase de 1 (100%)
    probabilidadEscape = Mathf.Min(probabilidadEscape, 1f);

    float resultado = Random.value; // Número aleatorio entre 0.0 y 1.0

    if (resultado <= probabilidadEscape)
    {
        Debug.Log("¡Escape exitoso!");

        // Verificar que la escena anterior no sea "SceneCombat" y si la última escena es "Mapa1_Dialogos"
        if (!string.IsNullOrEmpty(ScenePositionEnterCombatData.ultimaEscena) &&
            !ScenePositionEnterCombatData.ultimaEscena.Contains("SceneCombat"))
        {
            string previousScene = ScenePositionEnterCombatData.ultimaEscena;

             if (previousScene == "Mapa1_Dialogos")
                {
                    // Cargar la escena "Mapa1"
                    SceneManager.LoadScene("Mapa1");

                    // Esperar que la escena "Mapa1" se cargue y luego restaurar la posición
                    StartCoroutine(WaitForSceneToLoadAndRestorePosition("Mapa1"));
                }
                else if (previousScene == "Mapa2_Dialogos")
                {
                    // Cargar la escena "Mapa2_Solomon_Post_Servant"
                    SceneManager.LoadScene("Mapa2_Solomon_Post_Servant");

                    // Esperar que la escena "Mapa2" se cargue y luego restaurar la posición
                    StartCoroutine(WaitForSceneToLoadAndRestorePosition("Mapa2_Solomon_Post_Servant"));
                }
                else if (previousScene == "Mapa2_Solomon_Post_Servant"){
                    // Cargar la escena "Mapa2"
                    SceneManager.LoadScene("Mapa2");
                    StartCoroutine(WaitForSceneToLoadAndRestorePosition("Mapa2"));
                }
                else if (previousScene == "Mapa5"){
                    // Cargar la escena "Mapa2"
                    SceneManager.LoadScene("Mapa6");
                    StartCoroutine(WaitForSceneToLoadAndRestorePosition("Mapa6"));
                }
                else if (previousScene == "Mapa6"){
                    // Cargar la escena "Mapa2"
                    SceneManager.LoadScene("Mapa7");
                    StartCoroutine(WaitForSceneToLoadAndRestorePosition("Mapa7"));
                }
            else
            {
                // Si la escena anterior no es "Mapa1_Dialogos", cargar la última escena guardada
                SceneManager.LoadScene(previousScene);

                // Restaurar la posición guardada
                transform.position = ScenePositionEnterCombatData.ultimaPosicion;
            }

            // Restablecer la bandera de transición
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.isInTransition = false; // Permitir futuros combates
            }
        }
        else
        {
            Debug.LogError("No hay una escena anterior guardada o la escena guardada es un combate.");
        }
    }
    else
    {
        Debug.Log("¡No pudiste escapar! Los enemigos atacan.");

        // Llamar al turno del enemigo si el escape falla
        if (combatManager2 != null)
        {
            combatManager2.EnemyTurn();
        }
        else
        {
            Debug.LogError("El combatManager2 no está asignado. No se puede continuar el combate.");
        }
    }
}




    private Enemy[] FilterAliveEnemies(Enemy[] enemiesList)
    {
        // Filtrar los enemigos vivos
        List<Enemy> aliveEnemies = new List<Enemy>();
        foreach (Enemy enemy in enemiesList)
        {
            if (enemy != null && enemy.health > 0)
            {
                aliveEnemies.Add(enemy);
            }
        }
        return aliveEnemies.ToArray();
    }

    private Enemy GetNextAliveEnemy()
    {
        // Devolver el siguiente enemigo vivo
        foreach (Enemy enemy in enemies)
        {
            if (enemy != null && enemy.health > 0)
            {
                return enemy;
            }
        }
        return null;
    }

    // Función llamada cuando se presiona el botón de atacar
    private void OnAttackButtonPressed()
    {
        Attack(); // Llamar la función de ataque del jugador
        if (combatManager2 != null)
        {
            combatManager2.EnemyTurn(); // Los enemigos contraatacan después del ataque
        }
    }

    private void OnDefendButtonPressed()
    {
        isDefending = true; // Activar defensa
        Debug.Log("¡Defensa activada!");

        // Llamar a la función para que los enemigos ataquen
        if (combatManager2 != null)
        {
            combatManager2.EnemyTurn(); // Los enemigos atacan después de que el jugador defiende
        }
    }


    // Mover el marcador al enemigo seleccionado
    private void MoveMarkerToTarget()
    {
        // Asegúrate de que los marcadores estén desactivados al principio
        marcadorEnemy1.SetActive(false);
        marcadorEnemy2.SetActive(false);

        // Activar y mover el marcador correspondiente
        if (targetEnemy == enemies[0] && marcadorEnemy1 != null)
        {
            marcadorEnemy1.SetActive(true); // Activar marcador del enemigo 1

        }
        else if (targetEnemy == enemies[1] && marcadorEnemy2 != null)
        {
            marcadorEnemy2.SetActive(true); // Activar marcador del enemigo 2

        }
    }

    public void ReduceMana(int amount)
    {
        if (stats.currentMana >= amount)
        {
            stats.currentMana -= amount;
            Debug.Log("Maná reducido en " + amount + ". Maná restante: " + stats.currentMana);

            // Actualizar la UI del jugador después del cambio de maná
            PlayerStats.Instance.UpdateUI();
        }
        else
        {
            Debug.Log("No tienes suficiente maná para esta acción.");
        }
    }

    private Enemy GetCurrentMarkedEnemy()
    {
        foreach (Enemy enemy in enemies)
        {
            if (enemy != null)
            {
                // Si el marcador del enemigo está activo, devuelve ese enemigo
                if (marcadorEnemy1.activeSelf && enemy == enemies[0])
                    return enemies[0];

                if (marcadorEnemy2.activeSelf && enemy == enemies[1])
                    return enemies[1];
            }
        }

        return null; // Si no hay ningún marcador activo
    }

    public void ApplyBleedEffect(int damagePerTurn, int turns)
    {
        bleedDamage = damagePerTurn;  // Cantidad de daño por turno debido al sangrado
        bleedTurns = turns;  // Duración del sangrado
        StartCoroutine(BleedTimer());  // Llama a la corutina para aplicar el sangrado
    }

    private IEnumerator BleedTimer()
    {
        while (bleedTurns > 0)
        {
            yield return new WaitForSeconds(1);  // Espera 1 segundo por turno (ajustable)
            TakeDamage(bleedDamage);  // Aplica el daño por sangrado
            bleedTurns--;  // Reduce el número de turnos restantes
        }
    }

    public void ApplyAttackReduction(float reductionPercentage, int turns)
    {
        attackReduction = 1.0f - reductionPercentage;  // La reducción de ataque se calcula como el porcentaje (por ejemplo, 0.2 es 20%)
        attackReductionTurns = turns;  // Número de turnos de duración
        StartCoroutine(AttackReductionTimer());  // Llama a la corutina para gestionar la duración
    }

    private IEnumerator AttackReductionTimer()
    {
        while (attackReductionTurns > 0)
        {
            yield return new WaitForSeconds(1);  // Espera 1 segundo por turno (ajustable)
            attackReductionTurns--;  // Reduce el número de turnos restantes
        }

        // Una vez que los turnos se han acabado, restablece el valor original
        attackReduction = 1.0f;
    }

    // Método para aplicar Necrosis
    public void ApplyNecrosisEffect(int damagePerTurn, int duration)
    {
    isNecrosisActive = true;
    necrosisDamage = damagePerTurn;
    necrosisTurnsLeft = duration;
    canHeal = false;  // Bloquea la curación mientras dure la necrosis
    Debug.Log($"¡Necrosis aplicada! Recibirás {damagePerTurn} de daño por turno durante {duration} turnos.");
    }

    public void StartTurn()
    {
    if (isNecrosisActive && necrosisTurnsLeft > 0)
    {
        TakeDamage(necrosisDamage);
        necrosisTurnsLeft--;
        Debug.Log($"¡Necrosis inflige {necrosisDamage} de daño! ({necrosisTurnsLeft} turnos restantes)");

        if (necrosisTurnsLeft <= 0)
        {
            isNecrosisActive = false;
            canHeal = true;  // Permite curarse nuevamente
            Debug.Log("¡La necrosis se ha desvanecido!");
        }
    }
    }
    
    private void Die()
    {
         GameObject personaje = GameObject.Find("Solomon"); // Buscar al personaje "Solomon"
        if (personaje != null)
        {
        personaje.SetActive(false); // Desactivar el personaje
        }
        
        Debug.Log("El jugador ha muerto.");
        
        //Buscar el Canvas y desactivarlo
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
        canvas.SetActive(false);
        }

        // Activar la pantalla de Game Over
        if (gameOver != null)
        {
        gameOver.SetActive(true);
        isGameOver = true; // Marcar el estado de Game Over
        }
        else
        {
        Debug.LogError("No se ha asignado el objeto Game Over en el Inspector.");
        }

    }

    private IEnumerator SetPlayerPositionAfterLoad()
    {
        yield return new WaitForSeconds(0.1f);
        transform.position = ScenePositionEnterCombatData.ultimaPosicion;
    }

     // Coroutine para esperar a que la escena se cargue y restaurar la posición
    private IEnumerator WaitForSceneToLoadAndRestorePosition(string sceneName)
    {
        // Esperar hasta que la escena especificada esté activa (cargada)
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == sceneName);

        // Asegurarnos de que el personaje no se caiga
        if (transform.position.y < -5f) // Ajusta este valor según la altura de tu mapa
        {
            transform.position = new Vector3(transform.position.x, -3f, transform.position.z); // Coloca al personaje sobre el mapa
        }

        // Restaurar la posición después de cargar la escena
        transform.position = ScenePositionEnterCombatData.ultimaPosicion;
        Debug.Log("Posición restaurada en " + sceneName + ": " + transform.position);
    }
}