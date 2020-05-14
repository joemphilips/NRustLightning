
# NRustLightning: C# lightning netowrk daemon powered by [rust-lightning](https://github.com/rust-bitcoin/rust-lightning)

## How to test

```
source tests/NRustLightning.Server.Tests/env.sh
docker-compose -f tests/NRustLightning.Server.Tests/docker-compose.yml up
./tests/NRustLightning.Server.Tests/docker-lnd-cli.sh
```

