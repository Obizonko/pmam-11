#include <iostream>
#include <vector>
#include <string>
#include <iomanip>
#include <algorithm>

using namespace std;

// ---------------------------------------------
// Модель одного проєкту
// ---------------------------------------------
struct Project {
    string name;   // Назва проєкту
    int cost;      // Вартість (наприклад, тис. у.о.)
    int benefit;   // Очікувана вигода / ефект (умовні бали)
};

// ---------------------------------------------
// Функція виводу списку проєктів
// ---------------------------------------------
void printProjects(const vector<Project>& projects) {
    cout << "Список доступних проєктів:\n";
    cout << left << setw(15) << "Проєкт"
         << setw(10) << "Вартість"
         << setw(10) << "Вигода" << "\n";
    cout << string(35, '-') << "\n";

    for (const auto& p : projects) {
        cout << left << setw(15) << p.name
             << setw(10) << p.cost
             << setw(10) << p.benefit << "\n";
    }
    cout << "\n";
}

// ---------------------------------------------
// Динамічне програмування для задачі рюкзака 0/1
// ---------------------------------------------
vector<int> knapsackSolve(const vector<Project>& projects, int budget, int& maxBenefit) {
    int n = static_cast<int>(projects.size());

    // dp[i][w] - максимальна вигода, якщо розглядаємо
    // перші i проєктів та маємо бюджет w
    vector<vector<int>> dp(n + 1, vector<int>(budget + 1, 0));

    // Заповнення таблиці DP
    for (int i = 1; i <= n; ++i) {
        int cost = projects[i - 1].cost;
        int benefit = projects[i - 1].benefit;

        for (int w = 0; w <= budget; ++w) {
            // Варіант 1: не беремо проєкт i
            dp[i][w] = dp[i - 1][w];

            // Варіант 2: беремо проєкт i (якщо вистачає бюджету)
            if (w >= cost) {
                int candidate = dp[i - 1][w - cost] + benefit;
                if (candidate > dp[i][w]) {
                    dp[i][w] = candidate;
                }
            }
        }
    }

    maxBenefit = dp[n][budget];

    // Відновлення оптимального набору проєктів
    vector<int> chosenIndices;
    int w = budget;
    for (int i = n; i >= 1; --i) {
        if (dp[i][w] != dp[i - 1][w]) {
            // Проєкт i-1 був включений
            chosenIndices.push_back(i - 1);
            w -= projects[i - 1].cost;
        }
    }

    // Зараз індекси в зворотному порядку, розвернемо
    reverse(chosenIndices.begin(), chosenIndices.end());
    return chosenIndices;
}

// ---------------------------------------------
// Головна функція
// ---------------------------------------------
int main() {
    // Невеличкий вступ
    cout << "=============================================\n";
    cout << "  Project Portfolio Optimization (0/1 Knapsack)\n";
    cout << "  Сучасні технології програмування - C++\n";
    cout << "  Спеціальність: Системний аналіз\n";
    cout << "=============================================\n\n";

    // Приклад множини проєктів
    vector<Project> projects = {
        {"Proj_A", 4, 10},
        {"Proj_B", 6, 12},
        {"Proj_C", 5, 8},
        {"Proj_D", 3, 7},
        {"Proj_E", 2, 4}
    };

    printProjects(projects);

    int budget;
    cout << "Введіть доступний бюджет (ціле число, наприклад 10): ";
    cin >> budget;

    if (!cin || budget <= 0) {
        cerr << "Некоректне значення бюджету. Завершення програми.\n";
        return 1;
    }

    int maxBenefit = 0;
    vector<int> chosen = knapsackSolve(projects, budget, maxBenefit);

    cout << "\nРезультати оптимізації портфеля проєктів:\n";
    cout << "Доступний бюджет: " << budget << "\n";
    cout << "Максимальна сумарна вигода: " << maxBenefit << "\n";

    int usedBudget = 0;
    cout << "\nОбрані проєкти:\n";
    if (chosen.empty()) {
        cout << "Жодного проєкту не обрано (можливо, бюджет занадто малий).\n";
    } else {
        cout << left << setw(15) << "Проєкт"
             << setw(10) << "Вартість"
             << setw(10) << "Вигода" << "\n";
        cout << string(35, '-') << "\n";

        for (int idx : chosen) {
            const auto& p = projects[idx];
            cout << left << setw(15) << p.name
                 << setw(10) << p.cost
                 << setw(10) << p.benefit << "\n";
            usedBudget += p.cost;
        }
    }

    cout << "\nВикористаний бюджет: " << usedBudget << " з " << budget << "\n";
    cout << "Невикористаний бюджет: " << (budget - usedBudget) << "\n";

    cout << "\nПояснення:\n";
    cout << "- задача формалізується як оптимізація за обмеженням ресурсу (бюджету);\n";
    cout << "- використано класичний алгоритм динамічного програмування для задачі рюкзака 0/1;\n";
    cout << "- результатом є оптимальний портфель проєктів із максимальною сумарною вигодою.\n";

    cout << "\nНатисніть Enter для завершення...";
    cin.ignore();
    cin.get();
    return 0;
}
