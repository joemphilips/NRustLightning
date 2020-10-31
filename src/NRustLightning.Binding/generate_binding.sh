#!/usr/bin/env bash

# The code used to generate Binding.cs

set -eu
SOLUTION_ROOT=~/Working/sandbox/csharp/NRustLightning

git clone --quiet --depth 1 --single-branch --branch 2e4f410 https://github.com/microsoft/ClangSharp 
cd ClangSharp/sources/CLangSharpPInvokeGenerator

dotnet run --framework=netcoreapp3.1 -- \
    -f ${SOLUTION_ROOT}/rust-lightning/lightning-c-bindings/include/rust_types.h \
    -n "NRustLightning.Binding" \
    -o Binding.cs \
    -I /usr/local/opt/llvm/lib/clang/10.0.1/include /usr/local/opt/llvm/include/c++/v1 \
    -L /usr/local/opt/llvm/lib

