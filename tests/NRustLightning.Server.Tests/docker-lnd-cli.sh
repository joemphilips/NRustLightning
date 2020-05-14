#!/bin/bash

docker-compose exec lnd lncli --tlscertpath=/data/tls.cert --macaroonpath=/data/chain/bitcoin/regtest/admin.macaroon --rpcserver=localhost:${LND_REST_PORT} $@
