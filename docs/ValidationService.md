## Validation Service

### Brief
In your solution you should simulate or otherwise mock the validation part. This component should be
able to be switched out for a real validation service once we move to production. We should assume
that a validation service returns a unique identifier and a status that indicates whether the validation
was successful or not.

### Purpose
Allows to check:
- the market data contribution rights
- format (example: Negative FxQuote), 
- legal auditing (Financial regulation framework validation such as MIFID, etc.) 
 
Responds with an appropriate response code

