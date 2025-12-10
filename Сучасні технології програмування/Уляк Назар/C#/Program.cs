using System;
using System.Collections.Generic;
using System.Threading;

namespace AdvancedAtm
{
    // --- DATA MODEL ---
    class BankAccount
    {
        public string CardNumber { get; }
        private string Pin { get; } // Encapsulated for security
        public string OwnerName { get; }
        public decimal Balance { get; private set; }

        public BankAccount(string card, string pin, string name)
        {
            CardNumber = card;
            Pin = pin;
            OwnerName = name;
            Balance = 0; // New accounts start with 0
        }

        // Logic to validate login
        public bool ValidatePin(string inputPin)
        {
            return Pin == inputPin;
        }

        public void Deposit(decimal amount)
        {
            if (amount > 0) Balance += amount;
        }

        public bool TryWithdraw(decimal amount)
        {
            if (amount <= 0 || amount > Balance) return false;
            Balance -= amount;
            return true;
        }
    }

    // --- APPLICATION LOGIC ---
    class Program
    {
        // In-memory database
        static Dictionary<string, BankAccount> database = new Dictionary<string, BankAccount>();

        static void Main()
        {
            Console.Title = "C# Advanced ATM";

            // Main System Loop
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("=== WELCOME TO C# BANK ===");
                Console.ResetColor();
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Register new account");
                Console.WriteLine("3. Exit");
                Console.Write("Select option > ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        PerformLogin();
                        break;
                    case "2":
                        PerformRegistration();
                        break;
                    case "3":
                        Console.WriteLine("Goodbye!");
                        return; // Exit the program
                    default:
                        PrintMessage("Invalid option. Try again.", ConsoleColor.Red);
                        break;
                }
            }
        }

        // --- FEATURE: REGISTRATION ---
        static void PerformRegistration()
        {
            Console.Clear();
            Console.WriteLine("--- NEW ACCOUNT REGISTRATION ---");

            // 1. Enter unique Card Number
            Console.Write("Enter desired Card Number (e.g., 4444): ");
            string newCard = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(newCard))
            {
                PrintMessage("Card number cannot be empty.", ConsoleColor.Red);
                return;
            }

            if (database.ContainsKey(newCard))
            {
                PrintMessage("Error: This card number already exists!", ConsoleColor.Red);
                return;
            }

            // 2. Enter Name
            Console.Write("Enter your Full Name: ");
            string name = Console.ReadLine();

            // 3. Create PIN
            Console.Write("Set your 4-digit PIN: ");
            string pin = GetHiddenInput();

            if (pin.Length != 4 || !int.TryParse(pin, out _))
            {
                PrintMessage("Error: PIN must be exactly 4 digits.", ConsoleColor.Red);
                return;
            }

            // 4. Save to Database
            BankAccount newAccount = new BankAccount(newCard, pin, name);
            database.Add(newCard, newAccount);

            Console.WriteLine(); // New line after PIN input
            PrintMessage($"Success! Account created for {name}.", ConsoleColor.Green);
        }

        // --- FEATURE: LOGIN ---
        static void PerformLogin()
        {
            Console.Clear();
            Console.WriteLine("--- LOGIN ---");

            Console.Write("Card Number: ");
            string cardInput = Console.ReadLine();

            if (!database.ContainsKey(cardInput))
            {
                PrintMessage("Card not found. Please register first.", ConsoleColor.Red);
                return;
            }

            BankAccount account = database[cardInput];

            Console.Write("Enter PIN: ");
            string pinInput = GetHiddenInput();

            Console.WriteLine("\nVerifying...");
            Thread.Sleep(800); // Simulate network delay

            if (account.ValidatePin(pinInput))
            {
                RunUserSession(account);
            }
            else
            {
                PrintMessage("Wrong PIN!", ConsoleColor.Red);
            }
        }

        // --- FEATURE: USER DASHBOARD ---
        static void RunUserSession(BankAccount account)
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Welcome, {account.OwnerName}!");
                Console.ResetColor();
                Console.WriteLine($"Current Balance: ${account.Balance}");
                Console.WriteLine("---------------------------");
                Console.WriteLine("1. Withdraw Cash");
                Console.WriteLine("2. Deposit Cash");
                Console.WriteLine("3. Logout");
                Console.Write("Select > ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Amount to withdraw: $");
                        if (decimal.TryParse(Console.ReadLine(), out decimal withdrawAmount))
                        {
                            if (account.TryWithdraw(withdrawAmount))
                                PrintMessage($"Success. Please take your ${withdrawAmount}.", ConsoleColor.Green);
                            else
                                PrintMessage("Insufficient funds or invalid amount.", ConsoleColor.Red);
                        }
                        else PrintMessage("Invalid input.", ConsoleColor.Red);
                        break;

                    case "2":
                        Console.Write("Amount to deposit: $");
                        if (decimal.TryParse(Console.ReadLine(), out decimal depositAmount))
                        {
                            if (depositAmount > 0)
                            {
                                account.Deposit(depositAmount);
                                PrintMessage($"${depositAmount} added to your account.", ConsoleColor.Green);
                            }
                            else PrintMessage("Amount must be positive.", ConsoleColor.Red);
                        }
                        else PrintMessage("Invalid input.", ConsoleColor.Red);
                        break;

                    case "3":
                        return; // Go back to Main Menu

                    default:
                        break;
                }
            }
        }

        // --- UTILS ---

        // Masking password input with asterisks (*)
        static string GetHiddenInput()
        {
            string pass = "";
            do
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                // Handle Backspace
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    pass = pass.Substring(0, (pass.Length - 1));
                    Console.Write("\b \b");
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    break;
                }
            } while (true);
            return pass;
        }

        static void PrintMessage(string msg, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine("\n" + msg);
            Console.ResetColor();
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }
    }
}