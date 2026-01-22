# Development environment for Unity C# project
FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy

WORKDIR /app

# Install system dependencies for development
RUN apt-get update && apt-get install -y \
    git \
    curl \
    vim \
    && rm -rf /var/lib/apt/lists/*

# Copy source code (but don't build it)
COPY . .

# Default to shell access
CMD ["/bin/bash"]
