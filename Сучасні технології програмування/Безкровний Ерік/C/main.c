#include <stdio.h>

char a[3][3] = {
    {' ', ' ', ' '},
    {' ', ' ', ' '},
    {' ', ' ', ' '}
};

void print_board() {
#ifdef _WIN32
    system("cls");
#else
    system("clear");
#endif
    printf(" X / O TIC-TAC-TOE \n\n");
    for (int i = 0; i < 3; i++) {
        printf(" %c | %c | %c\n", a[i][0], a[i][1], a[i][2]);
        if (i < 2) printf("---+---+---\n");
    }
    printf("\n");
}

int check_win() {
    // rows
    for (int i = 0; i < 3; i++)
        if (a[i][0] == a[i][1] && a[i][1] == a[i][2] && a[i][0] != ' ')
            return 1;

    // columns
    for (int j = 0; j < 3; j++)
        if (a[0][j] == a[1][j] && a[1][j] == a[2][j] && a[0][j] != ' ')
            return 1;

    // diagonals
    if (a[0][0] == a[1][1] && a[1][1] == a[2][2] && a[0][0] != ' ')
        return 1;

    if (a[0][2] == a[1][1] && a[1][1] == a[2][0] && a[0][2] != ' ')
        return 1;

    return 0;
}

int board_full() {
    for (int i = 0; i < 3; i++)
        for (int j = 0; j < 3; j++)
            if (a[i][j] == ' ')
                return 0;
    return 1;
}

int main() {
    int r, c;
    char player = 'X';

    while (1) {
        print_board();
        printf("Гравець %c, введи рядок і стовпець (1-3 1-3): ", player);
        scanf("%d %d", &r, &c);
        r--; c--;

        if (r < 0 || r > 2 || c < 0 || c > 2 || a[r][c] != ' ') {
            printf("Невірний хід! Натисни Enter...\n");
            getchar(); getchar();
            continue;
        }

        a[r][c] = player;

        if (check_win()) {
            print_board();
            printf("Гравець %c переміг!\n", player);
            break;
        }

        if (board_full()) {
            print_board();
            printf("Нічия!\n");
            break;
        }

        player = (player == 'X' ? 'O' : 'X');
    }

    return 0;
}
