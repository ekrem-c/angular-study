## Story Number
C-1001

## Title
New Orders

## Description
As a user, I want to add new order details, so that it may be created in the system.

## Acceptance Criteria
+ Implement the *New Order* section
    + The order must call the API to persist the data. Data should be available when the application is restarted
    + Customer name should accept alpha-numeric input only.
    + Toppings is a multi-select control.
    + Size is a single select control.
    + Upon placing the order, it should appear in the *Open Orders* in real-time.
    + Change the status of the order to `New`.
