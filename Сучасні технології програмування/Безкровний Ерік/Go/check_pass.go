package main

import (
	"bufio"
	"fmt"
	"math"
	"os"
	"unicode"
)

type StrengthInfo struct {
	Length      int
	Lower       bool
	Upper       bool
	Numbers     bool
	Symbols     bool
	CharsetSize int
	Entropy     float64
	CrackTime   string
	Score       string
}

func charsetSize(password string) (int, bool, bool, bool, bool) {
	lower, upper, num, sym := false, false, false, false

	for _, ch := range password {
		switch {
		case unicode.IsLower(ch):
			lower = true
		case unicode.IsUpper(ch):
			upper = true
		case unicode.IsDigit(ch):
			num = true
		default:
			sym = true
		}
	}

	size := 0
	if lower {
		size += 26
	}
	if upper {
		size += 26
	}
	if num {
		size += 10
	}
	if sym {
		size += 33
	}

	return size, lower, upper, num, sym
}

func entropy(bitsPerChar float64, length int) float64 {
	return bitsPerChar * float64(length)
}

func humanTime(seconds float64) string {
	const (
		minute = 60
		hour   = 3600
		day    = 86400
		year   = 31536000
	)

	switch {
	case seconds < minute:
		return fmt.Sprintf("%.2f сек", seconds)
	case seconds < hour:
		return fmt.Sprintf("%.2f хв", seconds/minute)
	case seconds < day:
		return fmt.Sprintf("%.2f год", seconds/hour)
	case seconds < year:
		return fmt.Sprintf("%.2f днів", seconds/day)
	default:
		return fmt.Sprintf("%.2f років", seconds/year)
	}
}

func evaluatePassword(password string) StrengthInfo {
	size, lower, upper, num, sym := charsetSize(password)

	bitsPerChar := math.Log2(float64(size))
	ent := entropy(bitsPerChar, len(password))
	crackSeconds := math.Pow(float64(size), float64(len(password))) / 1e12

	score := "Дуже слабкий"
	switch {
	case ent < 28:
		score = "Слабкий"
	case ent < 36:
		score = "Середній"
	case ent < 60:
		score = "Хороший"
	case ent < 128:
		score = "Сильний"
	default:
		score = "Дуже сильний"
	}

	return StrengthInfo{
		Length:      len(password),
		Lower:       lower,
		Upper:       upper,
		Numbers:     num,
		Symbols:     sym,
		CharsetSize: size,
		Entropy:     ent,
		CrackTime:   humanTime(crackSeconds),
		Score:       score,
	}
}

func main() {
	reader := bufio.NewReader(os.Stdin)
	fmt.Print("Введіть пароль для перевірки → ")

	pass, _ := reader.ReadString('\n')
	pass = pass[:len(pass)-1]

	info := evaluatePassword(pass)

	fmt.Println("\n=== Аналіз пароля ===")
	fmt.Println("Довжина:", info.Length)
	fmt.Println("Містить малі літери:", info.Lower)
	fmt.Println("Містить великі літери:", info.Upper)
	fmt.Println("Містить цифри:", info.Numbers)
	fmt.Println("Містить символи:", info.Symbols)
	fmt.Println("Розмір алфавіту:", info.CharsetSize)
	fmt.Printf("Ентропія: %.2f біт\n", info.Entropy)
	fmt.Println("Оцінка:", info.Score)
	fmt.Println("Час зламу (bruteforce):", info.CrackTime)
}
