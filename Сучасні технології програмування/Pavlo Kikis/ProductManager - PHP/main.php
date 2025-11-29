<?php
require_once 'src/Product.php';
require_once 'src/ProductStorage.php';

$storage = new ProductStorage();
$products = $storage->load();

while (true) {
    echo "\n=== Product Manager ===\n";
    echo "1. Додати товар\n2. Список усіх\n3. Оновити\n4. Видалити\n";
    echo "5. Пошук (назва/категорія)\n6. Загальна вартість\n7. По категоріях\n8. Статистика\n9. Лог дій\n10. Відкат останньої дії (ROLLBACK)\n0. Вихід\n> ";
    $choice = trim(fgets(STDIN));

    switch ($choice) {
        case '1':
            echo "Назва: "; $name = trim(fgets(STDIN));
            echo "Ціна: "; $price = (float)trim(fgets(STDIN));
            echo "Кількість: "; $qty = (int)trim(fgets(STDIN));
            echo "Категорія (Enter = Загальне): "; $cat = trim(fgets(STDIN));
            $p = new Product($storage->getNextId($products), $name, $price, $qty, $cat);
            $storage->add($p, $products);
            echo "Додано!\n";
            break;

        case '2':
            if (empty($products)) { echo "Склад порожній.\n"; break; }
            foreach ($products as $p)
                echo "[{$p->id}] {$p->name} | {$p->price} грн | {$p->quantity} шт. | [{$p->category}] | {$p->createdAt}\n";
            break;

        case '3':
            echo "ID: "; $id = (int)trim(fgets(STDIN));
            echo "Нова назва (Enter = лишити): "; $n = trim(fgets(STDIN));
            echo "Нова ціна: "; $pr = trim(fgets(STDIN));
            echo "Нова кількість: "; $q = trim(fgets(STDIN));
            echo "Нова категорія: "; $c = trim(fgets(STDIN));
            $data = array_filter(['name'=>$n, 'price'=>$pr===''?null:(float)$pr, 'quantity'=>$q===''?null:(int)$q, 'category'=>$c]);
            $storage->update($id, $data, $products) ? print "Оновлено!\n" : print "Не знайдено.\n";
            break;

        case '4':
            echo "ID для видалення: "; $id = (int)trim(fgets(STDIN));
            $storage->delete($id, $products) ? print "Видалено!\n" : print "Не знайдено.\n";
            break;

        case '5':
            echo "Пошук: "; $term = trim(fgets(STDIN));
            $found = $storage->search($products, $term);
            foreach ($found as $p) echo "[{$p->id}] {$p->name} | {$p->price} грн | {$p->quantity} шт. | [{$p->category}]\n";
            if (empty($found)) echo "Нічого не знайдено.\n";
            break;

        case '6':
            echo "Загальна вартість складу: " . number_format($storage->totalValue($products), 2) . " грн\n";
            break;

        case '7':
            foreach ($storage->totalByCategory($products) as $cat => $sum)
                echo "$cat: " . number_format($sum, 2) . " грн\n";
            break;

        case '8':
            $s = $storage->stats($products);
            echo "Товарів: {$s['total_products']} | Позицій: {$s['total_items']} | Вартість: " . number_format($s['total_value'], 2) . " грн\n";
            echo "Категорії: " . implode(', ', $s['categories']) . "\n";
            break;

        case '9':
            $logPath = __DIR__ . '/storage/log.txt';
            if (!file_exists($logPath) || filesize($logPath) == 0) {
                echo "Лог порожній.\n";
                break;
            }

            echo str_repeat("═", 70) . "\n";
            echo "                     ІСТОРІЯ ДІЙ\n";
            echo str_repeat("─", 70) . "\n";

            $lines = array_reverse(file($logPath, FILE_IGNORE_NEW_LINES | FILE_SKIP_EMPTY_LINES));
            foreach ($lines as $line) {
                $entry = json_decode($line, true);
                if (!$entry) continue;

                $time = $entry['time'] ?? '';
                $action = $entry['action'] ?? '';

                switch ($action) {
                    case 'ADD':
                        $p = $entry['product'];
                        echo "{$time}  ДОДАНО      ➜  [{$p['id']}] {$p['name']} | {$p['price']} грн | {$p['quantity']} шт. | [{$p['category']}]\n";
                        break;

                    case 'UPDATE':
                        $p = $entry['product'];
                        echo "{$time}  ОНОВЛЕНО    ➜  [{$p['id']}] {$p['name']} | {$p['price']} грн | {$p['quantity']} шт. | [{$p['category']}]\n";
                        break;

                    case 'DELETE':
                        $p = $entry['product'];
                        echo "{$time}  ВИДАЛЕНО    ➜  [{$p['id']}] {$p['name']} | [{$p['category']}]\n";
                        break;

                    case 'ROLLBACK':
                        $rev = $entry['reverted'] ?? 'невідомо';
                        $symbol = $rev === 'ADD' ? '↶' : ($rev === 'DELETE' ? '↷' : '↺');
                        echo "{$time}  ВІДКАТ {$symbol}     ←  Скасовано дію: $rev\n";
                        break;
                }
            }
            echo str_repeat("═", 70) . "\n";
            break;

        case '10':
            if ($storage->rollback($products)) {
                echo "Останню дію скасовано (відкат виконано)!\n";
            } else {
                echo "Немає дій для відкату або лог порожній.\n";
            }
            break;

        case '0': exit("До зустрічі!\n");
    }
}