## SocGen Market Data Contribution Gateway

### Docs

#### [Task](./Task.md)
The task that was set, with my notes on assumptions made

#### [User Journeys](./UserJourneys.md)
Markdown files to document the user journeys

#### [Contribution Service](./ContributionService.md)
Markdown files to document the contribution service

#### [Validation Service](./ValidationService.md)
Markdown files to document the validation service


### Solution


## Steps for me
### 1. Complete base requirement for task using TDD 
#### a. simple way based of user stories
Integration TDD
- [ ] Add a test for the happy path
- [ ] Add a test for the unhappy path
- [ ] Add in SurrealDB client (a client for my database of choice)
- [ ] Create interface and mock with NSubstitute

### 2. Add logging
- [ ] Add logging and logging templates

### 3. Add metrics - this means "Add Telemetry"
- [ ] Tracing
- [ ] Logging 
- [ ] Metrics

### 4. Add containerization
- [ ] Add Dockerfile
- [ ] Add docker-compose

### 5. Add Terraform IaC
- [ ] Add Terraform IaC
 
### 6. Add CI/CD
- [ ] Github Actions to build, test and deploy to Azure
