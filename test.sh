. "$HOME/.cargo/env"
export RUST_SRC_PATH="$(rustc --print sysroot)/lib/rustlib/src/rust/src"

cd rust-lightning/bindings && cargo build --features "debug_assertions"
cd ../..
dotnet test tests/NRustLightning.Tests
