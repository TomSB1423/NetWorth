# Database Setup Instructions

## PostgreSQL with Docker

This project uses PostgreSQL running in a Docker container for local development.

### Starting the Database

1. Make sure Docker is running on your machine
2. From the project root directory, run:

   ```bash
   docker-compose up -d postgres
   ```

### Stopping the Database

```bash
docker-compose down
```

### Accessing the Database

You can connect to the PostgreSQL database using:

- **Host**: localhost
- **Port**: 5432
- **Database**: networth
- **Username**: networth_user
- **Password**: networth_password

### Using psql CLI

```bash
docker exec -it networth-postgres psql -U networth_user -d networth
```

### Database Initialization

The `init-scripts/01-init.sql` file contains initialization scripts that run when the container is first created. Add any schema or seed data there.

### Environment Variables

Database configuration is stored in `.env` file. Update the values there if needed.

### Connection String

The Azure Functions project uses this connection string format in `settings.json`:

```text
Host=localhost;Port=5432;Database=networth;Username=networth_user;Password=networth_password;Pooling=true;Minimum Pool Size=1;Maximum Pool Size=20;
```

### Data Persistence

Database data is persisted in a Docker volume named `postgres_data`. To completely reset the database:

```bash
docker-compose down
docker volume rm networth_postgres_data
docker-compose up -d postgres
```
