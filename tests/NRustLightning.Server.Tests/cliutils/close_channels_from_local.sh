lnd_pubkey=`./docker-lnd-cli.sh getinfo | jq ".identity_pubkey"`

curl -v -H "Content-Type: application/json" -X DELETE --data '{"TheirNetworkKey": '${lnd_pubkey}'}' http://localhost:10320/v1/channel/btc
