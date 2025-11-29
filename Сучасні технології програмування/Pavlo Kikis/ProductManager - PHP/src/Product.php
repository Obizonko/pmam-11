<?php

class Product {
    public int $id;
    public string $name;
    public float $price;
    public int $quantity;
    public string $category;
    public string $createdAt;

    public function __construct(int $id, string $name, float $price, int $quantity, string $category = 'Загальне') {
        $this->id = $id;
        $this->name = trim($name);
        $this->price = max(0.01, $price);
        $this->quantity = max(0, $quantity);
        $this->category = empty(trim($category)) ? 'Загальне' : trim($category);
        $this->createdAt = date('Y-m-d H:i:s');
    }

    public function toArray(): array {
        return get_object_vars($this);
    }
}