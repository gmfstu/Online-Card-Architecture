#!/bin/bash
IMAGE_TAG="${1:-online_card_architecture}"
ARCH="${2:-}"

PLATFORM_FLAG=""
if [ -n "$ARCH" ]; then
    PLATFORM_FLAG="--platform=$ARCH"
fi

docker build $PLATFORM_FLAG -t "$IMAGE_TAG" .
