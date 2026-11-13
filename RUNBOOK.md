# ServicePulse AI System — Runbook

## 1. 环境要求

| 组件 | 版本 |
|---|---|
| .NET SDK | 10.0.x |
| Node.js | 18+ |
| 数据库 | SQLite（自动创建，无需安装） |

## 2. 首次运行

### 2.1 后端
```bash
cd backend/src/ServicePulse.Api
dotnet run
```
启动后访问 `http://localhost:5295/api/health`，预期返回：
```json
{"status":"Healthy","environment":"Development","database":"Connected","timestamp":"..."}
```
> 启动时会自动执行迁移并种子数据。

### 2.2 前端
```bash
cd backend/frontend/ServicePulse.Frontend/ClientApp
npm install
npm start
```
浏览器打开 `http://localhost:3000`。

### 2.3 测试
```bash
cd backend
dotnet test src/ServicePulse.Tests/ServicePulse.Tests.csproj
```
预期：`Passed! - Failed: 0, Passed: 9`

## 3. 常用操作

### 3.1 停止服务
```bash
# 停止 API
taskkill //F //IM dotnet.exe

# 停止前端
# 在 npm start 窗口按 Ctrl+C
```

### 3.2 重置数据库
```bash
cd backend/src/ServicePulse.Api
rm ServicePulseDb.sqlite
dotnet run
```
迁移会自动重新创建表并种子数据。

### 3.3 新增 API 端点
1. 在 `src/ServicePulse.Api/Controllers/` 创建新 Controller
2. 遵循 `[ApiController]` + `[Route("api/[controller]")]` 约定
3. 重启 API 即可生效

### 3.4 前端修改
```bash
cd backend/frontend/ServicePulse.Frontend/ClientApp
npm start
```
支持热重载，修改后自动刷新。

## 4. Postman 测试

### 4.1 创建 Collection
导入以下请求（URL 均为 `http://localhost:5295`）：

| # | 名称 | 方法 | URL | 说明 |
|---|---|---|---|---|
| 1 | Get Calls | GET | `/api/calls?page=1&pageSize=10` | 分页查询 |
| 2 | Get Call | GET | `/api/calls/{id}` | 详情（含分析/审核） |
| 3 | Search by Agent | GET | `/api/calls/search?agentName=Alice` | 搜索 |
| 4 | Search by Status | GET | `/api/calls/search?status=Completed` | 状态过滤 |
| 5 | Get Stats | GET | `/api/calls/stats` | Dashboard 统计 |
| 6 | Create Call | POST | `/api/calls` | 创建通话 |
| 7 | Create - Not Found | POST | `/api/calls` | agent/customer 不存在 |
| 8 | Unknown ID | GET | `/api/calls/00000000-0000-0000-0000-000000000000` | 404 测试 |

### 4.2 Create Call 请求体
```json
{
  "agentName": "Alice Johnson",
  "customerName": "Sarah O'Brien",
  "externalCallId": "EXT-001",
  "startedAtUtc": "2026-01-01T10:00:00Z",
  "durationSeconds": 120,
  "transcriptText": "Hello this is a test call"
}
```

## 5. 故障排查

| 症状 | 可能原因 | 解决 |
|---|---|---|
| 端口 5295 被占用 | 旧进程未退出 | `taskkill //F //IM dotnet.exe` |
| 前端白屏 | npm 依赖未安装 | `npm install` |
| SQLite 锁定 | 多进程同时写入 | 关闭所有 dotnet 进程后重启 |
| 测试失败 | DLL 被占用 | `taskkill //F //IM dotnet.exe` 后重新构建 |
| 404 on /api/health | HealthController 未创建 | 确认文件存在：`src/ServicePulse.Api/Controllers/HealthController.cs` |

## 6. 项目结构

```
ServicePulse_AI_System/
├── backend/
│   ├── src/
│   │   ├── ServicePulse.Domain/       # 实体、枚举
│   │   ├── ServicePulse.Application/  # DTO、请求
│   │   ├── ServicePulse.Infrastructure/  # EF Core、迁移
│   │   ├── ServicePulse.Api/         # Controllers、Middleware
│   │   └── ServicePulse.Tests/       # xUnit 单元测试
│   └── frontend/
│       └── ServicePulse.Frontend/    # React + Reactstrap
├── database/                          # 种子数据 SQL
├── docs/                              # 架构文档
└── README.md
```