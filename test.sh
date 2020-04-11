source "$HOME/.cargo/env"
export RUST_SRC_PATH="$(rustc --print sysroot)/lib/rustlib/src/rust/src"

# for macos
export DYLD_LIBRARY_PATH="$(rustc --print sysroot)/lib:$DYLD_LIBRARY_PATH"
export DYLD_LIBRARY_PATH="`pwd`/rust-lightning/target/debug:$DYLD_LIBRARY_PATH"

# for linux
export LD_LIBRARY_PATH="$(rustc --print sysroot)/lib:$LD_LIBRARY_PATH"
export LD_LIBRARY_PATH="`pwd`/rust-lightning/target/debug:$LD_LIBRARY_PATH"

cd rust-lightning/bindings && cargo build --features "debug_assertions"
cd ../..
dotnet test NRustLightning.Tests
