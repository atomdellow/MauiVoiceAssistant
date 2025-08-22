#!/bin/bash

# Build Vue UI
echo "Building Vue UI..."
cd VueUI
npm run build
cd ..

# Copy Vue build to MAUI Resources
echo "Copying Vue build to MAUI Resources..."
rm -rf MauiApp/Resources/Raw/webapp/*
cp -r VueUI/dist/* MauiApp/Resources/Raw/webapp/

echo "Build preparation complete!"