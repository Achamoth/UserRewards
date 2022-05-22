# UserRewards
Simple wep application that exposes two RESTful APIs for querying and redeeming user rewards.

I've written the app using ASP.NET Core.

## Assumptions
I've made some assumptions and decisions about business logic where I had uncertainties:
- I only allow a reward to be redeemed between the _AvailableAt_ and _ExpiredAt_ times.
- New users are only created via the GET endpoint. Attempting to redeem a reward for a non-existent user will produce an error.
- New weekly rewards are only generated via the GET endpoint. Attempting to redeeem a non-existent reward will produce an error. Could be amended to trigger the creation flow of the GET API if the user/reward doesn't exist (would move that logic from the GET API into a shared workflow).
- If the GET endpoint is called without an _at_ query parameter, I process it with the current time (assuming the user wants to see rewards for the current week).
- Attempting to redeem the same reward multiple times has no effect; the operation will simply return the time at which the reward was redeeemed, rather than an error. Could easily be amended to generate an error or warning on subsequent attempts.

## Simplifications
For the sake of simplicity I have simply used file storage to persist the data. The implementation is extremely rudimentary, and I've given almost no thought to performance.

In reality I would likely use a Sqlite DB with some sort of lightweight ORM. I've abstracted the access to persistent storage as much as possible with the idea of making this transition relatively easy, and keeping the business logic reasonably agnostic to the underlying storage mechanism for now. All access is via a general interface whose implementation is currently decided via an appsettings config item.

## Decisions
I've built an extremely lightweight command execution framework to keep the endpoint controller simple and concentrate business logic in one area. Reflection allows for easy auto-registration of all commands in the DI container, foregoing the need to register any new commands as they're added.

Error-handling for validation errors is done by throwing a custom exception, which is transformed into custom error output by a simple middleware filter. This allows me to keep sensible and simple return types for endpoints, and a consistent means of returning user errors.
