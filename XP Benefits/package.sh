#!/bin/bash
VERSION="v2.0.0"
MOD_NAME="XP Benefits"
ECO_VERSION=$1
PACKAGE_NAME=$2
if [ -d "./${PACKAGE_NAME}" ]; then rm -r "./${PACKAGE_NAME}"; fi
cp -r "./src" "./${PACKAGE_NAME}"
cp "./README.md" "./${PACKAGE_NAME}/Mods/UserCode/${MOD_NAME}/README.md"
cp "./NOTICE" "./${PACKAGE_NAME}/Mods/UserCode/${MOD_NAME}/NOTICE"
sed -i "1s/.*/# ${MOD_NAME} ${VERSION} for Eco ${ECO_VERSION}/" "./${PACKAGE_NAME}/Mods/UserCode/${MOD_NAME}/README.md"