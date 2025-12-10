import numpy as np
import pandas as pd
import matplotlib.pyplot as plt
from datetime import datetime, timedelta

class MarketSimulator:
    def __init__(self, start_price=100, days=365, volatility=0.2):
        self.start_price = start_price
        self.days = days
        self.volatility = volatility
        self.data = None

    def generate_historical_data(self):
        """
        Генерує синтетичні 'історичні' дані для аналізу.
        У реальності ми б брали це з CSV або API.
        """
        np.random.seed(42) # Фіксуємо випадковість для відтворюваності
        dates = pd.date_range(start=datetime.now() - timedelta(days=self.days), periods=self.days)
        
        # Випадкові зміни ціни (Returns)
        returns = np.random.normal(loc=0.0005, scale=self.volatility/np.sqrt(252), size=self.days)
        
        # Розрахунок ціни: P_t = P_0 * exp(cumu_sum(returns))
        price_paths = self.start_price * np.exp(np.cumsum(returns))
        
        self.data = pd.DataFrame(data={'Close': price_paths}, index=dates)
        print(f"[INFO] Згенеровано історію за {self.days} днів.")
        return self.data

    def run_monte_carlo(self, simulations=1000, future_days=100):
        """
        Запускає симуляцію майбутнього 1000 разів.
        """
        if self.data is None:
            raise ValueError("Спочатку згенеруйте історичні дані!")

        last_price = self.data['Close'].iloc[-1]
        
        # Розрахунок логарифмічних прибутків
        log_returns = np.log(1 + self.data['Close'].pct_change())
        
        # Параметри для Броунівського руху (Drift & Variance)
        u = log_returns.mean()
        var = log_returns.var()
        drift = u - (0.5 * var)
        stdev = log_returns.std()

        # Магія NumPy: Генеруємо матрицю [дні x симуляції] миттєво
        # Z - це випадковий компонент (шок)
        Z = np.random.normal(0, 1, (future_days, simulations))
        
        # Формула геометричного Броунівського руху
        daily_returns = np.exp(drift + stdev * Z)
        
        price_paths = np.zeros_like(daily_returns)
        price_paths[0] = last_price
        
        # Накопичення результату
        for t in range(1, future_days):
            price_paths[t] = price_paths[t-1] * daily_returns[t]

        print(f"[INFO] Виконано {simulations} симуляцій на {future_days} днів вперед.")
        return price_paths

    def plot_results(self, simulations_data):
        plt.figure(figsize=(12, 6))
        
        # Стиль графіка
        plt.style.use('bmh') # або 'ggplot'

        # 1. Малюємо всі 1000 варіантів розвитку подій
        # alpha=0.05 робить лінії прозорими, щоб бачити щільність
        plt.plot(simulations_data, color='cyan', alpha=0.05, linewidth=1)
        
        # 2. Малюємо "середній" сценарій (медіану)
        median_path = np.median(simulations_data, axis=1)
        plt.plot(median_path, color='red', linewidth=2, label='Медіанний прогноз')

        # 3. Додаємо стартову ціну
        start_val = simulations_data[0][0]
        plt.axhline(y=start_val, color='black', linestyle='--', label='Поточна ціна')

        plt.title('Monte Carlo Simulation: 1000 можливих сценаріїв')
        plt.xlabel('Дні в майбутньому')
        plt.ylabel('Ціна активу ($)')
        plt.legend()
        plt.grid(True)
        
        # Показуємо вікно
        plt.show()

# --- MAIN ---
if __name__ == "__main__":
    # Ініціалізація
    sim = MarketSimulator(start_price=150, volatility=0.3)
    
    # 1. Отримуємо дані
    hist_data = sim.generate_historical_data()
    print(f"Остання відома ціна: ${hist_data['Close'].iloc[-1]:.2f}")

    # 2. Запускаємо "передбачення"
    future_paths = sim.run_monte_carlo(simulations=1000, future_days=90)

    # 3. Аналітика результатів
    final_prices = future_paths[-1]
    print("\n--- Результати прогнозу (через 90 днів) ---")
    print(f"Найгірший сценарій: ${np.min(final_prices):.2f}")
    print(f"Середній сценарій:  ${np.mean(final_prices):.2f}")
    print(f"Найкращий сценарій: ${np.max(final_prices):.2f}")

    # 4. Графік
    sim.plot_results(future_paths)