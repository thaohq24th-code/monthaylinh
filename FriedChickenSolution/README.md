# CakeTea Web - Hướng dẫn Build & Deploy Somee.com

## Yêu cầu
- .NET 8 SDK (https://dotnet.microsoft.com/download/dotnet/8)
- `dotnet-ef` tool: `dotnet tool install --global dotnet-ef`

---

## Bước 1 — Cài đặt EF Core Tools (chỉ cần 1 lần)
```
dotnet tool install --global dotnet-ef
```

---

## Bước 2 — Restore packages
```
dotnet restore
```

---

## Bước 3 — Tạo Migration
Chạy từ thư mục gốc (nơi có file `CakeTeaSolution.sln`):
```
dotnet ef migrations add InitialCreate --project Core.Database --startup-project Web
```

---

## Bước 4 — Cập nhật database (tuỳ chọn - chỉ khi test local)
Đảm bảo connection string trong `Web/appsettings.json` trỏ đúng tới database.
```
dotnet ef database update --project Core.Database --startup-project Web
```
> Nếu deploy Somee: bỏ qua bước này — app tự chạy migration khi khởi động.

---

## Bước 5 — Publish
```
dotnet publish Web/Web.csproj -c Release -o ./publish
```

---

## Bước 6 — Upload lên Somee.com
Nén thư mục `./publish` thành zip → upload lên Somee File Manager → Extract Here.

Cấu trúc thư mục gốc trên Somee phải là:
```
web.config
Web.dll
Web.exe
appsettings.json
wwwroot/
logs/
```

---

## Tài khoản mặc định sau khi deploy
| Tài khoản | Mật khẩu | Quyền |
|-----------|----------|-------|
| admin     | admin123 | Admin |
| customer  | admin123 | Customer |

---

## Lưu ý quan trọng
- `web.config` đã được cấu hình `hostingModel="outofprocess"` — BẮT BUỘC cho Somee.com
- Thư mục `wwwroot/uploads/` đã có sẵn để lưu ảnh upload
- Thư mục `logs/` đã có sẵn để lưu log khi web chạy
