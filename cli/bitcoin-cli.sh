docker-compose -f docker-compose.regtest.yml exec bitcoind bitcoin-cli -regtest -datadir=/data -rpcport=43782 $@

