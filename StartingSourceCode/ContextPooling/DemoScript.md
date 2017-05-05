# Demo: Context Pooling

## Everytime Setup

Required before/after each run of the demo
* Reset the starting point source code (`git reset --hard`)


## Demo Steps

* Walk thru what the code is doing (simulating a bunch of concurrent requests in a web application)
* Run the app and point out the throughput and number of context instances created
* Edit `Main(...)` to use `AddDbContextPool` rather than `AddDbContext`

```c#
var serviceProvider = new ServiceCollection()
	.AddEntityFrameworkSqlServer()
	.AddDbContextPool<BloggingContext>(
		c => c.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Demo.ContextPooling;Trusted_Connection=True;ConnectRetryCount=0;"))
	.BuildServiceProvider();
```

* Run the app and contrast to the previous throughput and number of context instances created
* Optionally, show that you can adjust the max pool size

```c#
var serviceProvider = new ServiceCollection()
	.AddEntityFrameworkSqlServer()
	.AddDbContextPool<BloggingContext>(
		c => c.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Demo.ContextPooling;Trusted_Connection=True;ConnectRetryCount=0;"),
		poolSize: 32)
	.BuildServiceProvider();
```
