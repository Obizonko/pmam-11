<?php

require_once 'Product.php';

class ProductStorage {
    private string $file = __DIR__ . '/../storage/products.json';
    private string $logFile = __DIR__ . '/../storage/log.txt';

    private function log(string $action, Product $product = null, array $extra = []): void {
        $time = date('Y-m-d H:i:s');
        $entry = ['time' => $time, 'action' => $action];
        if ($product) {
            $entry['product'] = $product->toArray();
        }
        $entry += $extra;
        file_put_contents($this->logFile, json_encode($entry, JSON_UNESCAPED_UNICODE) . PHP_EOL, FILE_APPEND);
    }

    public function load(): array {
        if (!file_exists($this->file)) return [];
        $json = file_get_contents($this->file);
        $data = json_decode($json, true) ?: [];
        return array_map(fn($i) => new Product(
            $i['id'],
            $i['name'],
            $i['price'],
            $i['quantity'],
            $i['category'] ?? 'Загальне'
        ), $data);
    }

    public function save(array $products): void {
        $data = array_map(fn($p) => $p->toArray(), $products);
        $flags = (defined('JSON_PRETTY') ? JSON_PRETTY : 0) | (defined('JSON_UNESCAPED_UNICODE') ? JSON_UNESCAPED_UNICODE : 0);
        file_put_contents($this->file, json_encode($data, $flags));
    }

    public function getNextId(array $products): int {
        return empty($products) ? 1 : max(array_map(fn($p) => $p->id, $products)) + 1;
    }

    public function add(Product $p, array &$products): void {
        $products[] = $p;
        $this->save($products);
        $this->log("ADD", $p);
    }

    public function update(int $id, array $data, array &$products): bool {
        foreach ($products as $p) {
            if ($p->id === $id) {
                $old = clone $p;
                if (!empty($data['name'])) $p->name = trim($data['name']);
                if (isset($data['price']) && $data['price'] >= 0.01) $p->price = $data['price'];
                if (isset($data['quantity']) && $data['quantity'] >= 0) $p->quantity = $data['quantity'];
                if (!empty($data['category'])) $p->category = trim($data['category']);
                $this->save($products);
                $this->log("UPDATE", $p, ['old' => $old->toArray()]);
                return true;
            }
        }
        return false;
    }

    public function delete(int $id, array &$products): bool {
        foreach ($products as $i => $p) {
            if ($p->id === $id) {
                $deleted = $p;
                unset($products[$i]);
                $products = array_values($products);
                $this->save($products);
                $this->log("DELETE", $deleted);
                return true;
            }
        }
        return false;
    }

    public function rollback(array &$products): bool {
        if (!file_exists($this->logFile) || filesize($this->logFile) == 0) {
            return false;
        }

        $lines = file($this->logFile, FILE_IGNORE_NEW_LINES | FILE_SKIP_EMPTY_LINES);
        $originalLines = $lines;

        while (!empty($lines)) {
            $lastLine = array_pop($lines);
            $entry = json_decode($lastLine, true);

            if (!$entry || !isset($entry['action']) || $entry['action'] === 'ROLLBACK') {
                continue;
            }

            switch ($entry['action']) {
                case 'ADD':
                    $products = array_filter($products, fn($p) => $p->id !== $entry['product']['id']);
                    $products = array_values($products);
                    break;

                case 'DELETE':
                    $p = $entry['product'];
                    $products[] = new Product(
                        $p['id'],
                        $p['name'],
                        $p['price'],
                        $p['quantity'],
                        $p['category'] ?? 'Загальне'
                    );
                    break;

                case 'UPDATE':
                    if (!empty($entry['old'])) {
                        foreach ($products as $p) {
                            if ($p->id === $entry['product']['id']) {
                                $old = $entry['old'];
                                $p->name = $old['name'];
                                $p->price = $old['price'];
                                $p->quantity = $old['quantity'];
                                $p->category = $old['category'] ?? 'Загальне';
                                break;
                            }
                        }
                    }
                    break;

                default:
                    continue 2;
            }

            $this->save($products);

            file_put_contents($this->logFile, implode(PHP_EOL, $lines) . (count($lines) ? PHP_EOL : ''));

            $this->log("ROLLBACK", null, ['reverted' => $entry['action']]);
            return true;
        }

        return false;
    }

    public function search(array $products, string $term): array {
        $term = strtolower(trim($term));
        return array_filter($products, fn($p) =>
            strpos(strtolower($p->name), $term) !== false ||
            strpos(strtolower($p->category), $term) !== false
        );
    }

    public function totalValue(array $products): float {
        return array_sum(array_map(fn($p) => $p->price * $p->quantity, $products));
    }

    public function totalByCategory(array $products): array {
        $totals = [];
        foreach ($products as $p) {
            $cat = $p->category;
            $totals[$cat] = ($totals[$cat] ?? 0) + ($p->price * $p->quantity);
        }
        return $totals;
    }

    public function stats(array $products): array {
        return [
            'total_products' => count($products),
            'total_value' => $this->totalValue($products),
            'total_items' => array_sum(array_map(fn($p) => $p->quantity, $products)),
            'categories' => array_unique(array_map(fn($p) => $p->category, $products))
        ];
    }
}