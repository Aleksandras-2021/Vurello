# Vurello - Task Management App

## About

A full-stack Trello-inspired task management application with a live demo available at [vurello.click](https://vurello.click).

## Project Overview

This system provides:
- Kanban-style boards with draggable task cards
- JWT-based authentication with role-based access control
- Team collaboration with role management
- Fully containerized with automated deployments via GitHub Actions CI/CD

## Repository Structure

- [`PSK.Server/`](PSK.Server): ASP.NET Core Web API backend
- [`psk.client/`](psk.client): React frontend

## Tech Stack

### Backend
![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=csharp&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-%235C2D91.svg?style=for-the-badge&logo=dotnet&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/postgresql-%23316192.svg?style=for-the-badge&logo=postgresql&logoColor=white)
![Docker](https://img.shields.io/badge/docker-%230db7ed.svg?style=for-the-badge&logo=docker&logoColor=white)

### Frontend
![React](https://img.shields.io/badge/react-%2320232a.svg?style=for-the-badge&logo=react&logoColor=%2361DAFB)
![JavaScript](https://img.shields.io/badge/javascript-%23323330.svg?style=for-the-badge&logo=javascript&logoColor=%23F7DF1E)
![Vite](https://img.shields.io/badge/vite-%23646CFF.svg?style=for-the-badge&logo=vite&logoColor=white)

### Infrastructure
![AWS EC2](https://img.shields.io/badge/AWS%20EC2-%23FF9900.svg?style=for-the-badge&logo=amazonec2&logoColor=white)
![Cloudflare](https://img.shields.io/badge/Cloudflare-F38020?style=for-the-badge&logo=Cloudflare&logoColor=white)
![GitHub Actions](https://img.shields.io/badge/github%20actions-%232671E5.svg?style=for-the-badge&logo=githubactions&logoColor=white)

## How to Run Locally

1. Make sure you have Docker and Docker Compose installed
2. Clone the repository
3. Create a `.env` file in the root with the following variables:
```env
POSTGRES_DB=your_db
POSTGRES_USER=your_user
POSTGRES_PASSWORD=your_password
```
4. Run the stack:
```bash
docker compose up --build
```
- Backend available at `http://localhost:5000`
- Swagger available at `http://localhost:5000/swagger`
- Frontend available at `http://localhost:3000`
5. Alternatively(without Docker),  perform `dotnet ef database update` and `dotnet run --launch-profile "https"`
## CI/CD Pipeline

Every merge to `main` triggers a GitHub Actions workflow that:
1. Runs automated tests
2. Builds Docker images
3. Transfers images to AWS EC2 via SSH
4. Remotely shuts down the old version and starts the new one

## Troubleshooting

**Frontend fails to launch on Windows:**
```bash
net stop winnat
net start winnat
```

## Preview

### Team Board
![image](https://github.com/user-attachments/assets/fa1ea151-bcf9-41c9-96c4-b33e294b7516)

### Tasks
![image](https://github.com/user-attachments/assets/3c987f5e-2525-4002-a3ad-dc6791cbdf53)

### Role Management
![image](https://github.com/user-attachments/assets/5061aabd-a6d9-40b8-94a5-074ee4df30be)
![image](https://github.com/user-attachments/assets/c4d61e01-5953-4d29-90a2-fa7107843e2e)
