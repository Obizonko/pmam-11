package main

import (
	"bufio"
	"fmt"
	"os"
	"strconv"
	"strings"

	"orderqueue/models"
	"orderqueue/services"
)

func main() {
	queue, err := services.NewOrderQueue()
	if err != nil {
		fmt.Println("Помилка завантаження даних:", err)
		return
		return
	}

	scanner := bufio.NewScanner(os.Stdin)

	for {
		fmt.Println("\nOrder Queue")
		fmt.Println(strings.Repeat("=", 55))
		total, p1, p2, p3 := queue.Stats()
		fmt.Printf("В черзі: %d замовлень (Термінових: %d | Високих: %d | Звичайних: %d)\n", total, p1, p2, p3)
		fmt.Println(strings.Repeat("=", 55))
		fmt.Println("1. Додати замовлення")
		fmt.Println("2. Обробити наступне")
		fmt.Println("3. Показати всю чергу")
		fmt.Println("4. Пошук за ім’ям")
		fmt.Println("5. Пошук за ID")
		fmt.Println("6. Редагувати замовлення")
		fmt.Println("7. Видалити за ID")
		fmt.Println("8. Статистика")
		fmt.Println("0. Вихід")
		fmt.Print("\nВиберіть дію → ")

		if !scanner.Scan() {
			break
		}
		choice := strings.TrimSpace(scanner.Text())

		switch choice {
		case "1":
			addOrder(scanner, queue)
		case "2":
			processNext(queue)
		case "3":
			showAll(queue)
		case "4":
			searchByName(scanner, queue)
		case "5":
			searchByID(scanner, queue)
		case "6":
			editOrder(scanner, queue)
		case "7":
			deleteOrder(scanner, queue)
		case "8":
			showStats(queue)
		case "0":
			fmt.Println("\nДо зустрічі!")
			return
		default:
			fmt.Println("Невірна команда, спробуйте ще раз.")
		}
	}
}

func addOrder(s *bufio.Scanner, q *services.OrderQueue) {
	fmt.Print("Ім’я клієнта: ")
	s.Scan()
	name := strings.TrimSpace(s.Text())
	if name == "" {
		fmt.Println("Скасовано.")
		return
	}

	fmt.Print("Товар: ")
	s.Scan()
	item := strings.TrimSpace(s.Text())

	fmt.Print("Пріоритет (1–3): ")
	s.Scan()
	prio, _ := strconv.Atoi(strings.TrimSpace(s.Text()))

	q.Add(name, item, prio)
	fmt.Println("Замовлення успішно додано!")
}

func processNext(q *services.OrderQueue) {
	o := q.Pop()
	if o == nil {
		fmt.Println("Черга порожня.")
	} else {
		fmt.Printf("Оброблено [#%d] %s — %s (пріор. %d) [%s]\n",
			o.ID, o.CustomerName, o.Item, o.Priority, o.CreatedAt)
	}
}

func showAll(q *services.OrderQueue) {
	list := q.List()
	if len(list) == 0 {
		fmt.Println("Черга порожня.")
		return
	}
	fmt.Printf("%-4s %-10s %-12s %-20s %s\n", "ID", "Пріоритет", "Дата", "Клієнт", "Товар")
	fmt.Println(strings.Repeat("-", 80))
	for _, o := range list {
		prio := []string{"", "ТЕРМІНОВО", "ВИСОКИЙ", "ЗВИЧАЙНИЙ"}[o.Priority]
		fmt.Printf("%-4d %-10s %-12s %-20s %s\n",
			o.ID, prio, o.CreatedAt[:10], o.CustomerName, o.Item)
	}
}

func searchByName(s *bufio.Scanner, q *services.OrderQueue) {
	fmt.Print("Частина імені: ")
	s.Scan()
	found := q.FindByCustomer(s.Text())
	if len(found) == 0 {
		fmt.Println("Нічого не знайдено.")
	} else {
		printOrders(found)
	}
}

func searchByID(s *bufio.Scanner, q *services.OrderQueue) {
	fmt.Print("ID: ")
	s.Scan()
	id, _ := strconv.Atoi(strings.TrimSpace(s.Text()))
	o := q.FindByID(id)
	if o == nil {
		fmt.Println("Замовлення не знайдено.")
	} else {
		printOrders([]models.Order{*o})
	}
}

func editOrder(s *bufio.Scanner, q *services.OrderQueue) {
	fmt.Print("ID для редагування: ")
	s.Scan()
	id, err := strconv.Atoi(strings.TrimSpace(s.Text()))
	if err != nil || q.FindByID(id) == nil {
		fmt.Println("Невірний ID.")
		return
	}

	fmt.Print("Нове ім’я клієнта (Enter — без змін): ")
	s.Scan()
	name := strings.TrimSpace(s.Text())

	fmt.Print("Новий товар (Enter — без змін): ")
	s.Scan()
	item := strings.TrimSpace(s.Text())

	fmt.Print("Новий пріоритет 1-3 (Enter — без змін): ")
	s.Scan()
	prioStr := strings.TrimSpace(s.Text())
	prio := 0
	if prioStr != "" {
		prio, _ = strconv.Atoi(prioStr)
	}

	if q.Edit(id, name, item, prio) {
		fmt.Println("Замовлення оновлено та чергу пересортовано.")
	}
}

func deleteOrder(s *bufio.Scanner, q *services.OrderQueue) {
	fmt.Print("ID для видалення: ")
	s.Scan()
	id, _ := strconv.Atoi(strings.TrimSpace(s.Text()))
	if q.DeleteByID(id) {
		fmt.Println("Замовлення видалено.")
	} else {
		fmt.Println("Замовлення не знайдено.")
	}
}

func showStats(q *services.OrderQueue) {
	total, p1, p2, p3 := q.Stats()
	fmt.Printf("\nСтатистика черги:\n")
	fmt.Printf("  Всього:      %d\n", total)
	fmt.Printf("  Термінові:   %d\n", p1)
	fmt.Printf("  Високий:     %d\n", p2)
	fmt.Printf("  Звичайні:    %d\n", p3)
}

func printOrders(orders []models.Order) {
	for _, o := range orders {
		prio := []string{"", "ТЕРМІНОВО", "ВИСОКИЙ", "ЗВИЧАЙНИЙ"}[o.Priority]
		fmt.Printf("[#%d] %s — %s | %s | %s\n", o.ID, o.CustomerName, o.Item, prio, o.CreatedAt)
	}
}