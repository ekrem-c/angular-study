## Standards
Follow the Angular standards set forth the in the [Angular Style Guide](https://angular.io/guide/styleguide)

Additional standards that are advisable to follow:
+ All variables must be typed.
+ Do not type variables with `any`.
+ Include access modifiers for **ALL** member variables (does not include `@Input, @Output, @ViewChild, etc.)

+ Include an access modifier and return type for **ALL** functions.

+ Create meaningful names for steam objects.

## Running the dev environment

The project is configured to use a mock API server. Data is not configured to persist and will be reset 
when `json-server`is restarted.

Use the following command to start the application with the mock server running in the background.

`npm run start:dev:mock`
