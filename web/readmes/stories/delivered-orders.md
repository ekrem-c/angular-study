## Story Number
C-1004

## Title
Delivered Orders
  
## Description
As a user, I want to be able to view orders that are delivered, so that I may know that they are cmoplete.
 
## Acceptance Criteria
+ Implement *Delivered* section
    + Each pizza takes 15s to be delivered
    + Pizzas may be queued for the same driver
    + Pizzas are delivered one at a time
    + The driver is no longer available in the select for the _Ready for Delivery_ section
    + Changed the status of the order to `Delivered` once the delivery is complete
    + Persist the information to the API
