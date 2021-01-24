#! /bin/bash
# build docker image
#$> docker build -f Dockerfile.Remote -t testtarget.local:1 .
# run kind and create a cluster
#$> kind create cluster --name ykind
# run kind to pre-load the container image so we can create pods through kubectl.
#$> kind load docker-image testtarget.local:1
# create K8s resources from this folder.
kubectl apply -f ./
kubectl get pv
kubectl get pvc
kubectl get configmaps
kubectl get all

##get list of pods that belong to the job
pods=$(kubectl get pods --selector=job-name="testtarget-job" --output=jsonpath='{.items[*].metadata.name}')
echo $pods