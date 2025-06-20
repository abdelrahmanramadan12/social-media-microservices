# Thryve – Backend

![cover](diagrams\Thryve.png)

**Thryve** is a distributed microservices-based social media platform built as the graduation project of the **Integrated Development and Architecture** track at the [Information Technology Institut (ITI)](https://iti.gov.eg/home).

Designed with modern architectural patterns and scalable technologies, Thryve aims to demonstrate enterprise-level backend design for a real-world application scenario, focused on performance, modularity, and scalability.

---

## 👥 Team

* [@miinamaaher1](https://github.com/miinamaaher1)
* [@MahmoudRKeshk](https://github.com/MahmoudRKeshk)
* [@abdelrahmanramadan12](https://github.com/abdelrahmanramadan12)
* [@HadyAbdelhady](https://github.com/HadyAbdelhady)
* [@alyaa999](https://github.com/alyaa999)
* [@Emanabdallah92](https://github.com/Emanabdallah92)

---

## 🧰 Technology Stack

* **.NET 9** – Backend services
* **RabbitMQ** – Event bus for asynchronous messaging
* **MongoDB** – NoSQL database for feeds and chat etc.
* **SQL Server** – Relational database for core data
* **Redis** – Caching layer
* **Docker** – Containerization
* **YARP** – API Gateway
* **SignalR** – Real-time messaging (chat and notifications)
* **Cloudinary** – Object storage for media files

---

## 🏗️ Overall System 

![system diagram](diagrams\system-diagram.svg)

Thryve follows an **event-driven microservices architecture** with a total of **15 microservices**, each focusing on a single business capability.

### ✅ Core Microservices

* **API Gateway**
* **Auth Service**
* **Post Service**
* **Comment Service**
* **Reaction Service**
* **Profile Service**
* **Follow Service**
* **Feed Service**
* **Chat Service**
* **Media Service**
* **Notification Service**

### 🎯 Aggregates (Backend For Frontend - BFF)

* **Post Aggregate**
* **Comment Aggregate**
* **Reaction Aggregate**
* **Follow Aggregate**

The system uses **fan-out** and **CQRS patterns** in specific services (e.g., feed), and relies heavily on **publish/subscribe** model via RabbitMQ.

---

## 📝 Post Service

![post service diagram](diagrams\post-service-diagram.svg)

Handles creation, editing, deletion, and retrieval of user posts.

* Stores posts in MongoDB
* Publishes events for feed, comment and reaction services
* Supports media attachments through the media service
* Validates author identity and user permissions

---

## 💬 Comment Service

![comment service diagram](diagrams\comment-service-diagram.svg)

Manages comments under posts.

* Integrates with reaction service for comment likes
* Publishes events for feed, post, reaction and notification services
* Supports media attachments through the media service

---

## ❤️ Reaction Service

![react service diagram](diagrams\react-service-diagram.svg)

Handles reactions (like/ Unlike) on posts and comments.

* Stores per-user reaction states
* Emits events for for feed, post and notification services

---

## 👤 Profile Service

![profile service diagram](diagrams\profile-service-diagram.svg)

Maintains user profile information and settings.

* Profile updates and avatar uploads
* Public/private profile settings
* Integrated with the media service for avatar storage

---

## ➕ Follow Service

![follow service diagram](diagrams\follow-service-diagram.svg)

Manages user following and follower relationships.

* Maintains follower/following lists
* Triggers feed rebuild and fan-out events
* Integrated with BFF to aggregate mutual followers and suggestions

---

## 📰 Feed Service

![feed service diagram](diagrams\feed-service-diagram.svg)

Implements **Fan-Out on Write** and **CQRS** for timeline generation.

* Stores user timelines in MongoDB
* Listens to post and follow events
* Supports infinite scrolling and pagination

---

## 💬 Chat Service

![chat service diagram](diagrams\chat-service-diagram.svg)

Enables real-time direct messaging between users.

* Built using **SignalR** for real-time communication
* Messages stored in MongoDB
* Supports delivery and seen acknowledgments

---

## 🧠 BFF (Backend For Frontend)

![comment aggregate service diagram](diagrams\comment-aggregate-service-diagram.svg)

Provides tailored APIs to the frontend application by aggregating data from multiple microservices.

* **Post Aggregate** – Post + author
* **Comment Aggregate** – Comment + author
* **Reaction Aggregate** – profiles who reacted
* **Follow Aggregate** – followers / following profiles

The BFF layer helps optimize API calls and reduce frontend complexity.

---

## 🔔 Notification Service

![notification service diagram](diagrams\notification-service-diagram.svg)

Enables real-time push notificatons.

* Built using **SignalR** for real-time push notificatons
* notificatons stored in MongoDB
* Supports seen acknowledgments
  