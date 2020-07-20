conn=`curl http://localhost:10320/v1/info | jq -r ".connectionString"`
./docker-lnd-cli.sh connect $conn


IFS='@'
read -ra PUBKEY_AND_IP_ADDRESS <<< "$conn"
echo $PUBKEY_AND_IP_ADDRESS

./docker-lightning-cli.sh connect ${PUBKEY_AND_IP_ADDRESS[0]} ${PUBKEY_AND_IP_ADDRESS[1]}

