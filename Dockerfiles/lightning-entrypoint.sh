#!/usr/bin/env bash

: "${EXPOSE_TCP:=false}"

datadir="${LIGHTNINGD_DATA}/${LIGHTNINGD_NETWORK}"

if [ "$EXPOSE_TCP" == "true" ]; then
    set -m
    lightningd "$@" &

    echo "C-Lightning starting"
    while read i; do if [ "$i" = "lightning-rpc" ]; then break; fi; done \
    < <(inotifywait -r  -e create,open --format '%f' --quiet "${LIGHTNIND_DATA}" --monitor)
    echo "C-Lightning started"
    echo "C-Lightning started, RPC available on port $LIGHTNINGD_RPC_PORT"

    socat "TCP4-listen:$LIGHTNINGD_RPC_PORT,fork,reuseaddr" "UNIX-CONNECT:${datadir}/lightning-rpc" &
    fg %-
else
    exec lightningd --network=${LIGHTNINGD_NETWORK} "$@"
fi
