#!/bin/bash

bitcoind_container_id="$(docker ps -q --filter label=com.docker.compose.project=nrustlightningservertests --filter label=com.docker.compose.service=bitcoind)"
docker exec -ti "$bitcoind_container_id" bitcoin-cli -datadir="/data" -rpcport=${BITCOIND_RPC_PORT} -rpcpassword=${BITCOIND_RPC_PASS} -rpcuser=${BITCOIND_RPC_USER} "$@"
