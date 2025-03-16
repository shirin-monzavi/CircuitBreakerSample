# Circuit Breaker Pattern in C#

## 📌Overview

The Circuit Breaker Pattern is a resilience pattern used to prevent a system from continuously trying to execute an operation that is likely to fail. It acts as a switch that stops requests to a failing service and allows it to recover before resuming normal operations.

## 🎯 When to Use the Circuit Breaker Pattern?

✅ To prevent cascading failures in a distributed system.

✅ When dealing with unreliable external services (APIs, databases, microservices).

✅ To avoid overwhelming a failing service with repeated requests.

✅ To improve fault tolerance and system stability.

## How It Works

The circuit breaker has three states:

Closed: Requests pass through normally.

Open: Requests are blocked, and an error is returned immediately.

Half-Open: A limited number of requests are allowed to check if the service has recovered.

## 🤝 Contributing
Feel free to fork the repository, submit pull requests, or open issues for improvements and discussions.
