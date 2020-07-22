
# NRustLightning: C# lightning netowrk daemon powered by [rust-lightning](https://github.com/rust-bitcoin/rust-lightning)

## Code organization

* `src/NRustLightning` ... library to interop with rust-lightning through abi.
* `src/NRustLightning.Server` ... to run it as standalone asp.net core server.
* `src/NRustLightning.Client` ... C# client library for the servrer.
* `src/NRustLightning.CLI` ... command-line application  to work with the server (WIP)

## How to configure

`NRustLightning.Server` takes configuration options by either

* cli argument (e.g. `--https.port=443`)
* Evnrironment variable  (e.g. `NRUSTLIGHTNING_HTTPS_PORT=443`)
* configuration ini file (WIP)

You can check all configuration options by running the following command.

```
git clone --recursive <this repository url>
dotnet run --project src/NRustLightning.Server -- --help 
```

You must make sure to connect to your [nbxplorer](https://github.com/dgarage/NBXplorer) instance by options which starts from `nbx`

If you don't have any, you can try with regtest in docker-compsoe. See below.

## Single file executable

`NRustLightning.CLI` and `NRustLightning.Server` supports single file executable.
Run

```
dotnet publish -c Release -r <your RID>
```

for which RID to use, see [microsoft official](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog)

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
