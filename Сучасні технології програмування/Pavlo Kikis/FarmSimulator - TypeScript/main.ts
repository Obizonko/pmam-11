import * as readline from 'readline';

// Типи росту рослин
type GrowthStage = 'насіння' | 'паросток' | 'доросла' | "зів'яла";

// Клас рослини
class Plant {
  name: string;
  growthStage: GrowthStage;
  daysSincePlanted: number;
  daysToMature: number;

  constructor(name: string, daysToMature: number) {
    this.name = name;
    this.growthStage = 'насіння';
    this.daysSincePlanted = 0;
    this.daysToMature = daysToMature;
  }

  grow() {
    this.daysSincePlanted++;
    if (this.daysSincePlanted >= this.daysToMature) this.growthStage = 'доросла';
    else if (this.daysSincePlanted >= Math.floor(this.daysToMature / 2))
      this.growthStage = 'паросток';
    if (this.daysSincePlanted > this.daysToMature + 3) this.growthStage = "зів'яла";
  }

  isHarvestable(): boolean {
    return this.growthStage === 'доросла';
  }

  toString(): string {
    return `${this.name} (${this.growthStage})`;
  }
}

// Клас тварини
class Animal {
  name: string;
  species: string;
  hunger: number;
  happiness: number;

  constructor(name: string, species: string) {
    this.name = name;
    this.species = species;
    this.hunger = 0; // ситість 0-10
    this.happiness = 5; // щастя 0-10
  }

  feed() {
    this.hunger = Math.max(0, this.hunger - 3);
    this.happiness = Math.min(10, this.happiness + 1);
  }

  play() {
    this.happiness = Math.min(10, this.happiness + 2);
    this.hunger = Math.min(10, this.hunger + 1);
  }

  dayPasses() {
    this.hunger = Math.min(10, this.hunger + 1);
    if (this.hunger > 7) this.happiness = Math.max(0, this.happiness - 2);
  }

  isAlive(): boolean {
    return this.hunger < 10;
  }

  toString(): string {
    return `${this.name} (${this.species}) - Ситість: ${10 - this.hunger}, Щастя: ${
      this.happiness
    }`;
  }
}

// Клас ферми
class Farm {
  plants: Plant[];
  animals: Animal[];
  day: number;
  balance: number;

  constructor() {
    this.plants = [];
    this.animals = [];
    this.day = 1;
    this.balance = 50;
  }

  plantCrop(name: string, daysToMature: number) {
    if (this.balance < 2) {
      console.log('💸 Недостатньо грошей для насіння!');
      return;
    }
    this.plants.push(new Plant(name, daysToMature));
    this.balance -= 2;
    console.log(`🌱 Ви посадили ${name}. Виросте через ${daysToMature} днів.`);
  }

  addAnimal(name: string, species: string) {
    if (this.balance < 10) {
      console.log('💸 Недостатньо грошей для купівлі тварини!');
      return;
    }
    this.animals.push(new Animal(name, species));
    this.balance -= 10;
    console.log(`🐾 Ви купили ${species} на ім'я ${name}.`);
  }

  harvest() {
    const harvestable = this.plants.filter((p) => p.isHarvestable());
    if (harvestable.length === 0) {
      console.log('🚫 Немає що збирати.');
    } else {
      const earnings = harvestable.length * 5;
      this.balance += earnings;
      console.log(
        `🌾 Зібрали: ${harvestable.map((p) => p.name).join(', ')}. Зароблено $${earnings}.`,
      );
    }
    this.plants = this.plants.filter((p) => !p.isHarvestable());
  }

  feedAnimals() {
    if (this.animals.length === 0) {
      console.log('🚫 Немає тварин для годування!');
      return;
    }
    this.animals.forEach((a) => a.feed());
    this.balance -= this.animals.length;
    console.log(`🍎 Ви нагодували всіх тварин. Вартість: $${this.animals.length}.`);
  }

  playWithAnimals() {
    if (this.animals.length === 0) {
      console.log('🚫 Немає тварин для гри!');
      return;
    }
    this.animals.forEach((a) => a.play());
    console.log('🎾 Пограли з усіма тваринами. Вони щасливіші!');
  }

  sellProduce() {
    const sellable = this.plants.filter((p) => p.isHarvestable());
    if (sellable.length === 0) {
      console.log('🚫 Немає врожаю для продажу.');
      return;
    }
    const earnings = sellable.length * 5;
    this.balance += earnings;
    console.log(`💰 Ви продали: ${sellable.map((p) => p.name).join(', ')} за $${earnings}.`);
    this.plants = this.plants.filter((p) => !p.isHarvestable());
  }

