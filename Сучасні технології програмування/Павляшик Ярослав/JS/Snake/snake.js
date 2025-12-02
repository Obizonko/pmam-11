const canvas = document.getElementById('gameCanvas');
const ctx = canvas.getContext('2d');

const box = 20; 
const canvasSize = canvas.width;

let snake = [{x: 8 * box, y: 8 * box}];
let direction = 'RIGHT';
let nextDirection = 'RIGHT';
let food = {x: randomPosition(), y: randomPosition()};
let score = 0;

document.addEventListener('keydown', changeDirection);

function randomPosition() {
    return Math.floor(Math.random() * (canvasSize / box)) * box;
}

function changeDirection(e) {
    if (e.key === "ArrowUp" && direction !== "DOWN") nextDirection = "UP";
    else if (e.key === "ArrowDown" && direction !== "UP") nextDirection = "DOWN";
    else if (e.key === "ArrowLeft" && direction !== "RIGHT") nextDirection = "LEFT";
    else if (e.key === "ArrowRight" && direction !== "LEFT") nextDirection = "RIGHT";
}

function draw() {
    direction = nextDirection;

    ctx.fillStyle = "#f0f0f0";
    ctx.fillRect(0, 0, canvasSize, canvasSize);

    for (let i = 0; i < snake.length; i++) {
        ctx.fillStyle = (i === 0) ? "green" : "darkgreen";
        ctx.fillRect(snake[i].x, snake[i].y, box, box);
    }

    ctx.fillStyle = "red";
    ctx.fillRect(food.x, food.y, box, box);

    let headX = snake[0].x;
    let headY = snake[0].y;

    if (direction === "LEFT") headX -= box;
    if (direction === "RIGHT") headX += box;
    if (direction === "UP") headY -= box;
    if (direction === "DOWN") headY += box;

    let newHead = {x: headX, y: headY};

    if (headX < 0 || headX >= canvasSize || headY < 0 || headY >= canvasSize || collision(newHead, snake)) {
        clearInterval(game);
        alert("Гра закінчена! Ваш рахунок: " + score);
        return;
    }

    if (headX === food.x && headY === food.y) {
        score++;
        document.getElementById('score').innerText = "Очки: " + score;
        food = {x: randomPosition(), y: randomPosition()};
    } else {
        snake.pop();
    }

    snake.unshift(newHead);
}

function collision(head, array) {
    for (let i = 0; i < array.length; i++) {
        if (head.x === array[i].x && head.y === array[i].y) return true;
    }
    return false;
}

let game = setInterval(draw, 150);
