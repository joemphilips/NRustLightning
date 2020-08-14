# Tests for the server

## Code organization

There are three kinds of tests

* Integration test without docker-compose
  * see `UnitTest1.cs`
  * these are mainly for testing the behavior of controller which does not directly depend on other daemon.
* Integration test with `bitcoind` and `nbxplorer`
  * see `ExplorerIntegrationTests/`
  * These are more heavier than above. mainly for simulating reorg.
* Full-integration tests against other lightning nodes.
  * See `LNIntegrationTests/`
  * These are the most heavy-weight test.
  * See below for the detail.

## Integration tests against other lightning nodes

To test the interoperability against other lightning nodes, some tests in this repo will launch `docker-compose` .

This is similar to [`BTCPayServer.Tests`](https://github.com/btcpayserver/btcpayserver/tree/master/BTCPayServer.Tests) but with one big difference, you don't have to launch docker-compose before
running any tests, it will launch independent `docker-compose` instance for every test class which inherits from `IClassFixture<DockerFixture>`,
and it stops and clean up when all tests in the test class finishes.

## how to check the behaviour manually

each servers in a docker-compose will bind to different ports, this is accomplished by giving the port number as a parameter
via environment variables. So you must set those variables in your environment if you want to manually run docker-compose and check the behavior.
For this purpose, I have included a `env.sh` to automatically export the variables so you can just run it without worrying much about the detail.

```sh
./env.sh
docker-compose up
# do anything you want
docker-compose down -v --remove-orphans
```

there is a script for using rpc in docker-compose easily. Those are...

* `./docker-bitcoin-cli.sh`
* `./docker-lightning-cli.sh`
* `./docker-lnd-cli.sh`

Which is just a proxy for cli runner for each daemon.
 
More high level facades are included in `cliutils/` subfolder.
## CI

We wanted to run this test in CI. But lnd behaves wanky and it returns code 500 sometimes, so we are not doing CI for now.

