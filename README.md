# TaskTrack API

ASP.NET Core 8 API for the PRN232 Task Management assignment.

## Run locally

Set `DATABASE_URL` to the existing PostgreSQL database `qe190064_prn232_ass1`, then run:

```powershell
dotnet run --project TaskTrack.API
```

Swagger is available at `/swagger`.

The API exposes public CRUD for departments, projects, tasks, and tags. Tasks use soft-delete; departments, projects, and tags reject deletes while related records exist.
