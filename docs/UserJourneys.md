## Creating User Journeys

### Reminder of Requirements

The product requirements for this initial phase are the following:

1. Business users should be able to process a market data contribution through the Market data contribution gateway and
   receive either a successful or unsuccessful response.

2. Business users should be able to retrieve the details of a previously contributed market data. The next section will
   discuss each of these in detail.



## User Journey
### Process a market data contribution

```mermaid

sequenceDiagram
   actor Business User
   participant Market Data Contribution Gateway as API
   participant DB
   participant Market Data Validation Service

   Business User ->>+ Market Data Contribution Gateway: Create Market Data Contribution
   note over Business User, Market Data Contribution Gateway: POST { RequestData }
   par
      Market Data Contribution Gateway ->> DB: Store Market Data Contribution
      note over Market Data Contribution Gateway, DB: { RequestId, RequestData, Status }
   and
      Market Data Contribution Gateway ->> Market Data Validation Service: Validate Market Data
   end
   Market Data Validation Service ->> Market Data Contribution Gateway: Return Validation Response
   Market Data Contribution Gateway ->> DB: Store Result of Validation
   note over Market Data Contribution Gateway, DB: For RequestId { ValidationResponse }
   critical
      Market Data Contribution Gateway ->>- Business User: Return Contribution Response
      note over Market Data Contribution Gateway, Business User: { RequestId, ValidationResponse, Status, CreatedAt }
   option
      Market Data Contribution Gateway ->> Business User: Return Error
      note over Market Data Contribution Gateway, Business User: { Reason }
   end

```

### Retrieve a market data contribution
```mermaid
  
sequenceDiagram
    actor Business User
    participant Market Data Contribution Gateway as API
    participant DB
    participant Market Data Validation Service

    Business User ->>+ Market Data Contribution Gateway: Retrieve Market Data Contribution
    Market Data Contribution Gateway ->>+ DB: Retrieve Market Data Contribution
    DB ->>- Market Data Contribution Gateway: Return Market Data Contribution 
    Market Data Contribution Gateway ->>- Business User: Return Market Data Contribution

```
