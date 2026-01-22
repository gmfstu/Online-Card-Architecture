#!/bin/bash
IMAGE_TAG="${1:-online_card_architecture}"
CONTAINER_NAME="${2:-$IMAGE_TAG}"
shift 2 2>/dev/null || shift 2>/dev/null || true

DETACH=""
RM_FLAG="--rm"

for arg in "$@"; do
    case "$arg" in
        -d|--detach) DETACH="-d" ;;
        -k|--keep) RM_FLAG="" ;;
    esac
done

docker run $DETACH -it $RM_FLAG --name "$CONTAINER_NAME" "$IMAGE_TAG"
