package services

import (
	"orderqueue/models"
	"orderqueue/storage"
	"sort"
	"strings"
	"time"
)

type OrderQueue struct {
	storage *storage.QueueStorage
	orders  []models.Order
	nextID  int
}

func NewOrderQueue() (*OrderQueue, error) {
	qs := &storage.QueueStorage{}
	orders, err := qs.Load()
	if err != nil {
		return nil, err
	}

	oq := &OrderQueue{
		storage: qs,
		orders:  orders,
	}
	oq.calculateNextID()
	oq.sortQueue()
	return oq, nil
}

func (oq *OrderQueue) calculateNextID() {
	oq.nextID = 1
	for _, o := range oq.orders {
		if o.ID >= oq.nextID {
			oq.nextID = o.ID + 1
		}
	}
}

func (oq *OrderQueue) sortQueue() {
	sort.SliceStable(oq.orders, func(i, j int) bool {
		if oq.orders[i].Priority != oq.orders[j].Priority {
			return oq.orders[i].Priority < oq.orders[j].Priority
		}
		return i < j
	})
}

func (oq *OrderQueue) Add(name, item string, priority int) {
	if priority < 1 || priority > 3 {
		priority = 3
	}

	order := models.Order{
		ID:           oq.nextID,
		CustomerName: strings.TrimSpace(name),
		Item:         strings.TrimSpace(item),
		Priority:     priority,
		CreatedAt:    time.Now().Format("2006-01-02 15:04"),
	}

	oq.nextID++
	oq.orders = append(oq.orders, order)
	oq.sortQueue()
	_ = oq.storage.Save(oq.orders)
}

func (oq *OrderQueue) Pop() *models.Order {
	if len(oq.orders) == 0 {
		return nil
	}
	order := oq.orders[0]
	oq.orders = oq.orders[1:]
	_ = oq.storage.Save(oq.orders)
	return &order
}

func (oq *OrderQueue) List() []models.Order {
	return oq.orders
}

func (oq *OrderQueue) FindByCustomer(partial string) []models.Order {
	partial = strings.ToLower(partial)
	var res []models.Order
	for _, o := range oq.orders {
		if strings.Contains(strings.ToLower(o.CustomerName), partial) {
			res = append(res, o)
		}
	}
	return res
}

func (oq *OrderQueue) FindByID(id int) *models.Order {
	for i := range oq.orders {
		if oq.orders[i].ID == id {
			return &oq.orders[i]
		}
	}
	return nil
}

func (oq *OrderQueue) DeleteByID(id int) bool {
	for i, o := range oq.orders {
		if o.ID == id {
			oq.orders = append(oq.orders[:i], oq.orders[i+1:]...)
			_ = oq.storage.Save(oq.orders)
			return true
		}
	}
	return false
}

func (oq *OrderQueue) Edit(id int, name, item string, priority int) bool {
	order := oq.FindByID(id)
	if order == nil {
		return false
	}
	if name != "" {
		order.CustomerName = strings.TrimSpace(name)
	}
	if item != "" {
		order.Item = strings.TrimSpace(item)
	}
	if priority >= 1 && priority <= 3 {
		order.Priority = priority
	}
	oq.sortQueue()
	_ = oq.storage.Save(oq.orders)
	return true
}

func (oq *OrderQueue) Stats() (total, p1, p2, p3 int) {
	total = len(oq.orders)
	for _, o := range oq.orders {
		switch o.Priority {
		case 1:
			p1++
		case 2:
			p2++
		case 3:
			p3++
		}
	}
	return
}