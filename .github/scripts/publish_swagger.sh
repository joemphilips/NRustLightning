
dotnet tool install -g --version 5.5.1 Swashbuckle.AspNetCore.Cli \
  && swagger tofile --output ./docs/dist/swagger-data.json src/NRustLightning.Server/bin/Debug/netcoreapp3.1/NRustLightning.Server.dll v1 \
  && wget https://github.com/swagger-api/swagger-ui/archive/v3.32.1.zip \
  && unzip -d docs/dist -j v3.32.1.zip "*/dist/*" \
  && sed -i -e 's/url: \"https:\/\/petstore.swagger.io\/v2\/swagger.json\"/url: \".\/swagger-data.json\"/' ./docs/dist/index.html
