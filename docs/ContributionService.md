## Contribution Service

### Process a market data contribution

The market data contribution gateway will need to provide business users with a way to process a contribution.

To do this, Business users should be able to submit a request to the gateway.

A contribution request should **include appropriate fields such as market data type and the market data itself**.

Market data **could be of any type and structure** but to keep it simple the contribution should be able to process at
least Fx quotes (ex: FxQuote, EUR/USD, bid, ask).

```
POST /contribution
{
    "marketDataType": "FxQuote",
    "marketData": {
        "currencyPair": "EUR/USD",
        "bid": 1.1234,
        "ask": 1.1236
    }
}
```

Another example with a different market data type:


```
POST /contribution
{
    "marketDataType": "Future",
    "marketData": {
        "sybmol": "ES",
        "bid": 3952.25,
        "ask": 3952.50
    }
}
```


### Retrieve a market data contribution

Business users should be able to retrieve the details of a previously contributed market data.

```
GET /contribution/{id:guid}

Returns 
{
    "id": "00000000-0000-0000-0000-000000000000",
    "marketDataType": "FxQuote",
    "marketData": {
        "currencyPair": "EUR/USD",
        "bid": 1.1234,
        "ask": 1.1236
    },
    "status": "Validated"
    "createdDate": "2020-01-01T00:00:00Z"
}
```

### Retrieve all market data contributions

Business users should be able to retrieve all market data contributions.

```
GET /contribution

[
    {
        "id": "00000000-0000-0000-0000-000000000000",
        "marketDataType": "FxQuote",
        "marketData": {
            "currencyPair": "EUR/USD",
            "bid": 1.1234,
            "ask": 1.1236
        },
        "status": "Validated"
        "createdDate": "2020-01-01T00:00:00Z"
    },
    {
        "id": "00000000-0000-0000-0000-000000000001",
        "marketDataType": "Future",
        "marketData": {
            "sybmol": "ES",
            "bid": 3952.25,
            "ask": 3952.50
        },
        "status": "Validated"
        "createdDate": "2020-01-01T00:00:00Z"
    }
]
```
