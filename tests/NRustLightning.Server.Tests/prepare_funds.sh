addr=`./docker-lnd-cli.sh newaddress p2wkh | jq -r ".address"`
./docker-bitcoin-cli.sh sendtoaddress $addr 1

addr=`./docker-lightning-cli.sh newaddr | jq -r ".address"`
./docker-bitcoin-cli.sh sendtoaddress $addr 1

addr=`curl http://localhost:10320/v1/wallet/btc/address | jq -r ".Address"`
./docker-bitcoin-cli.sh sendtoaddress $addr 1

addr=`./docker-bitcoin-cli.sh getnewaddress`
./docker-bitcoin-cli.sh generatetoaddress 101 $addr

