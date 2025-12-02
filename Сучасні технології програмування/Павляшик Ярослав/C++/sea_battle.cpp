#include <iostream>
#include <vector>
#include <ctime>
#include <cstdlib>
using namespace std;

const int N = 10;

// 0 — порожньо
// 1 — корабель
// 2 — промах
// 3 — попадання

struct Ship {
    vector<pair<int,int>> cells;
    bool alive = true;
};

vector<vector<int>> player(N, vector<int>(N, 0));
vector<vector<int>> bot(N, vector<int>(N, 0));
vector<Ship> shipsPlayer;
vector<Ship> shipsBot;

// Перевірка, чи можна поставити корабель
bool canPlace(vector<vector<int>>& board, int x, int y, int len, bool horizontal) {
    for (int i = 0; i < len; i++) {
        int nx = x + (horizontal ? 0 : i);
        int ny = y + (horizontal ? i : 0);
        if (nx < 0 || ny < 0 || nx >= N || ny >= N) return false;
        if (board[nx][ny] != 0) return false;

        // Перевірка сусідніх клітин
        for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++) {
                int xx = nx + dx, yy = ny + dy;
                if (xx >= 0 && yy >= 0 && xx < N && yy < N) {
                    if (board[xx][yy] == 1) return false;
                }
            }
    }
    return true;
}

void placeShipRandom(vector<vector<int>>& board, vector<Ship>& ships, int len) {
    while (true) {
        int x = rand() % N;
        int y = rand() % N;
        bool horizontal = rand() % 2;

        if (!canPlace(board, x, y, len, horizontal))
            continue;

        Ship s;
        for (int i = 0; i < len; i++) {
            int nx = x + (horizontal ? 0 : i);
            int ny = y + (horizontal ? i : 0);
            board[nx][ny] = 1;
            s.cells.push_back({nx, ny});
        }
        ships.push_back(s);
        return;
    }
}

void placeAllShips(vector<vector<int>>& board, vector<Ship>& ships) {
    vector<int> sizes = {4, 3, 3, 2, 2, 2, 1, 1, 1, 1};
    for (int s : sizes)
        placeShipRandom(board, ships, s);
}

void printBoards() {
    cout << "\n   ВАШЕ ПОЛЕ\t\tПОЛЕ БОТА\n";
    cout << "   0 1 2 3 4 5 6 7 8 9\t   0 1 2 3 4 5 6 7 8 9\n";
    for (int i = 0; i < N; i++) {
        cout << i << "  ";
        for (int j = 0; j < N; j++) {
            if (player[i][j] == 1) cout << "■ ";
            else if (player[i][j] == 3) cout << "X ";
            else if (player[i][j] == 2) cout << ". ";
            else cout << "~ ";
        }
        cout << "\t" << i << "  ";
        for (int j = 0; j < N; j++) {
            if (bot[i][j] == 3) cout << "X ";
            else if (bot[i][j] == 2) cout << ". ";
            else cout << "~ ";
        }
        cout << "\n";
    }
}

bool checkShipDestroyed(vector<Ship>& ships, int x, int y) {
    for (auto& s : ships) {
        for (auto& c : s.cells)
            if (c.first == x && c.second == y) {
                bool allHit = true;
                for (auto& cc : s.cells)
                    if (bot[cc.first][cc.second] != 3)
                        allHit = false;
                if (allHit) {
                    s.alive = false;
                    return true;
                }
            }
    }
    return false;
}

bool allDestroyed(vector<Ship>& ships) {
    for (auto& s : ships)
        if (s.alive) return false;
    return true;
}

void botTurn() {
    while (true) {
        int x = rand() % N;
        int y = rand() % N;
        if (player[x][y] == 2 || player[x][y] == 3)
            continue;
        if (player[x][y] == 1) {
            cout << "Бот влучив у (" << x << ", " << y << ")!\n";
            player[x][y] = 3;
        } else {
            cout << "Бот промахнувся.\n";
            player[x][y] = 2;
        }
        break;
    }
}

int main() {
    srand(time(0));

    placeAllShips(player, shipsPlayer);
    placeAllShips(bot, shipsBot);

    cout << "=== МОРСЬКИЙ БІЙ ===\n";

    while (true) {
        printBoards();

        int x, y;
        cout << "\nВаш постріл (x y): ";
        cin >> x >> y;

        if (x < 0 || y < 0 || x >= N || y >= N) {
            cout << "Невірні координати!\n";
            continue;
        }

        if (bot[x][y] == 2 || bot[x][y] == 3) {
            cout << "Ви вже стріляли сюди.\n";
            continue;
        }

        if (bot[x][y] == 1) {
            cout << "Влучили!\n";
            bot[x][y] = 3;
            checkShipDestroyed(shipsBot, x, y);
            if (allDestroyed(shipsBot)) {
                cout << "\nВИ ПЕРЕМОГЛИ!\n";
                break;
            }
        } else {
            cout << "Промах.\n";
            bot[x][y] = 2;
            botTurn();
            if (allDestroyed(shipsPlayer)) {
                cout << "\nБОТ ПЕРЕМІГ!\n";
                break;
            }
        }
    }

    return 0;
}