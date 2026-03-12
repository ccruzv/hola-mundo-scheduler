#!/bin/bash
set -euo pipefail

IMAGE_NAME="hola-mundo-scheduler"
IMAGE_TAG="${1:-latest}"

echo "==> Building Docker image: ${IMAGE_NAME}:${IMAGE_TAG}"
docker build -t "${IMAGE_NAME}:${IMAGE_TAG}" .

echo "==> Importing image into microk8s registry"
docker save "${IMAGE_NAME}:${IMAGE_TAG}" | microk8s ctr image import -

echo "==> Applying Kubernetes manifests"
microk8s kubectl apply -f k8s/namespace.yaml
microk8s kubectl apply -f k8s/resource-quota.yaml
microk8s kubectl apply -f k8s/deployment.yaml

echo "==> Deployment complete. Checking rollout status..."
microk8s kubectl rollout status deployment/hola-mundo-scheduler -n default

echo "==> Logs (últimas 20 líneas):"
sleep 3
microk8s kubectl logs deployment/hola-mundo-scheduler -n default --tail=20
