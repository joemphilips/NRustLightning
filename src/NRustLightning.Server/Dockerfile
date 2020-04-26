FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-bionic AS build
USER root
WORKDIR /src/
RUN apt-get update \
  && apt-get install -y \
  apt-utils \
  curl \
  gcc \
  build-essential  # For compiling some deps (such as secp256k1) with g++ \
  && rm -rf /var/lib/apt/lists/*
# Install Rust
RUN curl https://sh.rustup.rs -sSf > /tmp/rustup-init.sh \
    && chmod +x /tmp/rustup-init.sh \
    && sh /tmp/rustup-init.sh -y \
    && rm -rf /tmp/rustup-init.sh
ENV PATH "$PATH:/root/.cargo/bin"
RUN which cargo
# Install nightly rust.
RUN rustup install nightly
RUN rustup default nightly


# --- workaround to restore only deps for caching. -- 
RUN USER=root cargo new --lib lightning
RUN USER=root cargo new --lib bindings
RUN USER=root cargo new --bin lightning-net-tokio
COPY rust-lightning/Cargo* ./
COPY rust-lightning/bindings/Cargo.toml ./bindings/
COPY rust-lightning/lightning/Cargo.toml ./lightning/
RUN cargo build --release && rm -rf bindings lightning lightning-net-tokio
# -- --

COPY rust-lightning/lightning ./lightning/
COPY rust-lightning/bindings ./bindings/

WORKDIR /src

COPY ["src/NRustLightning.Server/NRustLightning.Server.csproj", "src/NRustLightning.Server/"]
RUN dotnet restore "src/NRustLightning.Server/NRustLightning.Server.csproj"
COPY . .
WORKDIR "/src/src/NRustLightning.Server"
RUN dotnet build "NRustLightning.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NRustLightning.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NRustLightning.Server.dll"]
