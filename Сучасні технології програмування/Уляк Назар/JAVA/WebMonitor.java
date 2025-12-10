import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.time.Duration;
import java.time.Instant;
import java.util.List;
import java.util.concurrent.CompletableFuture;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.regex.Matcher;
import java.util.regex.Pattern;
import java.util.stream.Collectors;

// Автоматично створює конструктор, геттери, equals(), hashCode() та toString()
record SiteResult(String url, int statusCode, long durationMs, String pageTitle) {}

public class WebMonitor {

    // Створюємо пул потоків (10 одночасних воркерів)
    private static final ExecutorService executor = Executors.newFixedThreadPool(10);
    
    // Сучасний HTTP клієнт
    private static final HttpClient client = HttpClient.newBuilder()
            .executor(executor)
            .connectTimeout(Duration.ofSeconds(5))
            .build();

    public static void main(String[] args) {
        // Список цілей
        List<String> targets = List.of(
            "https://www.google.com",
            "https://www.github.com",
            "https://stackoverflow.com",
            "https://www.oracle.com",
            "https://www.wikipedia.org",
            "https://www.apple.com",
            "https://non-existent-site-example.com", // Тест на помилку
            "https://www.bbc.com"
        );

        System.out.println("Starting async analysis for " + targets.size() + " sites...\n");

        long startTotal = System.currentTimeMillis();

        // 2. ASYNC FLOW: Запуск завдань
        // Створюємо список "Фьючерсів" (обіцянок результату в майбутньому)
        List<CompletableFuture<SiteResult>> futures = targets.stream()
                .map(WebMonitor::checkSiteAsync)
                .toList();

        // Чекаємо завершення ВСІХ запитів
        // join() блокує main потік, поки всі фонові задачі не виконаються
        List<SiteResult> results = futures.stream()
                .map(CompletableFuture::join) 
                .toList();

        long endTotal = System.currentTimeMillis();

        // 3. ANALYTICS: Обробка результатів через Stream API
        System.out.println("\n--- REPORT ---");
        
        // Вивід таблиці
        String format = "| %-35s | %-5s | %-6s | %-30s |%n";
        System.out.format("+-------------------------------------+-------+--------+--------------------------------+%n");
        System.out.format("| URL                                 | Code  | TimeMs | Title                          |%n");
        System.out.format("+-------------------------------------+-------+--------+--------------------------------+%n");

        for (SiteResult res : results) {
            System.out.format(format, 
                res.url().replace("https://www.", ""), // Трохи скоротимо URL для краси
                res.statusCode(), 
                res.durationMs(), 
                truncate(res.pageTitle(), 30)
            );
        }
        System.out.format("+-------------------------------------+-------+--------+--------------------------------+%n");

        // Статистика
        double avgTime = results.stream()
                .filter(r -> r.statusCode() == 200)
                .mapToLong(SiteResult::durationMs)
                .average()
                .orElse(0);

        System.out.println("\nTotal time: " + (endTotal - startTotal) + " ms");
        System.out.printf("Average response time (success only): %.2f ms%n", avgTime);

        // Завжди закриваємо пул потоків
        executor.shutdown();
    }

    // Метод повертає CompletableFuture (асинхронна операція)
    private static CompletableFuture<SiteResult> checkSiteAsync(String url) {
        HttpRequest request = HttpRequest.newBuilder()
                .uri(URI.create(url))
                .GET()
                .build();

        Instant start = Instant.now();

        // sendAsync не блокує потік, а повертає Future
        return client.sendAsync(request, HttpResponse.BodyHandlers.ofString())
                .thenApply(response -> {
                    // Цей блок виконується, коли прийшла відповідь
                    long time = Duration.between(start, Instant.now()).toMillis();
                    String title = extractTitle(response.body());
                    return new SiteResult(url, response.statusCode(), time, title);
                })
                .exceptionally(ex -> {
                    // Обробка помилок (наприклад, сайт недоступний)
                    long time = Duration.between(start, Instant.now()).toMillis();
                    return new SiteResult(url, 0, time, "ERROR: " + ex.getCause().getMessage()); // ex.getMessage()
                });
    }

    // Допоміжний метод: парсинг заголовка через Regex
    private static String extractTitle(String html) {
        Pattern pattern = Pattern.compile("<title>(.*?)</title>", Pattern.CASE_INSENSITIVE);
        Matcher matcher = pattern.matcher(html);
        return matcher.find() ? matcher.group(1).trim() : "No Title";
    }

    // Допоміжний метод для обрізання довгих рядків
    private static String truncate(String str, int width) {
        if (str.length() > width) {
            return str.substring(0, width - 3) + "...";
        }
        return str;
    }
}