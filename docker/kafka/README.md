# Kafka Development Environment

Docker Compose setup cho Apache Kafka để phát triển tính năng Activity Log trong HouseLinkAPI.

## Quick Start

```bash
# Khởi động Kafka
docker-compose up -d

# Kiểm tra trạng thái
docker-compose ps

# Truy cập Kafka UI
# http://localhost:8080
```

## Services

### 1. Zookeeper (port 2181)
- Quản lý Kafka cluster metadata
- Lưu trữ broker information và topic metadata
- Điều phối leader election

### 2. Kafka Broker (port 9092)
- Message broker chính
- Topic: `activity-logs` (sẽ được tạo tự động)
- Replication factor: 1 (development)

### 3. Kafka UI (port 8080)
- Giao diện web quản lý Kafka
- Xem topics, messages, consumer groups
- Quản lý partitions và offsets

## Cấu hình Kafka cho .NET

### Connection String
```
localhost:9092
```

### Cấu hình trong appsettings.json
```json
{
  "Kafka": {
    "BootstrapServers": "localhost:9092",
    "ActivityLogTopic": "activity-logs",
    "GroupId": "house-link-identity"
  }
}
```

## Các lệnh hữu ích

### Tạo topic mới
```bash
docker exec -it house-link-kafka-1 kafka-topics --create \
  --topic activity-logs \
  --bootstrap-server localhost:9092 \
  --partitions 3 \
  --replication-factor 1
```

### Xem danh sách topics
```bash
docker exec -it house-link-kafka-1 kafka-topics --list \
  --bootstrap-server localhost:9092
```

### Xem messages trong topic
```bash
docker exec -it house-link-kafka-1 kafka-console-consumer \
  --topic activity-logs \
  --bootstrap-server localhost:9092 \
  --from-beginning
```

### Xem consumer groups
```bash
docker exec -it house-link-kafka-1 kafka-consumer-groups --list \
  --bootstrap-server localhost:9092
```

## Development Workflow

1. **Khởi động Kafka**: `docker-compose up -d`
2. **Chạy ứng dụng**: dotnet run từ thư mục API
3. **Kiểm tra messages**: Truy cập Kafka UI hoặc dùng console consumer
4. **Dừng services**: `docker-compose down`

## Troubleshooting

### Kafka không kết nối được
```bash
# Kiểm tra logs
docker-compose logs kafka

# Kiểm tra network
docker network ls
docker network inspect house-link_default
```

### Port conflict
```bash
# Kiểm tra process đang dùng port
netstat -ano | findstr :9092
netstat -ano | findstr :2181
netstat -ano | findstr :8080
```

### Reset hoàn toàn
```bash
docker-compose down -v
docker system prune -a
docker volume prune
```

## Lưu ý

- **Development only**: Cấu hình này chỉ dành cho môi trường phát triển
- **Single broker**: Chỉ có 1 broker, không phải production cluster
- **No persistence**: Dữ liệu sẽ mất khi dừng container (trừ khi dùng volumes)
- **No security**: Không có SSL/TLS hoặc authentication