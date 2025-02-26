using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int experienceReward = 50;
    public int maxHealth = 50;
    public int health = 50;        // Salud del enemigo
    public int attackDamage = 6;   // Daño de ataque normal
    public int lifeStealDamage = 8; // Daño del ataque Chupavidas
    public int lifeStealAmount = 5; // Cantidad de salud que el enemigo recupera al usar Chupavidas
    public bool isMother = false;   // Bandera para determinar si es el jefe "Mother"
    public bool isServant = false;  // Bandera para determinar si es el jefe "Servant"
    public bool isViktor=false; // Bandera para determinar si es el jefe "Viktor"
     public Player player;

    private bool isEnfurecida = false;
    private bool haCurado = false;
    private float enfurecerHealthThreshold = 0.5f; // Umbral para enfurecerse (solo para Bosses)

    public bool hasNecrosis = false;  // Si el enemigo tiene necrosis
    public int necrosisTurnsLeft = 0;  // Cuántos turnos le quedan a la necrosis
    public int necrosisDamage = 5;    // Daño de necrosis que inflige cada turno

    public bool hasDarkShield = false;  // Si el enemigo tiene el escudo de oscuridad
    public float darkShieldDamageReduction = 0.5f; // Reducción de daño cuando el escudo está activo
    public bool hasUsedSoulSuck = false;  // Bandera para verificar si Viktor ya usó Chupaalmas

    public AudioClip damageSound;      // Sonido al recibir daño
    public AudioClip attackSound;      // Sonido al infligir daño

    public AudioClip chupaalmasSound;
    public AudioClip necrocuchillaSound;
    private AudioSource audioSource;   // Fuente de audio
    public AudioClip muereIntrusoClip;
    public AudioClip healSound;

    void Start()
    {
    audioSource = GetComponent<AudioSource>();
    if (audioSource == null)
    {
        audioSource = gameObject.AddComponent<AudioSource>(); // Agrega uno si no existe
    }
    }

    public void StartTurn()
    {
        // Si el enemigo tiene necrosis, recibe daño y decrementa los turnos restantes
        if (hasNecrosis && necrosisTurnsLeft > 0)
        {
            TakeDamage(necrosisDamage);  // Inflige el daño de necrosis
            necrosisTurnsLeft--;         // Reduce los turnos de necrosis
            Debug.Log($"¡Necrosis inflige {necrosisDamage} de daño! ({necrosisTurnsLeft} turnos restantes)");

            // Si los turnos de necrosis se han acabado, quitar el efecto
            if (necrosisTurnsLeft <= 0)
            {
                hasNecrosis = false;
                Debug.Log("¡La necrosis se ha desvanecido!");
            }
        }
    }

    // Método para recibir daño
    public void TakeDamage(int damage)
    {
        // Si el escudo de oscuridad está activo reducir el daño
        if (hasDarkShield)
        {
        // Reducir el daño recibido por Viktor en un 75%
        damage = Mathf.FloorToInt(damage * (1 - darkShieldDamageReduction));
        Debug.Log($"¡Escudo de Oscuridad activado! Viktor recibe solo {damage} de daño.");
        }
        health -= damage;
        Debug.Log("¡" + gameObject.name + " ha recibido " + damage +  " de daño!");

        PlaySound(damageSound);  

        // Verificar si el enemigo ha muerto
        if (health <= 0)
        {
            Die();
        }
    }

    // Método de muerte del enemigo
    private void Die()
    {
        Debug.Log("¡" +gameObject.name + " ha muerto!");

        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.GainExp(experienceReward); // El jugador gana experiencia
        }

        StartCoroutine(BlinkBeforeDestroy());
    }

    // Método de parpadeo antes de destruir el enemigo
    private IEnumerator BlinkBeforeDestroy()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            for (int i = 0; i < 5; i++) // Parpadea 5 veces
            {
                spriteRenderer.enabled = false; // Ocultar
                yield return new WaitForSeconds(0.1f);
                spriteRenderer.enabled = true; // Mostrar
                yield return new WaitForSeconds(0.1f);
            }
        }

        Destroy(gameObject, 0.5f); // Finalmente, destruir el enemigo
    }

    // Método para determinar el ataque basado en si es Boss o normal
    public void AttackPlayer(Player player, bool useLifeSteal=false)
    {
        if (isMother || isServant || isViktor)  // Si es un Boss específico
        {
            BossAttack(player);
        }
        else
        {
            if (useLifeSteal)
            {
                // Lógica de ataque de chupavidas
                int damage = lifeStealDamage;

                if (player.IsDefending()) // Reducir daño si el jugador está defendiendo
                {
                    damage = Mathf.FloorToInt(damage * 0.5f); // Reducir daño a la mitad si está defendiendo
                    Debug.Log("¡El jugador está defendiendo! El daño de Chupavidas se reduce a " + damage);
                }

                // Aplicar daño al jugador
                player.TakeDamage(damage);

                // El enemigo recupera vida (sin reducción)
                health = Mathf.Min(health + lifeStealAmount, maxHealth);
                Debug.Log("¡" + gameObject.name + " usa Chupavidas! " +
                          "Inflige " + damage + " de daño y se cura " + lifeStealAmount + " puntos de salud.");
            }
            else
            {
                // Ataque normal
                int damage = attackDamage;

                if (player.IsDefending()) // Reducir daño si el jugador está defendiendo
                {
                    damage = Mathf.FloorToInt(damage * 0.5f); // Reducir daño a la mitad si está defendiendo
                    Debug.Log("¡El jugador está defendiendo! El daño recibido se reduce a " + damage);
                }

                // Aplicar daño al jugador
                player.TakeDamage(damage);
                PlaySound(attackSound);
                Debug.Log("¡" + gameObject.name + " ataca al jugador e inflige " + damage + " de daño!");
            }
        }
    }

    // Ataque básico para enemigos normales
    private void AttackPlayerNormal(Player player)
    {
        int damage = attackDamage;

        if (player.IsDefending()) // Reducir daño si el jugador está defendiendo
        {
            damage = Mathf.FloorToInt(damage * 0.5f); // Reducir daño a la mitad si está defendiendo
            Debug.Log("¡El jugador está defendiendo! El daño recibido se reduce a " + damage);
        }
        PlaySound(attackSound); 
        player.TakeDamage(damage);
        Debug.Log($"¡{gameObject.name} ataca al jugador e inflige {damage} de daño!");
    }

    // Ataques especiales para Bosses (Mother o Servant)
    private void BossAttack(Player player)
    {
        if (isMother)
        {
         if (health <= maxHealth * enfurecerHealthThreshold && !isEnfurecida)
            {
            EntrarEnFuria();
            }

            float rng = Random.value; // Generamos un número aleatorio

            if (isEnfurecida && rng < 0.2f)
            {
            QuemarMana(player);
         }
            else if (rng < 0.5f)
            {
            AttackPlayerBasic(player);
            }
            else if (rng < 0.8f)
            {
            MordiscoVoraz(player);
            }
            else
            {
            GarraCruel(player);
            }
        }

        else if (isServant)
        {
        if (health <= maxHealth * 0.5f && !haCurado)
        {
        Curarse();
        attackDamage = Mathf.CeilToInt(attackDamage * 1.5f);
        }

        float rng = Random.value; // Generamos un número aleatorio entre 0 y 1

        if (rng < 0.5f)
        {
        
        AttackPlayerBasic(player);
        
        }
        else
        {
        MuereIntrusoMuere(player);
        }
        }
        else if (isViktor)
        {
        if (health <= maxHealth * 0.3f && !hasDarkShield)
        {
        hasDarkShield = true;  // Activar el escudo de oscuridad
        Debug.Log("¡Viktor invoca la oscuridad cercana para crear un escudo protector! Reduce el daño recibido en un 50%");
        }
        else if (health <= maxHealth * 0.3f && !hasUsedSoulSuck)
        {
        // Si la salud es 30% o menos y Viktor no ha usado Chupaalmas, lo usa
        Chupaalmas(player);
        }
        else
        {
        float rng = Random.value; // Generamos un número aleatorio entre 0 y 1

        if (rng < 0.5f)
        {
            AttackPlayerBasic(player);
        }
        else
        {
            Necrocuchilla(player);
        }
        }
    }
    }
    
    // Método para entrar en furia (solo Mother)
    private void EntrarEnFuria()
    {
        isEnfurecida = true;
        attackDamage = Mathf.CeilToInt(attackDamage * 1.5f);  // Aumenta el daño al entrar en furia
        Debug.Log($"{gameObject.name} ha entrado en furia! Daño aumentado.");
    }

    // Ataque básico de Boss
    private void AttackPlayerBasic(Player player)
    {
        int damage = attackDamage;
        if (player.IsDefending())
        {
            damage = Mathf.FloorToInt(damage * 0.5f); // Reducir daño si el jugador está defendiendo
            Debug.Log("¡El jugador está defendiendo! El daño recibido se reduce a " + damage);
        }

        player.TakeDamage(damage);
        PlaySound(attackSound);
        Debug.Log($"{gameObject.name} ataca al jugador e inflige {damage} de daño!");
    }

    // Ataque especial: Mordisco Voraz
    private void MordiscoVoraz(Player player)
    {
        int damage = Random.Range(10, 22);
        if (player.IsDefending())
        {
            damage = Mathf.FloorToInt(damage * 0.5f); // Reducir daño si el jugador está defendiendo
            Debug.Log("¡El jugador está defendiendo! El daño recibido se reduce a " + damage);
        }
        player.TakeDamage(damage);
        PlaySound(attackSound);
        if (Random.value < 0.2f)
        {
            player.ApplyAttackReduction(0.2f, 1);  // Reduce el ataque en un 20% durante 1 turno
            Debug.Log("¡Mordisco Voraz aplicado! Reducción de ataque del jugador.");
        }
    }

    // Ataque especial: Garra Cruel
    private void GarraCruel(Player player)
    {
        int damage = Random.Range(10, 18);
        if (player.IsDefending())
        {
            damage = Mathf.FloorToInt(damage * 0.5f); // Reducir daño si el jugador está defendiendo
            Debug.Log("¡El jugador está defendiendo! El daño recibido se reduce a " + damage);
        }
        player.TakeDamage(damage);
        PlaySound(attackSound);
        if (Random.value < 0.2f)
        {
            player.ApplyBleedEffect(5, 3);  // Aplica 5 de daño por turno durante 3 turnos
            Debug.Log("¡Sangrado aplicado por Garra Cruel!");
        }
    }

    // Ataque especial: Quemar Mana
    private void QuemarMana(Player player)
    {
        int manaRobado = 30;
        PlaySound(attackSound);
        player.ReduceMana(manaRobado);
        Debug.Log($"{gameObject.name} ha usado Quemar Mana! Roba {manaRobado} de mana del jugador.");
    }

    private void PlaySound(AudioClip clip)
    {
    if (clip != null && audioSource != null)
    {
        audioSource.PlayOneShot(clip); // Reproduce el clip sin interrumpir otros sonidos
    }
    }

    // Método para curarse una vez (solo Servant)
    private void Curarse()
    {
    int cantidadCura = (int)(maxHealth * 0.5f);
    health += cantidadCura;

    if (health > maxHealth)
    {
        health = maxHealth; // Evita que sobrepase el máximo de vida
    }
    
    haCurado = true; // Evita que se cure más de una vez
    Debug.Log("El Sirviente se injecta un chute de adrenalina en vena, se recupera " + cantidadCura + " puntos de vida y aumenta su daño");
    }

    // Ataque especial: MuereIntrusoMuere(solo Servant)
    void MuereIntrusoMuere(Player player)
    {

        int damage = Random.Range(8, 10);

    // Si la vida del jugador es menor al 50%, aumenta el daño en un 30%
    if (player.stats.currentHealth < player.stats.maxHealth * 0.5f)
    {
        damage = Mathf.FloorToInt(damage * 1.3f);
        Debug.Log("¡El jugador está debilitado! El Servant aprovecha y aumenta su daño a " + damage);
    }

    // Si el jugador está defendiendo, se reduce el daño a la mitad
    if (player.IsDefending())
    {
        damage = Mathf.FloorToInt(damage * 0.5f);
        Debug.Log("¡El jugador está defendiendo! El daño recibido se reduce a " + damage);
    }

    // Aplicar daño al jugador
    player.TakeDamage(damage);

    // Probabilidad del 40% de aplicar sangrado
    if (Random.value < 0.3f) // 40% de probabilidad
    {
        player.ApplyBleedEffect(5, 2); // Aplica 10 de daño por turno durante 2 turnos
        Debug.Log("¡El Sirviente grita: Muere intruso! Muere! y dispara a quemarropa, infligiendo " + damage + " y causando sangrado.");
    }
    else
    {
        Debug.Log("¡El Sirviente grita: Muere intruso! Muere! y dispara, infligiendo " + damage + " de daño!");
    }

    audioSource.PlayOneShot(muereIntrusoClip);
    }

     // Necrochuchilla (solo Viktor)
    private void Necrocuchilla(Player player)
    {
    int damage = Random.Range(10, 15); // Daño entre 10 y 15
    int probabilidadNecrosis = Random.Range(0, 100); // Probabilidad entre 0 y 100

    // Inflige Necrosis con un 30% de probabilidad
    if (probabilidadNecrosis < 30) // Cambié el 20 a 30 para un 30% de probabilidad
    {
        if (!player.isNecrosisActive) // Solo aplica necrosis si no está ya activa
        {
            player.isNecrosisActive = true;  // Activa necrosis en el jugador
            player.canHeal = false;  // Bloquea la curación
            player.necrosisDamage = 5;  // Daño por necrosis
            player.necrosisTurnsLeft = 2; // La necrosis dura 2 turnos
            Debug.Log($"{gameObject.name} ha infligido Necrosis al jugador!");
        }
    }

    // Verificar si el jugador está defendiendo
    if (player.IsDefending())
    {
        damage = Mathf.FloorToInt(damage * 0.5f); // Reduce el daño a la mitad si está defendiendo
    }

    // Inflige daño
    player.TakeDamage(damage);
     PlaySound(necrocuchillaSound);
    Debug.Log($"¡Necrocuchilla inflige {damage} de daño!");

    // Si la necrosis está activa, infligir daño por necrosis
    if (player.isNecrosisActive)
    {
        ApplyNecrosisDamage(player);
    }
    }

    // Método para aplicar el daño por necrosis durante el turno
    private void ApplyNecrosisDamage(Player player)
    {
    if (player.necrosisTurnsLeft > 0)
    {
        player.TakeDamage(player.necrosisDamage); // Inflige daño de necrosis
        player.necrosisTurnsLeft--; // Disminuye los turnos restantes de necrosis
        Debug.Log($"¡Necrosis inflige {player.necrosisDamage} de daño! Turnos restantes: {player.necrosisTurnsLeft}");

        // Cuando la necrosis termina, se permite la curación nuevamente
        if (player.necrosisTurnsLeft == 0)
        {
            player.isNecrosisActive = false;  // Desactiva la necrosis
            player.canHeal = true;  // Permite curarse de nuevo
            Debug.Log("¡La necrosis ha desaparecido!");
        }
    }
    }

    //Chupaalmas (Solo Viktor)
    private void Chupaalmas(Player player)
    {
    int damage = Random.Range(10, 15); // El daño infligido por Chupaalmas
    int healAmount = Mathf.FloorToInt(damage * 1f); // Viktor se cura un 100% del daño infligido

    if (player.IsDefending()) 
    {
        damage = Mathf.FloorToInt(damage * 0.5f);  // Reducir daño si el jugador está defendiendo
        Debug.Log("¡El jugador está defendiendo! El daño de Chupaalmas se reduce a " + damage);
    }

    // Aplicar daño al jugador
    player.TakeDamage(damage);
    PlaySound(chupaalmasSound);

    
    // Viktor se cura
    health = Mathf.Min(health + healAmount, maxHealth);
    Debug.Log($"¡Viktor absorve tu energia vital ejecutando Chupaalmas e inflige {damage} de daño y Viktor se cura {healAmount} de salud!");

    // Marcar que Viktor ya usó la habilidad
    hasUsedSoulSuck = true;

    // Reproducir el sonido de la habilidad (si lo deseas)
    PlaySound(attackSound);
    }

}

