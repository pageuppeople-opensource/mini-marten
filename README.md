# mini-marten

Quick test repo that shows some different marten patterns in use

## Prereq 

.Net 6 SDK or Visual Studio 2022
Make sure you have a postgres (14) db set up, change the connection string in Program.cs to match.


## How to navigate

Have a look at `Program.cs` to see how marten is configured.
`DocumentController` Has some basic crud operations for demonstration.
MartenApi.EventStore has an example of how the events/aggregations/services can be structured.
