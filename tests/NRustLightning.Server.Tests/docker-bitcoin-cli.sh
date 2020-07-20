#!/bin/bash

docker-compose exec -T bitcoind bitcoin-cli -datadir="/data" -rpcport=${BITCOIND_RPC_PORT} -rpcpassword=${BITCOIND_RPC_PASS} -rpcuser=${BITCOIND_RPC_USER} "$@"
