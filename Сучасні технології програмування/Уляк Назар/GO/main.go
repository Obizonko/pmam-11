package main

import (
	"bufio"
	"fmt"
	"log"
	"net"
)

// --- MODEL ---
type client chan<- string // Канал для відправки повідомлень клієнту

var (
	entering = make(chan client)
	leaving  = make(chan client)
	messages = make(chan string) // Всі повідомлення від клієнтів
)

// --- BROADCASTER (Центральний вузол) ---
// Ця функція працює в окремій горутині та керує всіма клієнтами
func broadcaster() {
	clients := make(map[client]bool) // Список активних клієнтів
	for {
		// select - це як switch, але для каналів. Чекає на першу подію.
		select {
		case msg := <-messages:
			// Розсилаємо повідомлення всім
			for cli := range clients {
				cli <- msg
			}
		case cli := <-entering:
			clients[cli] = true
		case cli := <-leaving:
			delete(clients, cli)
			close(cli)
		}
	}
}

// --- CLIENT HANDLER ---
func handleConn(conn net.Conn) {
	ch := make(chan string) // Вихідний канал для цього клієнта
	go clientWriter(conn, ch)

	who := conn.RemoteAddr().String()
	ch <- "You are " + who + "\n"
	messages <- who + " has arrived"
	entering <- ch

	input := bufio.NewScanner(conn)
	// Цикл читання вводу від клієнта
	for input.Scan() {
		messages <- who + ": " + input.Text()
	}

	// Коли клієнт відвалився
	leaving <- ch
	messages <- who + " has left"
	conn.Close()
}

// Функція, що пише дані прямо в сокет клієнта
func clientWriter(conn net.Conn, ch <-chan string) {
	for msg := range ch {
		fmt.Fprintln(conn, msg) // Пишемо в мережеве з'єднання
	}
}

// --- MAIN ---
func main() {
	listener, err := net.Listen("tcp", "localhost:8000")
	if err != nil {
		log.Fatal(err)
	}

	// Запускаємо мовника у фоні
	go broadcaster()
	
	log.Println("Chat server started on localhost:8000")

	for {
		conn, err := listener.Accept()
		if err != nil {
			log.Print(err)
			continue
		}
		// Для кожного нового підключення - своя горутина!
		go handleConn(conn)
	}
}