  nextDay() {
    this.day++;
    this.plants.forEach((p) => p.grow());
    this.animals.forEach((a) => a.dayPasses());
    this.animals = this.animals.filter((a) => a.isAlive());
    this.randomEvents();
    console.log(`⏳ День закінчився. Перехід до наступного дня...`);
  }

  randomEvents() {
    const r = Math.random();
    if (r < 0.1 && this.plants.length > 0) {
      const plant = randomChoice(this.plants);
      plant.growthStage = "зів'яла";
      console.log(`⚠️ Шкідники пошкодили ${plant.name}!`);
    } else if (r < 0.15 && this.animals.length > 0) {
      const animal = randomChoice(this.animals);
      animal.happiness = Math.max(0, animal.happiness - 3);
      console.log(`⚠️ ${animal.name} захворіла!`);
    } else if (r > 0.95) {
      const money = Math.floor(Math.random() * 10) + 5;
      this.balance += money;
      console.log(`🎉 Удача! Ви знайшли $${money}!`);
    }
  }

  summary(): string {
    const plantSummary = this.plants.map((p) => p.toString()).join(', ') || 'немає рослин';
    const animalSummary = this.animals.map((a) => a.toString()).join(', ') || 'немає тварин';
    return `📅 День ${this.day} | 💵 Баланс: $${this.balance}\n🌱 Рослини: ${plantSummary}\n🐾 Тварини: ${animalSummary}`;
  }

  isGameOver(): boolean {
    if (this.balance < 0) return true;
    if (this.animals.length === 0 && this.plants.length === 0) return true;
    return false;
  }
}

// Випадкові імена та види
const animalNames = ['Беті', 'МууМуу', 'Клукі', 'Вуллі', 'Поркі', 'Флаффі', 'Спайк'];
const speciesList = ['Корова', 'Курка', 'Вівця', 'Свиня', 'Коза'];
const plantNames = [
  { name: 'Морква', days: 3 },
  { name: 'Помідор', days: 5 },
  { name: 'Картопля', days: 4 },
  { name: 'Салат', days: 2 },
  { name: 'Гарбуз', days: 6 },
];

function randomChoice<T>(arr: T[]): T {
  return arr[Math.floor(Math.random() * arr.length)];
}

// readline
const rl = readline.createInterface({
  input: process.stdin,
  output: process.stdout,
});

const farm = new Farm();

// Початковий старт
for (let i = 0; i < 2; i++) {
  const plant = randomChoice(plantNames);
  farm.plantCrop(plant.name, plant.days);
}

for (let i = 0; i < 1; i++) {
  farm.addAnimal(randomChoice(animalNames), randomChoice(speciesList));
}

// Головний цикл
async function gameLoop() {
  console.log("🎉 Вітаємо у текстовій грі 'Ферма'!");
  while (true) {
    console.log('\n===================================');
    console.log(farm.summary());

    if (farm.isGameOver()) {
      console.log('🏁 Гра закінчена!');
      if (farm.balance < 0) console.log('💸 Ви розорилися.');
      else console.log('⏳ Всі ресурси вичерпані.');
      break;
    }

    const action = await question(
      'Виберіть дію:\n' +
        '1) Посадити рослину\n' +
        '2) Зібрати урожай\n' +
        '3) Погодувати тварин\n' +
        '4) Пограти з тваринами\n' +
        '5) Купити тварину\n' +
        '6) Продати врожай\n' +
        '7) Пропустити день\n' +
        '0) Вийти\n> ',
    );

    switch (action.trim()) {
      case '1':
        plantNames.forEach((p, i) => console.log(`${i + 1}) ${p.name} (${p.days} днів)`));
        const cropChoice = await question('Яку рослину посадити? (номер) > ');
        const cropIndex = parseInt(cropChoice) - 1;
        if (plantNames[cropIndex])
          farm.plantCrop(plantNames[cropIndex].name, plantNames[cropIndex].days);
        else console.log('🚫 Невірний вибір');
        break;
      case '2':
        farm.harvest();
        break;
      case '3':
        farm.feedAnimals();
        break;
      case '4':
        farm.playWithAnimals();
        break;
      case '5':
        farm.addAnimal(randomChoice(animalNames), randomChoice(speciesList));
        break;
      case '6':
        farm.sellProduce();
        break;
      case '7':
        console.log('⏩ Пропускаємо день...');
        break;
      case '0':
        console.log('👋 Дякуємо за гру!');
        rl.close();
        return;
      default:
        console.log('🚫 Невірний ввід');
    }

    farm.nextDay();
  }
  rl.close();
}

function question(query: string): Promise<string> {
  return new Promise((resolve) => rl.question(query, resolve));
}

gameLoop();
