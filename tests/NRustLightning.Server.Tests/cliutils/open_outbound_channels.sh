lnd_pubkey=`./docker-lnd-cli.sh getinfo | jq ".identity_pubkey"`
curl -H "Content-Type: application/json" -X POST --data '{"TheirNetworkKey": '${lnd_pubkey}', "ChannelValueSatoshis": 100000, "PushMSat": 1000 }' http://localhost:10320/v1/channel/BTC

sleep 5

./docker-bitcoin-cli.sh generatetoaddress 10 2N7LsBkVpuZkAcBbAa6JnNjqNtM9c9AxkcX
