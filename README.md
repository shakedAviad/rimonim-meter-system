# Meter System Backend

## Project Overview

A backend system for ingesting meter readings, processing them asynchronously through RabbitMQ, and persisting them into PostgreSQL with idempotent handling.

The solution is containerized and deployed using Kubernetes.

---

## Prerequisites

Before running the project, ensure the following tools are installed:

- .NET 10 SDK
- Docker Desktop
- Minikube
- kubectl
- Git Bash

---

## Running the Project

Start Minikube:

```bash
minikube start
```

Deploy the full environment:

```bash
bash deploy.sh
```

The deployment script:

- Deploys RabbitMQ
- Deploys PostgreSQL
- Initializes the database schema
- Publishes and deploys the API
- Publishes and deploys the Worker

---

## Verify Deployment

Check pods:

```bash
kubectl get pods
```

Expected running services:

```text
metersystem-api
metersystem-worker
postgres
rabbitmq
```

Check Kubernetes services:

```bash
kubectl get svc
```

---

## Access API

Generate an external URL:

```bash
minikube service metersystem-api --url
```

Open:

```text
http://localhost:<port>/scalar
```

The API is exposed through a Kubernetes `NodePort` service to satisfy the requirement of external cluster accessibility.

---

## Example Request

```json
{
  "meter_number": 12345,
  "readings": {
    "2026-03-18T10:15:00Z": 1234.56,
    "2026-03-18T10:00:00Z": 1234.51
  }
}
```

Expected response:

```text
202 Accepted
```

OpenAPI/Scalar includes a predefined example request automatically.

### Invalid Request Example

```json
{
  "meter_number": "invalid",
  "readings": {
    "bad-date": "wrong-value"
  }
}
```

Expected response:

```text
400 Bad Request
```

Validation is intentionally limited to request format validation according to task requirements.

---

## Verify Persistence

RabbitMQ:

```bash
kubectl exec -it deployment/rabbitmq -- rabbitmqctl list_queues name messages_ready messages_unacknowledged consumers
```

Expected:

```text
meter-readings 0 0 1
```

PostgreSQL:

Get the PostgreSQL pod:

```bash
kubectl get pods
```

Connect to PostgreSQL:

```bash
kubectl exec -it deployment/postgres -- psql -U postgres -d meters
```

Verify persisted data:

```sql
select * from meters;

select * from meter_readings;

select
m.meter_number,
r.*
from meter_readings r
join meters m
on m.meter_id = r.meter_id
order by m.meter_number;
```

---

## Notes

- Duplicate readings are deduplicated by `(meter_id, value_at)` using a first-write-wins approach.
- Validation intentionally checks request format only according to task requirements
- Uses .NET 10 and modern C# features
- OpenAPI/Scalar request examples are configured automatically

---

## Future Improvements

- Dead-letter queue support
- Retry policy
- Metrics and observability
- Health checks
- Raw endpoint implementation
