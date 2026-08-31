# School Scheduler - Project Guide

## Project Overview
Multi-tenant ASP.NET Core web backend that generates weekly class timetables for schools using a CP-SAT solver.

## Domain Model
- **Multi-tenancy**: Shared database. Every tenant-scoped table has a `SchoolId` foreign key.
- **School Configuration**:
    - Custom working days per week.
    - Custom lecture count/times and break count/times.
    - Grades: 12 grades divided into Primary (1-6), Intermediate (1-3), and Secondary (1-3).
    - Each grade has one or more classes (sections).
- **Teachers**:
    - Belong to a school (can work at multiple schools).
    - Teach one or more subjects.
    - Have a weekly availability (specific day + lecture-slot combinations).
- **Class Assignments**:
    - Mapping of teacher $\rightarrow$ subject $\rightarrow$ class.
    - Weekly quota (number of lectures per week) for each subject per class.

## Scheduling Constraints
1. **No Double-Booking**: A teacher cannot be in two places at once (same day+slot).
2. **Availability**: Teachers are only scheduled within their declared availability.
3. **Quota Fulfillment**: Each class must receive exactly the required weekly quota per subject.
4. **Subject Limit**: A subject appears at most twice per day for a given class.
5. **Fatigue Limit**: No teacher can have 3 or more consecutive lectures without a break.

## Architecture
Clean Architecture with 4 projects:
- `SchoolScheduler.Domain`: Entities, value objects, enums. No dependencies.
- `SchoolScheduler.Application`: CQRS (MediatR), interfaces, FluentValidation, DTOs.
- `SchoolScheduler.Infrastructure`: EF Core, multi-tenant query filters, Google OR-Tools CP-SAT solver implementation.
- `SchoolScheduler.Api`: Controllers, auth, DI composition root.

## Tech Stack
- **Framework**: ASP.NET Core (latest LTS)
- **ORM**: EF Core
- **Patterns**: CQRS via MediatR
- **Validation**: FluentValidation
- **Mapping**: AutoMapper
- **Solver**: `Google.OrTools` (CP-SAT)

## Multi-tenancy Implementation
- Shared database.
- Mandatory `SchoolId` on all tenant-scoped tables.
- Enforced via EF Core **Global Query Filters** based on the authenticated school's ID.
- *Rule*: Never rely on manual filtering in query code.
