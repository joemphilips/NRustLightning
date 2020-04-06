source "$HOME/.cargo/env"
export RUST_SRC_PATH="$(rustc --print sysroot)/lib/rustlib/src/rust/src"
export DYLD_LIBRARY_PATH="$(rustc --print sysroot)/lib:$DYLD_LIBRARY_PATH"
export DYLD_LIBRARY_PATH="`pwd`/rust-lightning/target/debug:$DYLD_LIBRARY_PATH"

cd rust-lightning/bindings && cargo build --features "debug_assertions"
cd ../..
dotnet test --filter "FullyQualifiedName~BroadcasterTestWrapper" DotNetLightning.LDK.Tests
