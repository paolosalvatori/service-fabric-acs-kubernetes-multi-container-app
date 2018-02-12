# Use the private and public key to create a secret used for SSL termination
kubectl create secret tls tls-secret --key tls.key --cert tls.crt