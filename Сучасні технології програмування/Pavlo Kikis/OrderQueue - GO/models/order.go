package models

type Order struct {
    ID           int    `json:"id"`
    CustomerName string `json:"customer_name"`
    Item         string `json:"item"`
    Priority     int    `json:"priority"` // 1 = urgent, 2 = high, 3 = normal
    CreatedAt    string `json:"created_at"`
}