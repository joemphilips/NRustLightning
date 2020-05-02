docker-compose -f docker-compose.regtest.yml exec lnd lncli --tlscertpath=/data/tls.cert --macaroonpath=/data/chain/bitcoin/regtest/admin.macaroon --rpcserver=localhost:32777 $@
