FROM mcr.microsoft.com/devcontainers/dotnet:0-7.0

ENV PATH $PATH:/home/vscode/.dotnet:/home/vscode/.dotnet/tools

RUN apt-get clean -y && rm -rf /var/lib/apt/lists/* /tmp/library-scripts

RUN apt-get update && export DEBIAN_FRONTEND=noninteractive \
    && apt-get -y install --no-install-recommends libnss3-tools htop nano curl jq

RUN sudo dotnet dev-certs https \
    && sudo -E dotnet dev-certs https -ep /usr/local/share/ca-certificates/aspnet/https.crt --format PEM \
    && sudo update-ca-certificates

# Used to persist bash history as per https://code.visualstudio.com/remote/advancedcontainers/persist-bash-history
RUN SNIPPET="export PROMPT_COMMAND='history -a' && export HISTFILE=/commandhistory/.bash_history" \
    && echo "$SNIPPET" >> "/root/.bashrc"

# install jless (because its not available at https://containers.dev/features at time of writing)
# RUN NONINTERACTIVE=1 /bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)" \
#         && (echo; echo 'eval "$(/home/linuxbrew/.linuxbrew/bin/brew shellenv)"') >> /home/vscode/.profile \
#         && eval "$(/home/linuxbrew/.linuxbrew/bin/brew shellenv)" \
#         && brew install jless

# RUN  apt-get update \
#     && apt-get install -y wget gnupg ca-certificates procps libxss1 \
#     && wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add - \
#     && sh -c 'echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google.list' \
#     && apt-get update \
#     # We install Chrome to get all the OS level dependencies, but Chrome itself
#     # is not actually used as it's packaged in the node puppeteer library.
#     # Alternatively, we could could include the entire dep list ourselves
#     # (https://github.com/puppeteer/puppeteer/blob/master/docs/troubleshooting.md#chrome-headless-doesnt-launch-on-unix)
#     # but that seems too easy to get out of date.
#     && apt-get install -y google-chrome-stable \
#     && rm -rf /var/lib/apt/lists/* \
#     && wget --quiet https://raw.githubusercontent.com/vishnubob/wait-for-it/master/wait-for-it.sh -O /usr/sbin/wait-for-it.sh \
#     && chmod +x /usr/sbin/wait-for-it.sh

