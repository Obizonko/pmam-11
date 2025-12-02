package main

import (
    "fmt"
    "math/rand"
    "time"
)

type Player struct {
    Name   string
    HP     int
    Attack int
    Defense int
    XP     int
    Inventory []string
}

type Enemy struct {
    Name   string
    HP     int
    Attack int
}

type Event struct {
    Description string
    Effect      func(p *Player)
}

func main() {
    rand.Seed(time.Now().UnixNano())

    player := Player{Name: "Герой", HP: 100, Attack: 15, Defense: 5, XP: 0}

    enemies := []Enemy{
        {"Скелет", 30, 5},
        {"Гоблін", 40, 7},
        {"Вовк", 25, 6},
    }

    events := []Event{
        {"Знайшли зілля здоров'я! +20 HP", func(p *Player){ p.HP += 20 }},
        {"Знайшли меч! +5 атаки", func(p *Player){ p.Attack += 5 }},
        {"Застрягли в пастці! -10 HP", func(p *Player){ p.HP -= 10 }},
    }

    fmt.Println("=== RPG Симулятор: Автоматична битва ===")

    for i := 1; i <= 5; i++ {
        fmt.Printf("\n--- Локація %d ---\n", i)
        encounter := rand.Intn(2)
        if encounter == 0 {
            enemy := enemies[rand.Intn(len(enemies))]
            fmt.Printf("Вас атакує: %s (HP: %d, Атака: %d)\n", enemy.Name, enemy.HP, enemy.Attack)
            fight(&player, enemy)
        } else {
            event := events[rand.Intn(len(events))]
            fmt.Println(event.Description)
            event.Effect(&player)
        }

        fmt.Printf("Статус гравця: HP=%d, Атака=%d, Захист=%d, XP=%d\n",
            player.HP, player.Attack, player.Defense, player.XP)
        if player.HP <= 0 {
            fmt.Println("Гравець загинув. Гра закінчена.")
            return
        }
    }

    fmt.Println("\nГра завершена! Ви вижили і здобули досвід:", player.XP)
}

func fight(player *Player, enemy Enemy) {
    for player.HP > 0 && enemy.HP > 0 {
        damageToEnemy := player.Attack - rand.Intn(3)
        if damageToEnemy < 0 { damageToEnemy = 0 }
        enemy.HP -= damageToEnemy
        fmt.Printf("Ви завдаєте %d шкоди %s (залишилось HP: %d)\n", damageToEnemy, enemy.Name, enemy.HP)

        if enemy.HP <= 0 {
            fmt.Println("Ворог переможений!")
            player.XP += 10
            return
        }

        damageToPlayer := enemy.Attack - player.Defense
        if damageToPlayer < 0 { damageToPlayer = 0 }
        player.HP -= damageToPlayer
        fmt.Printf("%s завдає %d шкоди вам (залишилось HP: %d)\n", enemy.Name, damageToPlayer, player.HP)
    }
}
