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

## How to run
### Prerequisites
- Docker
- .NET Core 7.0 SDK
- Running Surreal Db instance, configured within the appsettings.Development.json - A docker container for this can be started using the Powershell script located at `/libs/surrealdb-client/run-surrealdb.ps1`

### Calling the API
You can use the Swagger UI to call the API, or use Postman.
You can find the Swagger UI at `http://localhost:7133/swagger/index.html`
Details of the API request body can be found in the docs at [Contribution Service](./ContributionService.md)

### Running the tests
I have included both unit tests and integration tests.

Running the integration tests utilises TestContainers to spin up a docker container for the SurrealDb database.

Hence keep in mind you need docker running for the integration tests to pass.

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

#### 2.a Integration testing
Mocking at the boundaries of the system is a good way to ensure that the system is decoupled and can be tested in isolation.
This is why I've used integration tests and TestContainers 

### 3. Database
I opted to use a real database that I can spin up in a docker container, rather than an in memory database.

I have been working on a client for SurrealDB recently as a personal endeavour, so I decided to use that for this task as it is quick, lightweight and provides a nice SQL like interface.

The client for this can be found in the `/libs/surrealdb-client` folder.

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
There are no hidden user exceptions ( .NET SDK still heavily relies on exceptions ), so these can still throw in exceptional circumstances, as they should.

I hope that this solution showcases my understanding of both OOP and functional paradigms.

## Points to note

### Use of non-idiomatic code
The functional paradigms used are not idiomatic to C# and .NET, but I believe that the benefits out weight the learning curve,

I believe that functional programming is a good way to reduce bugs and improve code quality which is why I included it. 

That said I certainly can produce quality code whatever the paradigm and adapt to the preferences of the team. 
I would be intrigued on your perspectives on this.

### Adding invalid contributions to the database
It could be interpreted that the task is suggesting we don't add contributions to the database unless they are valid.
In practice the business user of the system might want to see all contributions, even if they are invalid for various reasons, for example they may not know they are getting items invalidated, maybe it's a upstream service, or the task is automated etc...

### If I were going further with this I would consider the following:
- Better model errors in domain, and refactor into `Either<Error, Success>` pattern
- Remove public constructors for my domain model, this was done for brevity, however in a full solution I would control creation of those entities better and implement custom Json converters to ensure that the domain model is not instantiated incorrectly.
I also wouldn't want my domain model to know about my infrastructure layer, currently it is using a record from the infrastructure, in practice we could use a value object for the `MarketDataValue` type

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
