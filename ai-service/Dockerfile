# Build stage
FROM python:3.12-slim AS build
WORKDIR /app

# Install dependencies
COPY ai-service/app/requirements.txt .
RUN pip install --no-cache-dir --prefix=/install -r requirements.txt

# Runtime stage
FROM python:3.12-slim AS runtime
WORKDIR /app

# Copy installed packages
COPY --from=build /install /usr/local

# Copy application code
COPY ai-service/app/ ./

# Expose port
EXPOSE 8000

# Run with uvicorn
CMD ["uvicorn", "main:app", "--host", "0.0.0.0", "--port", "8000"]
