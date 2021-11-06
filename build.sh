#!/bin/bash
rm -rf ./bin/Debug/netcoreapp3.1/win-x64/*
dotnet build --runtime win-x64
cp -r ./Assets ./bin/Debug/netcoreapp3.1/win-x64/Assets

