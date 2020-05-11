
# Integration tests for NRustLightning

To test the interoperability against other lightning nodes, some tests in this repo will launch `docker-compose` .

This is similar to [`BTCPayServer.Tests`](https://github.com/btcpayserver/btcpayserver/tree/master/BTCPayServer.Tests) but with one big difference, you don't have to launch docker-compose before
running any tests, it will launch independent `docker-compose` instance for every tests class,
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

