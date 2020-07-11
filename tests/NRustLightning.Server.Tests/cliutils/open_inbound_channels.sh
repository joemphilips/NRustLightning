conn=`curl http://localhost:10320/v1/info | jq -r ".connectionString"`

IFS='@'
read -ra PUBKEY_AND_IP_ADDRESS <<< "$conn"

echo "opening inbound channel to nrustlightning: $PUBKEY_AND_IP_ADDRESS"
./docker-lightning-cli.sh fundchannel ${PUBKEY_AND_IP_ADDRESS[0]} 100000


./docker-lnd-cli.sh openchannel ${PUBKEY_AND_IP_ADDRESS[0]} 100000


sleep 5
./docker-bitcoin-cli.sh generatetoaddress 10 2N7LsBkVpuZkAcBbAa6JnNjqNtM9c9AxkcX
