# Redpanda Development Environment

Docker Compose setup cho Redpanda để phát triển tính năng Activity Log trong HouseLinkAPI. Redpanda là Kafka-compatible message broker nhẹ hơn và nhanh hơn.

## Quick Start

```bash
# Khởi động Redpanda
docker-compose up -d

# Kiểm tra trạng thái
docker-compose ps

# Truy cập Redpanda Console
# http://localhost:8080
```

## Services

### 1. Redpanda Broker (port 9092)
- Kafka-compatible message broker
- Không cần Zookeeper
- Tích hợp Schema Registry
- Topic: `activity-logs` (sẽ được tạo tự động)

### 2. Redpanda Console (port 8080)
- Giao diện web quản lý Redpanda/Kafka
- Xem topics, messages, consumer groups
- Quản lý Schema Registry

### 3. Ports khác
- **8081**: Admin API
- **8082**: Schema Registry
- **9644**: Metrics

## Lợi ích của Redpanda so với Kafka

- **Nhẹ hơn**: Không cần Zookeeper, chỉ 1 container
- **Nhanh hơn**: Performance tốt hơn Kafka
- **Dễ setup**: Cấu hình đơn giản hơn
- **Kafka-compatible**: Sử dụng client Confluent.Kafka như bình thường
- **Tích hợp Schema Registry**: Built-in, không cần service riêng

## Cấu hình Redpanda cho .NET

### Connection String
```
localhost:9092
```

### Cấu hình trong appsettings.json
```json
{
  "Redpanda": {
    "BootstrapServers": "localhost:9092",
    "ActivityLogTopic": "activity-logs",
    "GroupId": "house-link-identity"
  }
}
```

## Các lệnh hữu ích

### Tạo topic mới
```bash
docker exec -it redpanda-broker rpk topic create activity-logs \
  --partitions 3 \
  --replicas 1
```

### Xem danh sách topics
```bash
docker exec -it redpanda-broker rpk topic list
```

### Xem messages trong topic
```bash
docker exec -it redpanda-broker rpk topic consume activity-logs \
  --offset oldest \
  --num 10
```

### Xem consumer groups
```bash
docker exec -it redpanda-broker rpk group list
```

## Development Workflow với ASP.NET Core

1. **Khởi động Redpanda**: `docker-compose up -d`
2. **Thêm package**: `dotnet add package Confluent.Kafka`
3. **Cấu hình appsettings.json** với Redpanda connection
4. **Chạy ứng dụng**: `dotnet run` từ thư mục API
5. **Kiểm tra messages**: Truy cập Redpanda Console tại http://localhost:8080

## Cấu hình client .NET

```csharp
var config = new ProducerConfig
{
    BootstrapServers = "localhost:9092",
    Acks = Acks.All,
    EnableIdempotence = true
};

using var producer = new ProducerBuilder<Null, string>(config).Build();
```

## Troubleshooting

### Redpanda không kết nối được
```bash
# Kiểm tra logs
docker-compose logs redpanda

# Kiểm tra network
docker network ls
docker network inspect docker_redpanda_default
```

### Port conflict
```bash
# Kiểm tra process đang dùng port
netstat -ano | findstr :9092
netstat -ano | findstr :8080
```

### Reset hoàn toàn
```bash
docker-compose down -v
docker system prune -a
docker volume prune
```

## Tích hợp với ASP.NET Core

Redpanda hoàn toàn tương thích với `Confluent.Kafka` package. Bạn có thể:

1. **Producer**: Gửi activity log events từ Identity Service
2. **Consumer**: Xử lý logs trong service riêng hoặc background service
3. **Schema Registry**: Sử dụng Avro schemas cho structured events

## Lưu ý

- **Kafka API compatibility**: Redpanda hỗ trợ Kafka API v1.0+
- **No Zookeeper**: Redpanda không cần Zookeeper
- **Single node**: Cấu hình này chỉ dành cho development
- **Persistence**: Dữ liệu được lưu trong volume `redpanda-data`