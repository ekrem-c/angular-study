## Story Number
C-1002
 
## Title
#### Open Orders
 
## Description
As a user, I want to see which order have been placed, so I may send them to the kitchen.
 
## Acceptance Criteria
+ Implement *Open Orders* section
    + Display the open order information.
    + Use the `mdTruncate` directive for when the toppings overflow the label
    + Send to kitchen simulates the cooking of a pizza.
        + Pizzas take at least 5s to cook for no toppings
        + Persit the state of `Cooking` for the order status to the API.
        + Calculate the rest of the time based on the time variable for the toppings.
            - Values are found in the [json payload](../../mock-server/mocks/toppings.json)
        + Change the status of the order to `ReadyForDelivery`.
        + Persist the information to the API.
