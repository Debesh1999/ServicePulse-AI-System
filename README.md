# ServicePulse AI System

全栈客服质检系统，展示 Clean Architecture + React + SQLite 完整技术栈。

## 架构分层

| 层 | 职责 | 路径 |
|---|---|---|
| Domain | 实体、枚举、领域规则 | `src/ServicePulse.Domain` |
| Application | DTO、请求、通用服务 | `src/ServicePulse.Application` |
| Infrastructure | EF Core、迁移、数据种子 | `src/ServicePulse.Infrastructure` |
| Api | HTTP 端点、中间件 | `src/ServicePulse.Api` |
| Frontend | React + Reactstrap | `frontend/ServicePulse.Frontend` |

## API 端点

- `GET /api/calls?page=&pageSize=` — 分页查询通话列表
- `GET /api/calls/{id}` — 通话详情（含分析结果、经理审核）
- `GET /api/calls/search?agentName=&customerName=&status=&from=&to=` — 搜索过滤
- `GET /api/calls/stats` — 仪表盘统计
- `POST /api/calls` — 创建新通话
- `GET /api/health` — 健康检查（含数据库连通性）

## 运行步骤

### 后端
```bash
cd backend/src/ServicePulse.Api
dotnet run
# 启动后访问 http://localhost:5295/api/health
```

### 前端
```bash
cd backend/frontend/ServicePulse.Frontend/ClientApp
npm install
npm start
# 浏览器打开 http://localhost:3000
```

### 测试
```bash
cd backend
dotnet test src/ServicePulse.Tests/ServicePulse.Tests.csproj
```

## V1 完成清单

- [x] Calls CRUD 端点（5 个）
- [x] 搜索与分页
- [x] 分析结果与经理审核展示
- [x] Dashboard 统计
- [x] 健康检查端点
- [x] 全局异常处理中间件
- [x] 输入验证（Required、Range、MaxLength）
- [x] 单元测试（9/9 通过）
- [x] 前端 Pager 组件
- [x] Postman 接口验证
