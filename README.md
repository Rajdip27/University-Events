# University Events

![CI Status](https://github.com/Rajdip27/University-Events/actions/workflows/main.yml/badge.svg)

# 🎓 University Event Management System (MVC)

A **Clean Architecture** based **ASP.NET Core MVC** application to manage university events, student registrations, payments, and food tokens.  
Includes **audit logging**, **smart admin dashboard (dark/light mode)**, and **secure authentication**.  
Now runs easily with **Docker**.

---

## 🐳 Run with Docker (Quick Start)

### 1. Build Docker Image
docker build -t universityevents .

### 2. Run Container
docker run -d -p 8080:80 --name universityevents universityevents

Access the app at 👉 http://localhost:8080

---
## 🚀 Features

- Category & Event Management  
- Student Registration (with Department + ID Card upload)  
- Online Payments (e.g., Stripe)  
- Food Tokens (auto after payment)  
- Audit Logs (track changes)  
- Smart Admin Dashboard (Dark/Light Mode, Sidebar, Footer)  

---

## 🏗️ Clean Architecture Structure

UniversityEvents  
│  
├── UniversityEvents.Core (Entities & Interfaces)  
├── UniversityEvents.Application (Business Logic)  
├── UniversityEvents.Infrastructure (DbContext, Services, Migrations)  
└── UniversityEvents.Web (ASP.NET Core MVC UI, Controllers, Views, Static files)  

---

## 🗄️ Database Relationships

- Category → Event (1:N)  
- Event → StudentRegistration (1:N)  
- StudentRegistration → Payment (1:1)  
- StudentRegistration → FoodToken (1:1)  

---

## 🎨 Admin Dashboard

- Razor Views + TailwindCSS + ShadCN UI  
- Sidebar & footer layout  
- Dark/Light mode toggle  
- Charts: registrations, payments, food tokens  

---

## 📌 Next Improvements

- QR-code scanning for food tokens  
- Email/SMS notifications  
- Multi-payment provider support  
- Attendance tracking  

---

## 🧑‍💻 Author

Developed by **[Santanu Chandra]**  
📧 Contact: Srajdip920@gmail.com
