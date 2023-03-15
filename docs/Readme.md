# SocGen Market Data Contribution Gateway

## Docs
### [Task](./Task.md)
The task that was set, with my notes on assumptions made

### [User Journeys](./UserJourneys.md)
Markdown files to document the user journeys

### [Contribution Service](./ContributionService.md)
Markdown files to document the contribution service

### [Validation Service](./ValidationService.md)
Markdown files to document the validation service

## Approach

### 1. Assumptions & Design & BDD
I first read the task and made some assumptions, which I documented in the [Task](./Task.md) document.

I then created a [User Journeys](./UserJourneys.md) document to visually lay out the user journeys. 
Ultimately it's the User journey that drives the design of the solution. This is a key hallmark of BDD and I try to always approach solution implementation from this perspective.

I then created separate documents for both the [Contribution Service](./ContributionService.md) and the [Validation Service](./ValidationService.md) where I could break down what I would expect a simple request, response to look like, and also the HTTP methods used. 

### 2. TDD
Once isolating that the Contribution Service would be the core of the solution, I started to write integration tests for the happy path and unhappy path. I used the [Contribution Service](./ContributionService.md) document to help me with this.

Once I had a basic creation pattern created I refactored the code into a service class to make it more testable.

I then added in a mock for the Validation Service, and wrote a test to ensure that the Validation Service was called.


## Code style, patterns, and practices
I have chosen to showcase my skills in the following areas:
- TDD & BDD
- SOLID
- Clean Code
- Functional Programming
- Domain Driven Design

I opted for using the OneOf library so I could leverage Discriminated Unions and LangExt.Core so I could have Either generics. This helped me achieve two things:
- Allow a control flow that doesn't rely on exceptions
- Better model the domain ideas of the solution

A nice bonus of these decisions is that my method signatures fully describe the possible responses of my methods, bar exceptional cases. 
There are no hidden user exceptions ( .NET SDK still heavily relies on exceptions ), so these can still throw, as they should.

I hope that this solution showcases my understanding of both OOP and functional paradigms.
I believe that functional programming is a good way to reduce bugs and improve code quality.


## Solution

### 1. Complete base requirement for task using TDD 
#### a. simple way based of user stories
Integration TDD
- [x] Add a test for the happy path
- [x] Add a test for the unhappy path
- [x] Add in SurrealDB client (a client for my database of choice)
- [x] Create interface and mock with NSubstitute
- [x] Add TestContainers for integration tests

### 2. Add logging
- [ ] Add logging and logging templates

### 3. Add metrics - this means "Add Telemetry"
- [ ] Tracing
- [ ] Logging 
- [ ] Metrics

## Further steps that could be taken

### 4. Add containerization
- Add Dockerfile
- Add docker-compose

### 5. Add Terraform IaC
- Add Terraform IaC
 
### 6. Add CI/CD
- Github Actions to build, test and deploy to Azure
