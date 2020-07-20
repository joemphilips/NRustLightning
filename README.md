
# NRustLightning: C# lightning network daemon powered by [rust-lightning](https://github.com/rust-bitcoin/rust-lightning)

## Code organization

* `src/NRustLightning` ... library to interop with rust-lightning through abi.
* `src/NRustLightning.Server` ... to run it as standalone asp.net core server.
* `src/NRustLightning.Client` ... C# client library for the server.
* `src/NRustLightning.CLI` ... command-line application  to work with the server (WIP)

## How to configure

`NRustLightning.Server` takes configuration options by either

* cli argument (e.g. `--https.port=443`)
* Environment variable  (e.g. `NRUSTLIGHTNING_HTTPS_PORT=443`)
* configuration ini file (WIP)

You can check all configuration options by running the following command.

```
git clone --recursive <this repository url>
dotnet run --project src/NRustLightning.Server -- --help 
```

You must make sure to connect to your [nbxplorer](https://github.com/dgarage/NBXplorer) instance by options which starts from `nbx`

If you don't have any, you can try with regtest in docker-compose. See below.

## How to test

```
source tests/NRustLightning.Server.Tests/env.sh
docker-compose -f tests/NRustLightning.Server.Tests/docker-compose.yml up
./tests/NRustLightning.Server.Tests/docker-lnd-cli.sh
```

## API

We expose the swagger endpoint in debug build. If you want to check it out quickly, first run regtest server by

```
source tests/NRustLightning.Server.Tests/env.sh
docker-compose -f tests/NRustLightning.Server.Tests/docker-compose.yml up
```


and access to `http://localhost:10320/swagger/`
