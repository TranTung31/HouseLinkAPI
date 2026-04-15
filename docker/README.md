# Docker Infrastructure

Thư mục này chứa các Docker Compose files để chạy các dịch vụ infrastructure cần thiết cho phát triển HouseLinkAPI.

## Cấu trúc

```
docker/
├── kafka/
│   ├── docker-compose.yml    # Kafka + Zookeeper + Kafka UI
│   └── README.md
├── redpanda/
│   ├── docker-compose.yml    # Redpanda + Console
│   └── README.md
└── README.md
```

## Redpanda (Activity Logging - Recommended)

Redpanda là Kafka-compatible message broker nhẹ hơn và nhanh hơn, phù hợp hơn cho development với ASP.NET Core.

### Chạy Redpanda

```bash
cd docker/redpanda
docker-compose up -d
```

### Kiểm tra trạng thái

```bash
docker-compose ps
```

### Truy cập Redpanda Console

- URL: http://localhost:8080
- Xem topics, messages, consumer groups
- Quản lý Schema Registry

### Dừng Redpanda

```bash
docker-compose down
```

### Xóa dữ liệu (reset)

```bash
docker-compose down -v
```

## Kafka (Legacy Reference)

Sử dụng nếu bạn muốn so sánh hoặc cần các tính năng đặc biệt của Kafka.

### Chạy Kafka

```bash
cd docker/kafka
docker-compose up -d
```

### Kiểm tra trạng thái

```bash
docker-compose ps
```

### Truy cập Kafka UI

- URL: http://localhost:8080
- Xem topics, messages, consumer groups

## Services Comparison

| Service | Port | Mục đích | Redpanda | Kafka |
|---------|------|----------|----------|-------|
| Broker | 9092 | Message broker chính | ✅ | ✅ |
| Console/UI | 8080 | Giao diện quản lý | ✅ | ✅ |
| Admin API | 8081 | REST API quản lý | ✅ | ❌ |
| Schema Registry | 8082 | Schema quản lý | ✅ | ✅ |
| Zookeeper | 2181 | Cluster metadata | ❌ | ✅ |

## Troubleshooting

### Redpanda không kết nối được

```bash
# Kiểm tra logs
docker-compose logs redpanda

# Restart services
docker-compose restart
```

### Kafka không kết nối được

```bash
# Kiểm tra logs
docker-compose logs kafka

# Restart services
docker-compose restart
```

### Port đã được sử dụng

```bash
# Kiểm tra process đang dùng port
netstat -ano | findstr :9092
netstat -ano | findstr :8080
```

### Xóa toàn bộ containers và volumes

```bash
# Redpanda
cd docker/redpanda
docker-compose down -v

# Kafka
cd docker/kafka
docker-compose down -v

# Clean toàn bộ
docker system prune -a
```