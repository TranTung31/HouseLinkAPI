# HouseLinkAPI

HouseLinkAPI là một hệ thống API Gateway và Identity Service được xây dựng trên nền tảng ASP.NET Core, sử dụng kiến trúc microservices với YARP Reverse Proxy và Entity Framework Core.

## Kiến trúc

Dự án được chia thành các thành phần chính:

### 1. Gateway (HouseLink.Gateway)
- **Công nghệ**: ASP.NET Core, YARP Reverse Proxy
- **Chức năng**:
  - Là điểm vào duy nhất cho toàn bộ hệ thống (API Gateway)
  - Xử lý xác thực JWT
  - Phân phối yêu cầu đến các service phía sau
  - Rate limiting và bảo mật
  - CORS configuration
  - Health checks

### 2. Identity Service (HouseLink.Identity)
- **Công nghệ**: ASP.NET Core, Entity Framework Core, SQL Server
- **Kiến trúc**: Clean Architecture (Domain, Application, Infrastructure, API)
- **Chức năng**:
  - Quản lý xác thực và ủy quyền người dùng
  - JWT token generation và refresh
  - Quản lý roles và permissions
  - Entity Framework migrations

## Công nghệ sử dụng

- **Backend**: ASP.NET Core 8.0
- **ORM**: Entity Framework Core 8.0
- **Database**: SQL Server
- **API Gateway**: YARP Reverse Proxy
- **Authentication**: JWT Bearer Tokens
- **Architecture**: Clean Architecture / Microservices
- **Rate Limiting**: AspNetCoreRateLimit
- **CORS**: Cross-Origin Resource Sharing

## Cấu trúc thư mục

```
src/
├── Gateways/
│   └── HouseLink.Gateway/     # API Gateway sử dụng YARP
└── Services/
    └── HouseLink.Identity/    # Identity Service
        ├── HouseLink.Identity.API        # API layer
        ├── HouseLink.Identity.Application # Application layer  
        ├── HouseLink.Identity.Domain     # Domain layer
        └── HouseLink.Identity.Infrastructure # Infrastructure layer
```

## Cài đặt và chạy

### Yêu cầu hệ thống
- .NET 8.0 SDK
- SQL Server (Express hoặc Full)
- Visual Studio hoặc VS Code

### Cấu hình

1. **Cập nhật chuỗi kết nối trong `appsettings.json`**:
   ```json
   {
     "ConnectionStrings": {
       "Default": "Data Source=your_server;Initial Catalog=HouseLinkDB;Integrated Security=True;Trust Server Certificate=True"
     }
   }
   ```

2. **Chạy migrations**:
   ```bash
   cd src/Services/HouseLink.Identity/HouseLink.Identity.API
   dotnet ef database update
   ```

3. **Chạy các dịch vụ**:
   - Gateway: `cd src/Gateways/HouseLink.Gateway && dotnet run`
   - Identity: `cd src/Services/HouseLink.Identity/HouseLink.Identity.API && dotnet run`

## Tính năng nổi bật

- **API Gateway**: Tất cả yêu cầu đi qua một điểm duy nhất với xử lý xác thực tập trung
- **JWT Authentication**: Xác thực dựa trên token với thời gian sống và refresh token
- **Rate Limiting**: Giới hạn số lượng request từ một IP trong khoảng thời gian
- **Forward Claims**: Truyền thông tin người dùng xuống các service phía sau
- **Health Checks**: Giám sát tình trạng hoạt động của hệ thống
- **Clean Architecture**: Tách biệt giữa business logic và infrastructure
- **Entity Framework Migrations**: Quản lý schema database dễ dàng

## Endpoint Routing

Gateway sử dụng YARP để phân phối yêu cầu:
- `/api/auth/**` → Identity Service
- `/api/listings/**` → Listing Service
- `/api/listings/public/**` → Public Listing Service
- `/api/notifications/**` → Notification Service

## Bảo mật

- JWT token validation
- Rate limiting chống DDoS
- CORS configuration
- Forward user claims đến các service phía sau

## Môi trường hỗ trợ

- Development: `http://localhost:5006` (Identity API)
- Frontend origins: `http://localhost:3000`, `https://yourdomain.com`

## Đóng góp

1. Fork dự án
2. Tạo branch mới (`git checkout -b feature/amazing-feature`)
3. Commit thay đổi (`git commit -m 'Add some amazing feature'`)
4. Push lên branch (`git push origin feature/amazing-feature`)
5. Tạo Pull Request