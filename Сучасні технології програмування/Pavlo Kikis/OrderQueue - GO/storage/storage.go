package storage

import (
    "encoding/json"
    "os"
    "orderqueue/models"
)

const dataDir = "data"
const filePath = "data/orders.json"

type QueueStorage struct{}

func init() {
    os.MkdirAll(dataDir, 0755)
}

func (qs *QueueStorage) Load() ([]models.Order, error) {
    if _, err := os.Stat(filePath); os.IsNotExist(err) {
        return []models.Order{}, nil
    }

    data, err := os.ReadFile(filePath)
    if err != nil {
        return nil, err
    }

    var orders []models.Order
    if len(data) == 0 {
        return []models.Order{}, nil
    }
    err = json.Unmarshal(data, &orders)
    return orders, err
}

func (qs *QueueStorage) Save(orders []models.Order) error {
    data, _ := json.MarshalIndent(orders, "", "  ")
    return os.WriteFile(filePath, data, 0644)
}